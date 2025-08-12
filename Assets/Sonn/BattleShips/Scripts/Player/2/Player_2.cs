using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sonn.BattleShips
{
    public class Player_2 : MonoBehaviour, IComponentChecking
    {
        public bool isSelectedPlayer1Cell;

        private GameManager m_gameMng;
        private bool m_player_1_CellDiscovered = false;
        private Cell m_selectedPlayer1Cell;

        private void Awake()
        {
            m_selectedPlayer1Cell = null;
            m_gameMng = FindObjectOfType<GameManager>();
        }
        public void Player_2_FindPlayer_1_Cell()
        {
            if (IsComponentNull())
            {
                return;
            }
            if (!m_player_1_CellDiscovered)
            {
                GameObject[] obj = GameObject.FindGameObjectsWithTag(Const.PLAYER_1_CELL_TAG);
                if (obj.Length > 0)
                {
                    foreach (var o in obj)
                    {
                        if (!m_gameMng.playerCells.Contains(o))
                        {
                            m_gameMng.playerCells.Add(o);
                        }
                    }

                    m_player_1_CellDiscovered = true;
                    Debug.Log($"Có {m_gameMng.playerCells.Count} ô của người chơi thứ 1 mà người chơi thứ 2 tìm thấy!");
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
        public void Player_2_Turning()
        {
            if (!isSelectedPlayer1Cell)
            {
                isSelectedPlayer1Cell = true;
                StartCoroutine(Player_2_ShootCoroutine(Const.PLAYER_1_CELL_TAG, m_selectedPlayer1Cell));
            }
        }
        IEnumerator Player_2_ShootCoroutine(string tagEnemycell, Cell selectedCell)
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
            m_gameMng.WaitNextTurn(1);
        }
    }
}
