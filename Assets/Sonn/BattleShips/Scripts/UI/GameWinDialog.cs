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
        public void Ok()
        {
            if (IsComponentNull())
            {
                return;
            }
            Close();
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
