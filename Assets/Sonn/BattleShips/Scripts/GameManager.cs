using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Sonn.BattleShips
{
    public class GameManager : MonoBehaviour
    {
        public Image playerTurnImg, enemyTurnImg;
        public int playerShipCount, enemyShipCount;
        public GameObject hitPrefab, missPrefab;
        public List<GameObject> playerUI, enemyUI, enemyCells, playerCells;
        public GameOverDialog gameOverDialog;
        public GameWinDialog gameWinDialog;

        private Player_1 m_player_1;
        private Player_2 m_player_2;
        private EnemyAI m_enemy;
        private int m_turn = 1;

        private void Awake()
        {
            m_player_1 = FindObjectOfType<Player_1>();

            Scene sc = SceneManager.GetActiveScene();
            if (sc.name == Const.GAME_PLAY_1_VS_1_SCENE)
            {
                m_player_2 = FindObjectOfType<Player_2>();
                m_enemy = null;
            }
            else if (sc.name == Const.GAME_PLAY_1_VS_AI_SCENE)
            {
                m_enemy = FindObjectOfType<EnemyAI>();
                m_player_2 = null;
            }

        }   
        private void Update()
        {
            UpdateWithSceneActive(SceneManager.GetActiveScene());
        }
        private void UpdateWithSceneActive(Scene sc)
        {
            if (sc != null)
            {
                m_player_1.PlayerFindEnemyCell(ref sc);

                if (sc.name == Const.GAME_PLAY_1_VS_1_SCENE)
                {
                    m_player_2.Player_2_FindPlayer_1_Cell();
                }
                else if (sc.name == Const.GAME_PLAY_1_VS_AI_SCENE)
                {
                    m_enemy.EnemyFindPlayerCell();
                }

                CheckTurnWithScene(ref sc);
                CheckEndGame();
            }
        }    
        private void CheckTurnWithScene(ref Scene sc)
        {
            if (sc != null)
            {
                if (m_turn == 1)
                {
                    PlayerTurn(ref sc);
                }
                else if (m_turn == 2)
                {
                    if (sc.name == Const.GAME_PLAY_1_VS_AI_SCENE)
                    {
                        EnemyTurn();
                    }
                    else if (sc.name == Const.GAME_PLAY_1_VS_1_SCENE)
                    {
                        Player_2_Turn();
                    }
                }
            }
        }
        private void Player_2_Turn()
        {
            SetUI(false);
            m_player_2.Player_2_Turning();
        }
        private void EnemyTurn()
        {
            SetUI(false);
            m_enemy.EnemyTurning();
        }
        private void PlayerTurn(ref Scene sc)
        {
            if (sc != null)
            {
                SetUI(true);
                m_player_1.PlayerTurning(ref sc);
            }
        }
        private void CheckEndGame()
        {
            if (enemyShipCount == 0)
            {
                gameWinDialog.Show(true);
                m_turn = 0;
            }
            else if (playerShipCount == 0)
            {
                gameOverDialog.Show(true);
                m_turn = 0;
            }    
        }    
        IEnumerator Wait(int number)
        {
            yield return new WaitForSeconds(1.5f);
            m_turn = number;
        }    
        public void WaitNextTurn(int num)
        {
            StartCoroutine(Wait(num));
        } 
        private void SetUI(bool isPlayerTurn)
        {
            playerTurnImg.gameObject.SetActive(isPlayerTurn);
            enemyTurnImg.gameObject.SetActive(!isPlayerTurn);   
        }    
        public void CheckCellIsHit(Cell c, List<GameObject> list, 
                                   out bool ShootIsHit, out bool isSunkShip)
        {
            ShootIsHit = false;
            isSunkShip = false;

            if (c == null || c.isHit)
            {
                return;
            }

            c.isHit = true;
            
            if (c.hasEnemyShip || c.hasPlayerOneShip || c.hasPlayerTwoShip)
            {
                var newHit = Instantiate(hitPrefab, c.transform.position, Quaternion.identity);
                
                list.Add(newHit);
                
                ShootIsHit = true;

                var part = c.shipPartTransform;
                if (part != null)
                {
                    var ship = part.GetComponentInParent<Ship>();
                    Scene sc = SceneManager.GetActiveScene();
                    if (sc != null)
                    {
                       isSunkShip = TryHandleShipSunk(ship, sc.name);
                    }
                }
                
                if (m_turn == 1)
                {
                    enemyShipCount--;
                }
                else if (m_turn == 2)
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
        private bool TryHandleShipSunk(Ship ship, string nameScene)
        {
            if (ship == null || ship.isSunkShip)
            {
                return false;
            }

            if (nameScene == Const.GAME_PLAY_1_VS_AI_SCENE)
            {
                CheckShipLayerSunk(ship, Const.PLAYER_1_SHIP_LAYER, Const.ENEMY_SHIP_LAYER);
            }
            else if (nameScene == Const.GAME_PLAY_1_VS_1_SCENE)
            {
                CheckShipLayerSunk(ship, Const.PLAYER_1_SHIP_LAYER, Const.PLAYER_2_SHIP_LAYER);
            }
            
            return true;
        }
        private void CheckShipLayerSunk(Ship s, string tagPlayer, string tagEnemy)
        {
            int shipLayer = s.gameObject.layer;

            bool isEnemyShip = shipLayer == LayerMask.NameToLayer(tagEnemy);
            bool isPlayerShip = shipLayer == LayerMask.NameToLayer(tagPlayer);

            var sourceCells = isEnemyShip ? enemyCells :
                              isPlayerShip ? playerCells : null;

            if (sourceCells == null || sourceCells.Count <= 0)
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
                if (cell != null &&
                    cell.shipPartTransform != null &&
                    cell.shipPartTransform.IsChildOf(s.transform))
                {
                    shipObjCell.Add(cell);
                }
            }

            if (shipObjCell.Count <= 0)
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

            s.isSunkShip = true;

            var shipRenderer = s.GetComponentInChildren<SpriteRenderer>();
            if (shipRenderer != null)
            {
                shipRenderer.enabled = true;
            }

            s.GetComponentInChildren<Collider2D>().enabled = true;

            s.gameObject.layer = LayerMask.NameToLayer(Const.DEAD_LAYER);

            Debug.Log($"Tàu {s.name} của phe {(isEnemyShip ? "Enemy" : "Player")} đã bị đánh chìm!");
        }
    }
}
