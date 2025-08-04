using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sonn.BattleShips
{
    public class Player : MonoBehaviour, IComponentChecking
    {
        public bool isSelectedCell;

        private GameManager m_gameMng;
        private bool m_enemyCellDiscovered = false;
        private Cell m_selectEnemyCell;

        private void Awake()
        {
            m_selectEnemyCell = null;
            m_gameMng = FindObjectOfType<GameManager>();
        }
        public bool IsComponentNull()
        {
            bool check = m_gameMng == null;
            if (check)
            {
                Debug.LogWarning("Có component bị rỗng. Hãy kiểm tra lại!");
            }
            return check;
        }
        private void Start()
        {
            Init();
        }
        private void Init()
        {
            if (IsComponentNull())
            {
                return;
            }

            if (!m_enemyCellDiscovered)
            {
                GameObject[] enemyCells = GameObject.FindGameObjectsWithTag(Const.ENEMY_CELL_TAG);
                if (enemyCells.Length > 0)
                {
                    m_gameMng.enemyCells.AddRange(enemyCells);
                    m_enemyCellDiscovered = true;
                }
                Debug.Log($"Có {m_gameMng.enemyCells.Count} ô của người chơi có trong Game play!");
            }
        }    
    }
}
