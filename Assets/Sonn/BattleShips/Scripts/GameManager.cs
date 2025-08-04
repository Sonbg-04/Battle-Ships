using System;
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
        public List<GameObject> enemyCells;
        public GameOverDialog gameOverDialog;
        public GameWinDialog gameWinDialog;

        private Player m_player;
        private EnemyAI m_enemy;

        private void Awake()
        {
            m_player = FindObjectOfType<Player>();
            m_enemy = FindObjectOfType<EnemyAI>();
        }
        private void Start()
        {
            Init();
        }
        private void Update()
        {
            CheckTurn();
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
        private void Init()
        {
            if (IsComponentNull())
            {
                return;
            }

            
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
            yourTurnImg.gameObject.SetActive(false);
            enemyturnImg.gameObject.SetActive(true);
            if (!m_enemy.isEnemySelectedCell)
            {
                m_enemy.isEnemySelectedCell = true;
                m_enemy.isEnemyShoot = true;

            }    
        }

        private void PlayerTurn()
        {
            yourTurnImg.gameObject.SetActive(true);
            enemyturnImg.gameObject.SetActive(false);
            if (!m_player.isSelectedCell)
            {
                if (Input.GetMouseButtonDown(0))
                {

                }    
            }    
        }
        private void CheckEndGame()
        {
            if (enemyShipCount == 0)
            {
                gameWinDialog.Show(true);
            }
            else if (playerShipCount == 0)
            {
                gameOverDialog.Show(true);
            }    
        }
    }
}
