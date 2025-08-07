using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sonn.BattleShips
{
    public class GameManager : MonoBehaviour, IComponentChecking
    {
        public Image yourTurnImg, enemyturnImg;
        public int turn = 1, playerShipCount, enemyShipCount;
        public GameObject hitPrefab, missPrefab;
        public List<GameObject> playerUI, enemyUI, enemyCells, playerCells;
        public GameOverDialog gameOverDialog;
        public GameWinDialog gameWinDialog;

        private Player m_player;
        private EnemyAI m_enemy;

        private void Awake()
        {
            m_player = FindObjectOfType<Player>();
            m_enemy = FindObjectOfType<EnemyAI>();
        }   
        private void Update()
        {
            if (IsComponentNull())
            {
                return;
            }
            m_player.PlayerFindEnemyCell();
            m_enemy.EnemyFindPlayerCell();
            CheckTurn();
            CheckEndGame();
        }
        public bool IsComponentNull()
        {
            bool check = m_player == null || m_enemy == null;
            if (check)
            {
                Debug.LogWarning("Có component bị rỗng. Hãy kiểm tra lại!");
            }
            return check;
        }
        private void CheckTurn()
        {
            if (turn == 1)
            {
                PlayerTurn();

            }
            else if (turn == 2 && !m_enemy.isEnemyShoot)
            {
                EnemyTurn();
            }    
        }
        private void EnemyTurn()
        {
            SetUI(false);
            m_enemy.EnemyTurning();
        }
        private void PlayerTurn()
        {
            SetUI(true);
            m_player.PlayerTurning();
        }
        private void CheckEndGame()
        {
            if (enemyShipCount == 0)
            {
                gameWinDialog.Show(true);
                turn = 0;
            }
            else if (playerShipCount == 0)
            {
                gameOverDialog.Show(true);
                turn = 0;
            }    
        }
        IEnumerator Wait(int number)
        {
            yield return new WaitForSeconds(1.5f);
            turn = number;
            m_player.isSelectedCell = false;
            m_enemy.isEnemySelectedCell = false;
        }    
        public void WaitNextTurn(int num)
        {
            StartCoroutine(Wait(num));
        } 
        private void SetUI(bool isPlayerTurn)
        {
            yourTurnImg.gameObject.SetActive(isPlayerTurn);
            enemyturnImg.gameObject.SetActive(!isPlayerTurn);
        }    
        public void CheckCellIsHit(Cell c, List<GameObject> list, out bool ShootIsHit)
        {
            ShootIsHit = false;

            if (c == null || c.isHit)
            {
                return;
            }

            c.isHit = true;
            
            if (c.hasEnemyShip || c.hasPlayerShip)
            {
                var newHit = Instantiate(hitPrefab, c.transform.position, Quaternion.identity);
                
                list.Add(newHit);
                
                ShootIsHit = true;

                var part = c.shipPartTransform;
                if (part != null)
                {
                    var ship = part.GetComponentInParent<Ship>();
                    TryHandleShipSunk(ship);
                }
                
                if (turn == 1)
                {
                    enemyShipCount--;
                }
                else if (turn == 2)
                {
                    playerShipCount--;
                }
            }
            else
            {
                var newMiss = Instantiate(missPrefab, c.transform.position, Quaternion.identity);
                
                list.Add(newMiss);
            }
        }
        private void TryHandleShipSunk(Ship ship)
        {
            if (ship == null || ship.isSunkShip)
            {
                return;
            }

            int shipLayer = ship.gameObject.layer;

            bool isEnemyShip = shipLayer == LayerMask.NameToLayer(Const.SHIP_ENEMY_LAYER);
            bool isPlayerShip = shipLayer == LayerMask.NameToLayer(Const.SHIP_PLAYER_LAYER);

            var sourceCells = isEnemyShip ? enemyCells :
                              isPlayerShip ? playerCells : null;
            
            if (sourceCells == null)
            {
                return;
            }

            List<Cell> shipObjCell = new();

            foreach (var cellObj in sourceCells)
            {
                if (cellObj == null)
                {
                    continue;
                }

                var cell = cellObj.GetComponent<Cell>();
                if (cell == null)
                {
                    continue;
                }

                if (cell.shipPartTransform != null && cell.shipPartTransform.IsChildOf(ship.transform))
                {
                    shipObjCell.Add(cell);
                }
            }

            if (shipObjCell.Count == 0)
            {
                return;
            }

            foreach (var c in shipObjCell)
            {
                if (c != null && !c.isHit)
                {
                    return;
                }
            }

            ship.isSunkShip = true;
            ship.GetComponentInChildren<SpriteRenderer>().enabled = true;
            
            Debug.Log($"Tàu {ship.name} của phe {(isEnemyShip ? "Enemy" : "Player")} đã bị đánh chìm!");
        }
    }
}
