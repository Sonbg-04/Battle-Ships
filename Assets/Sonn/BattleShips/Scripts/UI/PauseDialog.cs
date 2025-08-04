using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sonn.BattleShips
{
    public class PauseDialog : Dialog, IComponentChecking
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
            AudioManager.Ins.PlaySFX(AudioManager.Ins.buttonClickSource);
            AudioManager.Ins.StopMusic(AudioManager.Ins.backgroundSource);
            Time.timeScale = 0;
        }
        public override void Close()
        {
            base.Close();
            Time.timeScale = 1f;
            AudioManager.Ins.PlaySFX(AudioManager.Ins.buttonClickSource);
            AudioManager.Ins.ResumeMusic(AudioManager.Ins.backgroundSource);
        }
        public void Replay()
        {
            if (IsComponentNull())
            {
                return;
            }
            Close();
            AudioManager.Ins.PlaySFX(AudioManager.Ins.buttonClickSource);
            SceneManager.LoadScene(Const.SET_PLACESHIPS_SCENE);
            GameObject[] objs = FindObjectsOfType<GameObject>();
            foreach (var obj in objs)
            {
                if (obj != null)
                {
                    if (obj.CompareTag(Const.SET_PLACESHIPS_TAG))
                    {
                        Destroy(obj);
                    }
                }
            }
        }
        public void BackToMenu()
        {
            if (IsComponentNull())
            {
                return;
            }
            AudioManager.Ins.PlaySFX(AudioManager.Ins.buttonClickSource);
            SceneManager.LoadScene(Const.MAIN_MENU_SCENE);
            GameObject[] objs = FindObjectsOfType<GameObject>();
            foreach (var obj in objs)
            {
                if (obj != null)
                {
                    if (obj.CompareTag(Const.SET_PLACESHIPS_TAG))
                    {
                        Destroy(obj);
                    }
                }
            }
        }
    }
}
