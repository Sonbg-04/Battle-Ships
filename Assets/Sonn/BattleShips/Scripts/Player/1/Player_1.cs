using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sonn.BattleShips
{
    public class Player_1 : MonoBehaviour, IComponentChecking
    {
        public bool isSelectedPlayer2Cell, 
                    isSelectedEnemyCell;

        private GameManager m_gameMng;
        private bool m_player_2_CellDiscovered = false,
                     m_enemyCellDiscovered = false;
        private Cell m_selectedPlayer2Cell, 
                     m_selectedEnemyCell;

        private void Awake()
        {
            m_selectedPlayer2Cell = null;
            m_selectedEnemyCell = null;
            m_gameMng = FindObjectOfType<GameManager>();
        }
        public void PlayerFindEnemyCell()
        {
            if (IsComponentNull())
            {
                return;
            }

            if (Pref.currentMode == GameMode.Player_AI)
            {
                CheckFindEnemyCellWithScene(ref m_enemyCellDiscovered, Const.ENEMY_CELL_TAG);
            }
            else if (Pref.currentMode == GameMode.Player_Player)
            {
                CheckFindEnemyCellWithScene(ref m_player_2_CellDiscovered, Const.PLAYER_2_CELL_TAG);
            }

        }
        private void CheckFindEnemyCellWithScene(ref bool check, string tagCellEnemy)
        {
            if (!check)
            {
                GameObject[] obj = GameObject.FindGameObjectsWithTag(tagCellEnemy);
                if (obj.Length > 0)
                {
                    foreach (var o in obj)
                    {
                        if (!m_gameMng.enemyCells.Contains(o))
                        {
                            m_gameMng.enemyCells.Add(o);
                        }
                    }

                    check = true;

                    Debug.Log($"Có {m_gameMng.enemyCells.Count} ô của kẻ thù mà người chơi tìm thấy!");
                }
            }    
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
        public void PlayerTurning()
        {
            if (IsComponentNull())
            {
                return;
            }

            if (Pref.currentMode == GameMode.Player_Player)
            {
                if (!isSelectedPlayer2Cell)
                {
                    isSelectedPlayer2Cell = true;
                    StartCoroutine(PlayerShootCoroutine(Const.PLAYER_2_CELL_TAG, m_selectedPlayer2Cell));
                }
            }
            else if (Pref.currentMode == GameMode.Player_AI)
            {
                if (!isSelectedEnemyCell)
                {
                    isSelectedEnemyCell = true;
                    StartCoroutine(PlayerShootCoroutine(Const.ENEMY_CELL_TAG, m_selectedEnemyCell));
                }
            }
        }
        IEnumerator PlayerShootCoroutine(string tagEnemycell, Cell selectedCell)
        {
            while (true)
            {
                yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

                RaycastHit2D hit = Physics2D.Raycast(
                    Camera.main.ScreenToWorldPoint(Input.mousePosition),
                    Vector2.zero
                );

                if (hit.collider != null && hit.collider.CompareTag(tagEnemycell))
                {
                    Cell cell = hit.collider.GetComponent<Cell>();
                    if (!cell.isHit)
                    {
                        selectedCell = cell;

                        m_gameMng.CheckCellIsHit(selectedCell, m_gameMng.playerUI, out bool isHit, out _);

                        if (!isHit)
                        {
                            break;
                        }
                    }
                }
            }

            selectedCell = null;
            m_gameMng.WaitNextTurn(2);
        }
    }
}
