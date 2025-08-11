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
            if (!isSelectedPlayer1Cell && Input.GetMouseButtonDown(0))
            {
                isSelectedPlayer1Cell = true;
                StartCoroutine(Player_2_ShootCoroutine());
            }
        }
        IEnumerator Player_2_ShootCoroutine()
        {   
            bool isKeepShooting = true, 
                 hitLastShot = false;

            while (isKeepShooting)
            {
                yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

                RaycastHit2D hit = Physics2D.Raycast(
                    Camera.main.ScreenToWorldPoint(Input.mousePosition),
                    Vector2.zero
                    );
                
                CheckShootScenePlayer(hit, Const.PLAYER_1_CELL_TAG, ref hitLastShot, ref isKeepShooting);
                
            }

            m_gameMng.WaitNextTurn(1);

            m_selectedPlayer1Cell = null;
            isSelectedPlayer1Cell = false;
        }
        private void CheckShootScenePlayer(RaycastHit2D hit, string tag, ref bool lasthit, ref bool keep)
        {
            if (hit.collider != null && hit.collider.CompareTag(tag))
            {
                var cell = hit.collider.GetComponent<Cell>();
                if (cell != null && !cell.isHit)
                {
                    m_selectedPlayer1Cell = cell;
                    m_gameMng.CheckCellIsHit(m_selectedPlayer1Cell, m_gameMng.enemyUI, 
                                             out bool isShootingHit, out _);
                    lasthit = isShootingHit;
                    if (!lasthit)
                    {
                        keep = false;
                    }
                }
            }
        }
    }
}
