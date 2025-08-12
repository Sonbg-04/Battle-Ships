using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sonn.BattleShips
{
    public class GameWinDialog : Dialog, IComponentChecking
    {
        public bool IsComponentNull()
        {
            bool check = AudioManager.Ins == null;
            if (check)
            {
                Debug.LogWarning("Có component bị rỗng. Hãy kiểm tra lại!");
            }
            return check;
        }
        public override void Show(bool isShow)
        {
            if (IsComponentNull())
            {
                return;
            }
            base.Show(isShow);
            AudioManager.Ins.StopMusic(AudioManager.Ins.backgroundSource);
            AudioManager.Ins.PlaySFX(AudioManager.Ins.victorySource);
        }
        public override void Close()
        {
            base.Close();
        }
        public void Replay()
        {
            if (IsComponentNull())
            {
                return;
            }

            Close();

            AudioManager.Ins.PlaySFX(AudioManager.Ins.buttonClickSource);

            if (Pref.currentMode == GameMode.Player_Player)
            {
                SceneManager.LoadScene(Const.SET_PLACESHIP_PLAYER_1_SCENE);

                GameObject[] objs_1 = GameObject.FindGameObjectsWithTag(Const.SET_PLACESHIP_PLAYER_1_TAG);
                GameObject[] objs_2 = GameObject.FindGameObjectsWithTag(Const.SET_PLACESHIP_PLAYER_2_TAG);
                if (objs_1.Length > 0 || objs_2.Length > 0)
                {
                    foreach (var obj in objs_1)
                    {
                        Destroy(obj);
                    }
                    foreach (var obj in objs_2)
                    {
                        Destroy(obj);
                    }
                }
            }
            else if (Pref.currentMode == GameMode.Player_AI)
            {
                SceneManager.LoadScene(Const.SET_PLACESHIP_1_VS_AI_SCENE);

                GameObject[] objs = GameObject.FindGameObjectsWithTag(Const.SET_PLACESHIP_1_VS_AI_TAG);
                if (objs.Length > 0)
                {
                    foreach (var obj in objs)
                    {
                        Destroy(obj);
                    }
                }
            }
        }
        public void Ok()
        {
            if (IsComponentNull())
            {
                return;
            }

            Close();

            AudioManager.Ins.PlaySFX(AudioManager.Ins.buttonClickSource);

            SceneManager.LoadScene(Const.MAIN_MENU_SCENE);

            GameObject[] objs_3 = GameObject.FindGameObjectsWithTag(Const.MAIN_MENU_TAG);

            if (Pref.currentMode == GameMode.Player_Player)
            {
                GameObject[] objs_1 = GameObject.FindGameObjectsWithTag(Const.SET_PLACESHIP_PLAYER_1_TAG);
                GameObject[] objs_2 = GameObject.FindGameObjectsWithTag(Const.SET_PLACESHIP_PLAYER_2_TAG);
                if (objs_1.Length > 0 || objs_2.Length > 0 || objs_3.Length > 0)
                {
                    foreach (var obj in objs_1)
                    {
                        Destroy(obj);
                    }
                    foreach (var obj in objs_2)
                    {
                        Destroy(obj);
                    }
                    foreach (var obj in objs_3)
                    {
                        Destroy(obj);
                    }
                }
            }
            else if (Pref.currentMode == GameMode.Player_AI)
            {
                GameObject[] objs = GameObject.FindGameObjectsWithTag(Const.SET_PLACESHIP_1_VS_AI_TAG);
                if (objs.Length > 0 || objs_3.Length > 0)
                {
                    foreach (var obj in objs)
                    {
                        Destroy(obj);
                    }
                    foreach (var obj in objs_3)
                    {
                        Destroy(obj);
                    }
                }
            }
        }
    }
}
