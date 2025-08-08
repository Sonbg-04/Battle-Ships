using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sonn.BattleShips
{
    public class EnemyAI : MonoBehaviour, IComponentChecking
    {
        public GameObject enemyCellPrefab;
        public GameObject[] enemyShipPrefabs;
        public Vector3 offsetPos, offsetScalePos;
        public bool isEnemyShoot, isEnemySelectedCell, isEnemyHunting;

        private Cell[, ] m_enemyCell;
        private List<Cell> m_enemyCellList, m_occupiedEnemyCells, 
                           m_potentialTargetCells, m_currentHitCells;
        private Vector2Int? m_enemyFirstHit, m_enemyLastHit;
        private int m_enemyHuntDir = -1;
        private bool m_enemyDirConfirmed = false, m_enemyReservedTried = false,
            m_playerCellDiscovered = false;
        private GameManager m_gameMng;

        public List<Cell> EnemyCellList { get => m_enemyCellList; }

        private void Awake()
        {
            m_enemyCell = new Cell[GridManager.Ins.Row, GridManager.Ins.Col];
            m_enemyCellList = new();    
            m_gameMng = FindObjectOfType<GameManager>();
            m_occupiedEnemyCells = new();
            m_potentialTargetCells = new();
            m_currentHitCells = new();
            m_enemyFirstHit = null;
            m_enemyLastHit = null;
        }
        private void Start()
        {
            EnemyDrawGridMap();
            OffsetOfGrid();
            foreach (var s in enemyShipPrefabs)
            {
                if (s != null)
                {
                    EnemyCreateAndPlaceShips(s);
                }    
            }
        }
        public bool IsComponentNull()
        {
            bool check = GridManager.Ins == null || m_gameMng == null;
            if (check)
            {
                Debug.LogWarning("Có component bị rỗng. Hãy kiểm tra lại!");
            }
            return check;
        }
        private void EnemyDrawGridMap()
        {
            if (IsComponentNull())
            {
                return;
            }

            for (int x = 0; x < GridManager.Ins.Row; x++)
            {
                for (int y = 0; y < GridManager.Ins.Col; y++)
                {
                    var enemyCell = Instantiate(enemyCellPrefab, Vector3.zero, Quaternion.identity);
                    Vector3 pos = new(
                        x * -GridManager.Ins.CellDistance,
                        y * -GridManager.Ins.CellDistance,
                        0
                        );
                    enemyCell.transform.position = pos;
                    enemyCell.transform.SetParent(transform);
                    enemyCell.tag = Const.ENEMY_CELL_TAG;
                    enemyCell.layer = LayerMask.NameToLayer(Const.ENEMY_CELL_LAYER);
                    enemyCell.name = $"EnemyCell[{x}][{y}]";

                    var c = enemyCell.GetComponent<Cell>();
                    if (c == null)
                    {
                        return;
                    }
                    m_enemyCell[x, y] = c;
                    m_enemyCell[x, y].cellPosOnGrid = new Vector2Int(
                        Mathf.RoundToInt(m_enemyCell[x, y].transform.position.x / -GridManager.Ins.CellDistance),
                        Mathf.RoundToInt(m_enemyCell[x, y].transform.position.y / -GridManager.Ins.CellDistance)
                        );

                    m_enemyCellList.Add(m_enemyCell[x, y]);
                }
            }
        }
        public void EnemyFindPlayerCell()
        {
            if (IsComponentNull())
            {
                return;
            }    

            if (!m_playerCellDiscovered)
            {
                GameObject[] objs = GameObject.FindGameObjectsWithTag(Const.PLAYER_CELL_TAG);
                if (objs.Length > 0)
                {
                    m_gameMng.playerCells.AddRange(objs);
                    m_playerCellDiscovered = true;
                    Debug.Log($"Có {m_gameMng.playerCells.Count} ô người chơi mà kẻ thù tìm thấy!");
                }
            }    
        }    
        private void OffsetOfGrid()
        {
            transform.position += offsetPos;

            transform.localScale = new Vector3(
                transform.localScale.x * offsetScalePos.x,
                transform.localScale.y * offsetScalePos.y,
                transform.localScale.z
                );

        }
        private bool IsInsideEnemyGrid(int x, int y)
        {
            return x >= 0 && x < GridManager.Ins.Row &&
                   y >= 0 && y < GridManager.Ins.Col; 
        }    
        private bool IsEnemyShipNextToAnother(List<Cell> occupiedCells)
        {
            foreach (var enemyC in occupiedCells)
            {
                Vector2Int CPos = enemyC.cellPosOnGrid;

                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        if (x == 0 || y == 0)
                        {
                            continue;
                        }
                        Vector2Int neighbor = new(CPos.x + x, CPos.y + y);
                        foreach (var c in m_enemyCellList)
                        {
                            if (c.cellPosOnGrid == neighbor && c.hasEnemyShip)
                            {
                                return true;
                            }
                        }    
                    }    
                }
            }
            return false;
        }
        private bool CanEnemyShipPlace(int startX, int startY, bool isVertical,
                               int shipEnemySize, List<Cell> outEnemyCells)
        {
            outEnemyCells.Clear();

            List<Cell> tempCells = new();

            for (int i = 0; i < shipEnemySize; i++)
            {
                int x = isVertical ? startX : (startX + i);
                int y = isVertical ? (startY + i) : startY;

                if (!IsInsideEnemyGrid(x, y))
                {
                    return false;
                }

                var enemyCell = m_enemyCell[x, y];
                if (enemyCell.hasEnemyShip)
                {
                    return false;
                }

                tempCells.Add(enemyCell);
            }

            if (IsEnemyShipNextToAnother(tempCells))
            {
                return false;
            }

            outEnemyCells.AddRange(tempCells);
            return true;
        }

        private void EnemyCreateAndPlaceShips(GameObject ship)
        {
            if (IsComponentNull() || ship == null)
            {
                return;
            }

            bool isShipEnemyPlaced = false;
            int attemp = 0, maxAttemp = 100;
            int shipSize = ship.transform.childCount;
            
            if (shipSize <= 0)
            {
                return;
            }

            while (!isShipEnemyPlaced && attemp < maxAttemp)
            {
                attemp++;

                bool vertical = Random.value > 0.5f;

                int maxX = vertical
                           ? (GridManager.Ins.Row - shipSize)
                           : (GridManager.Ins.Row - 1);
                int maxY = vertical
                           ? (GridManager.Ins.Col - 1)
                           : (GridManager.Ins.Col - shipSize);
                if (maxX < 0 || maxY < 0)
                {
                    break;
                }    

                int startX = Random.Range(0, maxX + 1);
                int startY = Random.Range(0, maxY + 1);

                if (!CanEnemyShipPlace(startX, startY, vertical, shipSize, m_occupiedEnemyCells))
                {
                    continue;
                }

                var newEnemyShip = Instantiate(ship, Vector3.zero, Quaternion.identity);
                newEnemyShip.name = $"EnemyShip_{shipSize}";
                newEnemyShip.layer = LayerMask.NameToLayer(Const.ENEMY_SHIP_LAYER);
                SetTagEnemy(newEnemyShip);
                newEnemyShip.GetComponent<Collider2D>().enabled = false;

                Vector3 centerPos = Vector3.zero;
                foreach (var item in m_occupiedEnemyCells)
                {
                    centerPos += item.transform.position;
                }
                centerPos /= m_occupiedEnemyCells.Count;

                newEnemyShip.transform.SetPositionAndRotation(
                                    centerPos,
                                    vertical ? Quaternion.identity
                                             : Quaternion.Euler(0, 0, -90f)
                                 );

                var s = newEnemyShip.GetComponent<Ship>();
                if (s != null)
                {
                    s.isRotatedShip = !vertical;
                    s.isPlacedShip = true;
                }

                var shipRenderer = newEnemyShip.GetComponentInChildren<SpriteRenderer>();
                if (shipRenderer != null)
                {
                    shipRenderer.enabled = false;
                }

                for (int i = 0; i < shipSize; i++)
                {
                    var part = newEnemyShip.transform.GetChild(i);
                    part.gameObject.layer = LayerMask.NameToLayer(Const.ENEMY_SHIP_LAYER);
                    var cell = m_occupiedEnemyCells[i];
                    
                    cell.hasEnemyShip = true;
                    cell.shipPartTransform = part;

                    if (part.CompareTag(Const.ENEMY_SHIP_PART_2_1_TAG))
                    {
                        part.localPosition += new Vector3(0, -0.17f, 0);
                    }
                    else if (part.CompareTag(Const.ENEMY_SHIP_PART_2_2_TAG))
                    {
                        part.localPosition += new Vector3(0, -0.02f, 0);
                    }
                    else if (part.CompareTag(Const.ENEMY_SHIP_PART_3_1_TAG))
                    {
                        part.localPosition += new Vector3(0, -0.24f, 0);
                    }
                    else if (part.CompareTag(Const.ENEMY_SHIP_PART_3_2_TAG))
                    {
                        part.localPosition += new Vector3(0, -0.1f, 0);
                    }
                    else if (part.CompareTag(Const.ENEMY_SHIP_PART_3_3_TAG))
                    {
                        part.localPosition += new Vector3(0, -0.02f, 0);
                    }
                    else if (part.CompareTag(Const.ENEMY_SHIP_PART_4_1_TAG))
                    {
                        part.localPosition += new Vector3(0, -0.39f, 0);
                    }
                    else if (part.CompareTag(Const.ENEMY_SHIP_PART_4_2_TAG))
                    {
                        part.localPosition += new Vector3(0, -0.25f, 0);
                    }
                    else if (part.CompareTag(Const.ENEMY_SHIP_PART_4_3_TAG))
                    {
                        part.localPosition += new Vector3(0, -0.13f, 0);
                    }
                    else if (part.CompareTag(Const.ENEMY_SHIP_PART_4_4_TAG))
                    {
                        part.localPosition += Vector3.zero;
                    }
                    else if (part.CompareTag(Const.ENEMY_SHIP_PART_5_1_TAG))
                    {
                        part.localPosition += new Vector3(0, -0.48f, 0);
                    }
                    else if (part.CompareTag(Const.ENEMY_SHIP_PART_5_2_TAG))
                    {
                        part.localPosition += new Vector3(0, -0.39f, 0);
                    }
                    else if (part.CompareTag(Const.ENEMY_SHIP_PART_5_3_TAG))
                    {
                        part.localPosition += new Vector3(0, -0.25f, 0);
                    }
                    else if (part.CompareTag(Const.ENEMY_SHIP_PART_5_4_TAG))
                    {
                        part.localPosition += new Vector3(0, -0.11f, 0);
                    }
                    else if (part.CompareTag(Const.ENEMY_SHIP_PART_5_5_TAG))
                    {
                        part.localPosition += new Vector3(0, -0.02f, 0);
                    }
                }

                isShipEnemyPlaced = true;
            }
        }
        private void SetTagEnemy(GameObject go)
        {
            if (go != null)
            {
                if (go.transform.childCount == 2)
                {
                    go.tag = Const.ENEMY_SHIP_2_TAG;
                    go.transform.GetChild(0).tag = Const.ENEMY_SHIP_PART_2_1_TAG;
                    go.transform.GetChild(1).tag = Const.ENEMY_SHIP_PART_2_2_TAG;
                }
                else if (go.transform.childCount == 3)
                {
                    go.tag = Const.ENEMY_SHIP_3_TAG;
                    go.transform.GetChild(0).tag = Const.ENEMY_SHIP_PART_3_1_TAG;
                    go.transform.GetChild(1).tag = Const.ENEMY_SHIP_PART_3_2_TAG;
                    go.transform.GetChild(2).tag = Const.ENEMY_SHIP_PART_3_3_TAG;
                }
                else if (go.transform.childCount == 4)
                {
                    go.tag = Const.ENEMY_SHIP_4_TAG;
                    go.transform.GetChild(0).tag = Const.ENEMY_SHIP_PART_4_1_TAG;
                    go.transform.GetChild(1).tag = Const.ENEMY_SHIP_PART_4_2_TAG;
                    go.transform.GetChild(2).tag = Const.ENEMY_SHIP_PART_4_3_TAG;
                    go.transform.GetChild(3).tag = Const.ENEMY_SHIP_PART_4_4_TAG;
                }
                else if (go.transform.childCount == 5)
                {
                    go.tag = Const.ENEMY_SHIP_5_TAG;
                    go.transform.GetChild(0).tag = Const.ENEMY_SHIP_PART_5_1_TAG;
                    go.transform.GetChild(1).tag = Const.ENEMY_SHIP_PART_5_2_TAG;
                    go.transform.GetChild(2).tag = Const.ENEMY_SHIP_PART_5_3_TAG;
                    go.transform.GetChild(3).tag = Const.ENEMY_SHIP_PART_5_4_TAG;
                    go.transform.GetChild(4).tag = Const.ENEMY_SHIP_PART_5_5_TAG;
                }
            }
        }
        public void EnemyTurning()
        {
            if (IsComponentNull())
            {
                return;
            }    

            if (!isEnemySelectedCell)
            {
                isEnemySelectedCell = true;
                isEnemyShoot = true;
                StartCoroutine(EnemyShootCoroutine());
            }    
        }
        IEnumerator EnemyShootCoroutine()
        {
            bool keepShooting = true;
            while (keepShooting)
            {
                yield return new WaitForSeconds(0.5f);

                int prevHitCount = m_currentHitCells.Count;
                EnemyChoosePlayerCell();
                bool justHit = (m_currentHitCells.Count > prevHitCount);
                keepShooting = justHit;
            }
            isEnemyShoot = false;
            m_gameMng.WaitNextTurn(1);
        }
        private Vector2Int DirectionToVector(int direction)
        {
            return direction switch
            {
                0 => Vector2Int.up,
                1 => Vector2Int.down,
                2 => Vector2Int.right,
                3 => Vector2Int.left,
                _ => Vector2Int.zero
            };
        }    
        private int OppositeDirection(int direction)
        {
            return direction switch
            {
                0 => 1,
                1 => 0,
                3 => 2,
                2 => 3,
                _ => m_enemyHuntDir
            };
        }   
        private void StopEnemyHunting()
        {
            isEnemyHunting = false;
            m_currentHitCells.Clear();
            m_potentialTargetCells.Clear();
            m_enemyFirstHit = null;
            m_enemyLastHit = null;
            m_enemyHuntDir = -1;
            m_enemyDirConfirmed = false;
            m_enemyReservedTried = false;
        }    
        private void EnemyAddPotentialTarget(Cell c)
        {
            if (IsComponentNull() || c == null || c.isHit)
            {
                return;
            }    
            if (!m_potentialTargetCells.Contains(c))
            {
                m_potentialTargetCells.Add(c);
            }    
        }    
        private Cell FindPlayerCell(Vector2Int pos)
        {
            foreach (var c in GridManager.Ins.CellList)
            {
                if (c != null && c.cellPosOnGrid == pos)
                {
                    return c;
                }    
            }
            return null;
        }    
        private void EnemyRigisterHit(Cell c)
        {
            if (IsComponentNull() || c == null)
            {
                return;
            }    
            m_currentHitCells.Add(c);
            isEnemyHunting = true;
            
            if (!m_enemyFirstHit.HasValue)
            {
                m_enemyFirstHit = c.cellPosOnGrid;
            }    

            m_enemyLastHit = c.cellPosOnGrid;

            if (m_currentHitCells.Count == 1)
            {
                m_potentialTargetCells.Clear();
                int[] dir = new[] { 0, 1, 2, 3 };
                foreach (var d in dir)
                {
                    var n = FindPlayerCell(c.cellPosOnGrid + DirectionToVector(d));
                    EnemyAddPotentialTarget(n);
                }
            }
            else if (m_currentHitCells.Count == 2 && !m_enemyDirConfirmed)
            {
                Vector2Int delta = m_enemyLastHit.Value - m_enemyFirstHit.Value;
                if (delta != Vector2Int.zero)
                {
                    if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                    {
                        m_enemyHuntDir = delta.x > 0 ? 2 : 3;
                    }
                    else if (Mathf.Abs(delta.y) > Mathf.Abs(delta.x))
                    {
                        m_enemyHuntDir = delta.y > 0 ? 0 : 1;
                    }
                    
                    m_enemyDirConfirmed = true;
                    m_enemyReservedTried = false;

                    Vector2Int axis = DirectionToVector(m_enemyHuntDir);
                    m_potentialTargetCells = m_potentialTargetCells.FindAll(
                        t =>
                        {
                            Vector2Int off = t.cellPosOnGrid - m_enemyFirstHit.Value;
                            return (axis.x != 0 && off.x != 0 && off.y == 0) ||
                                   (axis.y != 0 && off.y != 0 && off.x == 0); 
                        });

                    EnemyAddPotentialTarget(FindPlayerCell(m_enemyLastHit.Value + axis));
                    EnemyAddPotentialTarget(FindPlayerCell(m_enemyFirstHit.Value - axis));

                }
            }    
        }
        private int PotentialScore(Cell c, int shipSize)
        {
            if (c == null || c.isHit)
            {
                return -1;
            }

            int score = 0;
            Vector2Int p = c.cellPosOnGrid;

            for (int i = 0; i < shipSize && i + p.y < GridManager.Ins.Col; i++)
            {
                var cc = FindPlayerCell(new Vector2Int(p.x, p.y + i));
                if (cc != null && !cc.isHit)
                {
                    score++;
                }
                else
                {
                    break;
                }
            }

            for (int i = 0; i < shipSize && i + p.x < GridManager.Ins.Row; i++)
            {
                var cc = FindPlayerCell(new Vector2Int(p.x + i, p.y));
                if (cc != null && !cc.isHit)
                {
                    score++;
                }
                else
                {
                    break;
                }
            }

            return score;
        }
        private void EnemyChoosePlayerCell()
        {
            if (IsComponentNull())
            {
                return;
            }   
            
            if (isEnemyHunting)
            {
                while (m_potentialTargetCells.Count > 0)
                {
                    Cell target = m_potentialTargetCells[0];
                    m_potentialTargetCells.RemoveAt(0);
                    if (target != null && !target.isHit)
                    {
                        m_gameMng.CheckCellIsHit(target, m_gameMng.enemyUI, out bool isHit, out bool isSunk);

                        if (isHit)
                        {
                            EnemyRigisterHit(target);

                            if (isSunk)
                            {
                                StopEnemyHunting();
                                break;
                            }    

                            if (m_enemyDirConfirmed && m_enemyLastHit.HasValue)
                            {
                                m_potentialTargetCells.Clear();
                                Vector2Int next = m_enemyLastHit.Value + DirectionToVector(m_enemyHuntDir);
                                EnemyAddPotentialTarget(FindPlayerCell(next));

                                if (m_potentialTargetCells.Count == 0 && m_enemyFirstHit.HasValue)
                                {
                                    Vector2Int rev = m_enemyFirstHit.Value - DirectionToVector(m_enemyHuntDir);
                                    EnemyAddPotentialTarget(FindPlayerCell(rev));
                                }
                            }
                        }
                        else
                        {
                            if (isEnemyHunting && m_enemyDirConfirmed)
                            {
                                if (!m_enemyReservedTried && m_enemyFirstHit.HasValue)
                                {
                                    m_enemyHuntDir = OppositeDirection(m_enemyHuntDir);
                                    m_enemyReservedTried = true;

                                    m_potentialTargetCells.Clear();
                                    Vector2Int nextTry = m_enemyFirstHit.Value + DirectionToVector(m_enemyHuntDir);
                                    EnemyAddPotentialTarget(FindPlayerCell(nextTry));
                                }
                                else
                                {
                                    m_enemyDirConfirmed = false;
                                    m_enemyReservedTried = false;
                                    m_enemyHuntDir = -1;

                                    if (m_enemyFirstHit.HasValue)
                                    {
                                        m_potentialTargetCells.Clear();
                                        int[] dir = new[] {0, 1, 2, 3};
                                        foreach (var i in dir)
                                        {
                                            EnemyAddPotentialTarget(FindPlayerCell(m_enemyFirstHit.Value
                                                + DirectionToVector(i)));
                                        }
                                    }
                                }
                            }
                        }
                        return;
                    }
                }
                StopEnemyHunting();
            }

            int shipLen = 5;
            List<Cell> candidates = new();
            List<Cell> parity = new();
            List<Cell> fallback = new();

            foreach (var ec in GridManager.Ins.CellList)
            {
                if (ec == null || ec.isHit)
                {
                    continue;
                }

                bool goodParity = ((ec.cellPosOnGrid.x + ec.cellPosOnGrid.y) % 2 == 0);
                int score = PotentialScore(ec, shipLen);
                if (score > 0)
                {
                    if (goodParity)
                    {
                        parity.Add(ec);
                    }
                    else
                    {
                        candidates.Add(ec);
                    }    
                }
                else
                {
                    fallback.Add(ec);
                }    
            }

            if (parity.Count > 0)
            {
                candidates.AddRange(parity);
            }

            if (candidates.Count == 0)
            {
                candidates = fallback;
            }

            if (candidates.Count > 0)
            {
                candidates.Sort((a, b) =>
                    PotentialScore(b, shipLen).CompareTo(PotentialScore(a, shipLen))
                );
                int take = Mathf.Min(shipLen, candidates.Count);
                Cell chosen = candidates[Random.Range(0, take)];

                m_gameMng.CheckCellIsHit(chosen, m_gameMng.enemyUI, out bool isEnemyHit, out bool isSunkShip);
                
                if (isEnemyHit)
                {
                    EnemyRigisterHit(chosen);
                    if (isSunkShip)
                    {
                        StopEnemyHunting();
                    }    
                }
                return;
            }

            foreach (var c in GridManager.Ins.CellList)
            {
                if (c != null && !c.isHit)
                {
                    m_gameMng.CheckCellIsHit(c, m_gameMng.enemyUI, out bool isEnemyHit, out bool isSunkShip);
                    
                    if (isEnemyHit)
                    {
                        EnemyRigisterHit(c);
                        if (isSunkShip)
                        {
                            StopEnemyHunting();
                        }
                    }
                    return;
                }    
            }    
        }        
    }
}

