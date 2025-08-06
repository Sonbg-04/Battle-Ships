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
        public bool isEnemyShoot, isEnemySelectedCell;

        private Cell[, ] m_enemyCell;
        private List<Cell> m_enemyCellList, m_occupiedEnemyCells;
        private GameManager m_gameMng;

        private void Awake()
        {
            m_enemyCell = new Cell[GridManager.Ins.Row, GridManager.Ins.Col];
            m_enemyCellList = new();    
            m_gameMng = FindObjectOfType<GameManager>();
            m_occupiedEnemyCells = new();
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
                    enemyCell.name = $"EnemyCell[{x}][{y}]";

                    var c = enemyCell.GetComponent<Cell>();
                    if (c == null)
                    {
                        return;
                    }
                    m_enemyCell[x, y] = c;
                    m_enemyCell[x, y].cellPosOnGrid = new Vector2(
                        Mathf.RoundToInt(m_enemyCell[x, y].transform.position.x / -GridManager.Ins.CellDistance),
                        Mathf.RoundToInt(m_enemyCell[x, y].transform.position.y / -GridManager.Ins.CellDistance)
                        );

                    m_enemyCellList.Add(m_enemyCell[x, y]);
                }
            }
            Debug.Log($"Có {m_enemyCellList.Count} ô của kẻ thù được lưu vào Game Play!");
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
        private bool CheckEnemyShipTogether(int x, int y)
        {
            Vector2Int[] OrthogonalDirs =
            {
                Vector2Int.up,
                Vector2Int.down,
                Vector2Int.left,
                Vector2Int.right
            };

            Vector2Int pos = new(x, y);
            foreach (var dir in OrthogonalDirs)
            {
                Vector2Int neighbor = dir + pos;
                if (!IsInsideEnemyGrid(neighbor.x, neighbor.y))
                {
                    continue;
                }    
                if (m_enemyCell[neighbor.x, neighbor.y].hasShip)
                {
                    return true;
                }    
            }
            return false;
        }
        private bool CanEnemyShipPlace(int startX, int startY, bool isVertical,
            int shipEnemySize, List<Cell> outEnemyCells)
        {
            outEnemyCells.Clear();

            for (int i = 0; i < shipEnemySize; i++)
            {
                int x = isVertical ? startX : (startX + i);
                int y = isVertical ? (startY + i) : startY;

                if (!IsInsideEnemyGrid(x, y))
                {
                    return false;
                }    
                
                var enemyCell = m_enemyCell[x, y];
                if (enemyCell.hasShip)
                {
                    return false;
                }
                
                if (CheckEnemyShipTogether(x, y))
                {
                    return false;
                }   

                outEnemyCells.Add(enemyCell);

            }
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
                SetLayerEnemy(newEnemyShip, Const.SHIP_ENEMY_LAYER);
                newEnemyShip.SetActive(false);

                Vector3 centerPos = Vector3.zero;
                foreach (var item in m_occupiedEnemyCells)
                {
                    centerPos += item.transform.position;
                }
                centerPos /= m_occupiedEnemyCells.Count;

                if (newEnemyShip.CompareTag(Const.SHIP_CELL_2_TAG))
                {
                    newEnemyShip.transform.SetPositionAndRotation(
                                    centerPos + new Vector3(),
                                    vertical ? Quaternion.identity
                                             : Quaternion.Euler(0, 0, -90f)
                                 );
                }
                else if (newEnemyShip.CompareTag(Const.SHIP_CELL_3_TAG))
                {
                    newEnemyShip.transform.SetPositionAndRotation(
                                    centerPos + new Vector3(),
                                    vertical ? Quaternion.identity
                                             : Quaternion.Euler(0, 0, -90f)
                                 );
                }
                else if (newEnemyShip.CompareTag(Const.SHIP_CELL_4_TAG))
                {
                    newEnemyShip.transform.SetPositionAndRotation(
                                    centerPos + new Vector3(),
                                    vertical ? Quaternion.identity
                                             : Quaternion.Euler(0, 0, -90f)
                                 );
                }
                else
                {
                    newEnemyShip.transform.SetPositionAndRotation(
                                    centerPos + new Vector3(),
                                    vertical ? Quaternion.identity
                                             : Quaternion.Euler(0, 0, -90f)
                                 );
                }   

                var s = newEnemyShip.GetComponent<Ship>();
                if (s != null)
                {
                    s.isRotatedShip = !vertical;
                    s.isPlacedShip = true;
                }

                for (int i = 0; i < shipSize; i++)
                {
                    var part = newEnemyShip.transform.GetChild(i);
                    part.SetParent(newEnemyShip.transform);
                    
                    if (part.CompareTag(Const.SHIP_PART_2_TAG))
                    {
                        if (part.name == Const.SHIP_PART_2_1_NAME)
                        {
                            part.localPosition += new Vector3(0, -0.17f, 0);
                        }
                        else if (part.name == Const.SHIP_PART_2_2_NAME)
                        {
                            part.localPosition += new Vector3(0, -0.02f, 0);
                        }
                    }
                    else if (part.CompareTag(Const.SHIP_PART_3_TAG))
                    {
                        if (part.name == Const.SHIP_PART_3_1_NAME)
                        {
                            part.localPosition += new Vector3(0, -0.24f, 0);
                        }
                        else if (part.name == Const.SHIP_PART_3_2_NAME)
                        {
                            part.localPosition += new Vector3(0, -0.1f, 0);
                        }
                        else
                        {
                            part.localPosition += new Vector3(0, -0.02f, 0);
                        }    
                    }
                    else if (part.CompareTag(Const.SHIP_PART_4_TAG))
                    {
                        if (part.name == Const.SHIP_PART_4_1_NAME)
                        {
                            part.localPosition += new Vector3(0, -0.39f, 0);
                        }
                        else if (part.name == Const.SHIP_PART_4_2_NAME)
                        {
                            part.localPosition += new Vector3(0, -0.25f, 0);
                        }
                        else if (part.name == Const.SHIP_PART_4_3_NAME)
                        {
                            part.localPosition += new Vector3(0, -0.13f, 0);
                        }
                        else
                        {
                            part.localPosition += Vector3.zero;
                        }    
                    }
                    else if (part.CompareTag(Const.SHIP_PART_5_TAG))
                    {
                        if (part.name == Const.SHIP_PART_5_1_NAME)
                        {
                            part.localPosition += new Vector3(0, -0.48f, 0);
                        }
                        else if (part.name == Const.SHIP_PART_5_2_NAME)
                        {
                            part.localPosition += new Vector3(0, -0.39f, 0);
                        }
                        else if (part.name == Const.SHIP_PART_5_3_NAME)
                        {
                            part.localPosition += new Vector3(0, -0.25f, 0);
                        }
                        else if (part.name == Const.SHIP_PART_5_4_NAME)
                        {
                            part.localPosition += new Vector3(0, -0.11f, 0);
                        }
                        else
                        {
                            part.localPosition += new Vector3(0, -0.02f, 0);
                        }    

                    }

                    var cell = m_occupiedEnemyCells[i];
                    cell.hasShip = true;
                    cell.shipPartTransform = part;
                    cell.GetComponent<SpriteRenderer>().color = Color.blue;
                }

                isShipEnemyPlaced = true;
            }
        }
        private void SetLayerEnemy(GameObject go, int layer)
        {
            go.layer = layer;
            foreach (Transform t in go.transform)
                SetLayerEnemy(t.gameObject, layer);
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

            }
            isEnemyShoot = false;
            m_gameMng.WaitNextTurn(1);
        }

    }
}

