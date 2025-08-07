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
        private Cell m_selectedEnemyCell;
        private EnemyAI m_enemyAI;

        private void Awake()
        {
            m_selectedEnemyCell = null;
            m_gameMng = FindObjectOfType<GameManager>();
            m_enemyAI = FindObjectOfType<EnemyAI>();
        }
        public void PlayerFindEnemyCell()
        {
            if (IsComponentNull())
            {
                return;
            }

            if (!m_enemyCellDiscovered)
            {
                GameObject[] obj = GameObject.FindGameObjectsWithTag(Const.ENEMY_CELL_TAG);
                if (obj.Length > 0)
                {
                    m_gameMng.enemyCells.AddRange(obj);
                    m_enemyCellDiscovered = true;
                    Debug.Log($"Có {m_gameMng.enemyCells.Count} ô kẻ thù mà người chơi tìm thấy!");
                }
            }
        }
        public bool IsComponentNull()
        {
            bool check = m_gameMng == null || m_enemyAI == null;
            if (check)
            {
                Debug.LogWarning("Có component bị rỗng. Hãy kiểm tra lại!");
            }
            return check;
        }
        public void PlayerTurning()
        {
            if (!isSelectedCell && Input.GetMouseButtonDown(0))
            {
                StartCoroutine(PlayerShootCoroutine());
            }
        }
        IEnumerator PlayerShootCoroutine()
        {
            isSelectedCell = true;
            bool isKeepShooting = true, hitLastShot = false;

            while (isKeepShooting)
            {
                yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
                RaycastHit2D hit = Physics2D.Raycast(
                    Camera.main.ScreenToWorldPoint(Input.mousePosition),
                    Vector2.zero
                    );
                if (hit.collider != null && hit.collider.CompareTag(Const.ENEMY_CELL_TAG))
                {
                    var cell = hit.collider.GetComponent<Cell>();
                    if (!cell.isHit)
                    {
                        m_selectedEnemyCell = cell;
                        m_gameMng.CheckCellIsHit(m_selectedEnemyCell, m_gameMng.playerUI, out bool isShootingHit);
                        hitLastShot = isShootingHit;
                        if (!hitLastShot)
                        {
                            isKeepShooting = false;
                        }  
                    }    
                }    
            }
            m_gameMng.WaitNextTurn(2);
        }    
        public void PlayerSunkShip()
        {

        }    
    }
}
