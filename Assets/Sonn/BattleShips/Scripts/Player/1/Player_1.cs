using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sonn.BattleShips
{
    public class Player_1 : MonoBehaviour, IComponentChecking
    {
        public bool isSelectedPlayer2Cell, isSelectedEnemyCell;

        private GameManager m_gameMng;
        private bool m_player_2_CellDiscovered = false,
                     m_enemyCellDiscovered = false;
        private Cell m_selectedPlayer2Cell, m_selectedEnemyCell;

        private void Awake()
        {
            m_selectedPlayer2Cell = null;
            m_selectedEnemyCell = null;
            m_gameMng = FindObjectOfType<GameManager>();
        }
        public void PlayerFindEnemyCell(ref Scene sc)
        {
            if (IsComponentNull() || !sc.IsValid())
            {
                return;
            }
            
            if (sc.name == Const.GAME_PLAY_1_VS_AI_SCENE)
            {
                if (!m_enemyCellDiscovered)
                {
                    GameObject[] obj = GameObject.FindGameObjectsWithTag(Const.ENEMY_CELL_TAG);
                    if (obj.Length > 0)
                    {
                        foreach (var o in obj)
                        {
                            if (!m_gameMng.enemyCells.Contains(o))
                            {
                                m_gameMng.enemyCells.Add(o);
                            }
                        }
                        m_enemyCellDiscovered = true;
                        Debug.Log($"Có {m_gameMng.enemyCells.Count} ô của kẻ thù mà người chơi thứ 1 tìm thấy!");
                    }
                }
            }
            else if (sc.name == Const.GAME_PLAY_1_VS_1_SCENE)
            {
                if (!m_player_2_CellDiscovered)
                {
                    GameObject[] obj = GameObject.FindGameObjectsWithTag(Const.PLAYER_2_CELL_TAG);
                    if (obj.Length > 0)
                    {
                        foreach (var o in obj)
                        {
                            if (!m_gameMng.enemyCells.Contains(o))
                            {
                                m_gameMng.enemyCells.Add(o);
                            }
                        }
                        m_player_2_CellDiscovered = true;
                        Debug.Log($"Có {m_gameMng.enemyCells.Count} ô của người chơi thứ 2 mà người chơi thứ 1 tìm thấy!");
                    }
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
        public void PlayerTurning(ref Scene sc)
        {
            if (IsComponentNull() || !sc.IsValid())
            {
                return;
            }    

            if (Input.GetMouseButtonDown(0))
            {
                if (sc.name == Const.GAME_PLAY_1_VS_AI_SCENE)
                {
                    if (!isSelectedEnemyCell)
                    {
                        isSelectedEnemyCell = true;
                        StartCoroutine(PlayerShootCoroutine(Const.GAME_PLAY_1_VS_AI_SCENE));
                    }
                }
                else if (sc.name == Const.GAME_PLAY_1_VS_1_SCENE)
                {
                    if (!isSelectedPlayer2Cell)
                    {
                        isSelectedPlayer2Cell = true;
                        StartCoroutine(PlayerShootCoroutine(Const.GAME_PLAY_1_VS_1_SCENE));
                    }
                }
            }
        }   
        IEnumerator PlayerShootCoroutine(string nameScene)
        {
            bool isKeepShooting = true, hitLastShot = false;

            while (isKeepShooting)
            {
                yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

                RaycastHit2D hit = Physics2D.Raycast(
                    Camera.main.ScreenToWorldPoint(Input.mousePosition),
                    Vector2.zero
                    );
                
                if (nameScene == Const.GAME_PLAY_1_VS_AI_SCENE)
                {
                    CheckShootScenePlayer(ref m_selectedEnemyCell, hit,
                    Const.ENEMY_CELL_TAG, ref hitLastShot, ref isKeepShooting);
                }
                else if (nameScene == Const.GAME_PLAY_1_VS_1_SCENE)
                {
                    CheckShootScenePlayer(ref m_selectedPlayer2Cell, hit,
                    Const.PLAYER_2_CELL_TAG, ref hitLastShot, ref isKeepShooting);
                }

            }

            m_gameMng.WaitNextTurn(2);
            ResetWithScene(nameScene);
        }
        private void ResetWithScene(string nameScene)
        {
            if (nameScene == Const.GAME_PLAY_1_VS_AI_SCENE)
            {
                m_selectedEnemyCell = null;
                isSelectedEnemyCell = false;
            }
            else if (nameScene == Const.GAME_PLAY_1_VS_1_SCENE)
            {
                m_selectedPlayer2Cell = null;
                isSelectedPlayer2Cell = false;
            }    
        }    
        private void CheckShootScenePlayer(ref Cell selectCell, RaycastHit2D hit, string tag, 
                                           ref bool lasthit, ref bool keep)
        {
            if (hit.collider != null && hit.collider.CompareTag(tag))
            {
                var cell = hit.collider.GetComponent<Cell>();
                if (cell != null && !cell.isHit)
                {
                    selectCell = cell;
                    m_gameMng.CheckCellIsHit(selectCell, m_gameMng.playerUI, out bool isShootingHit, out _);
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
