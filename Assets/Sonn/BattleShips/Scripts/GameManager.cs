using System;
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
        public List<GameObject> enemyCells, playerUI, enemyUI;
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
            m_player.PlayerChooseCell();
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
            else
            {
                c.isHit = true;
                if (c.hasShip)
                {
                    var newHit = Instantiate(hitPrefab, c.transform.position, Quaternion.identity);
                    list.Add(newHit);
                    ShootIsHit = true;
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

        }    

    }
}
