using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Sonn.BattleShips
{
    public class PauseDialog : Dialog, IComponentChecking
    {
        public Button pauseBtn;
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
            AudioManager.Ins.PlaySFX(AudioManager.Ins.buttonClickSource);
            AudioManager.Ins.PauseMusic(AudioManager.Ins.backgroundSource);
            Time.timeScale = 0;
            pauseBtn.gameObject.SetActive(false);
        }
        public override void Close()
        {
            if (IsComponentNull())
            {
                return;
            }
            Time.timeScale = 1f;
            base.Close();
            AudioManager.Ins.PlaySFX(AudioManager.Ins.buttonClickSource);
            AudioManager.Ins.ResumeMusic(AudioManager.Ins.backgroundSource);
            pauseBtn.gameObject.SetActive(true);
        }
        public void Replay()
        {
            if (IsComponentNull())
            {
                return;
            }
            Close();
            AudioManager.Ins.PlaySFX(AudioManager.Ins.buttonClickSource);

            Scene sc = SceneManager.GetActiveScene();
            if (sc.name == Const.GAME_PLAY_1_VS_1_SCENE)
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
            else if (sc.name == Const.GAME_PLAY_1_VS_AI_SCENE)
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
            Scene sc = SceneManager.GetActiveScene();
            if (sc.name == Const.GAME_PLAY_1_VS_1_SCENE)
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
            else if (sc.name == Const.GAME_PLAY_1_VS_AI_SCENE)
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
