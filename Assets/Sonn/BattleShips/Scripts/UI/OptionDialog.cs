using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sonn.BattleShips
{
    public class OptionDialog : Dialog, IComponentChecking
    {
        public override void Show(bool isShow)
        {
            base.Show(isShow);
        }
        public override void Close()
        {
            base.Close();
        }
        public void SoloPlayerEvent()
        {
            if (IsComponentNull())
            {
                return;
            }    

            AudioManager.Ins.PlaySFX(AudioManager.Ins.buttonClickSource);
            Pref.currentMode = GameMode.Player_Player;
            SceneManager.LoadScene(Const.SET_PLACESHIP_PLAYER_1_SCENE);
        }
        public void SoloAIEvent()
        {
            if (IsComponentNull())
            {
                return;
            }

            AudioManager.Ins.PlaySFX(AudioManager.Ins.buttonClickSource);
            Pref.currentMode = GameMode.Player_AI;
            SceneManager.LoadScene(Const.SET_PLACESHIP_PLAYER_1_SCENE);
        }
        public bool IsComponentNull()
        {
            bool check = AudioManager.Ins == null;
            if (check)
            {
                Debug.LogWarning("Có component bị rỗng. Hãy kiểm tra lại!");
            }
            return check;
        }
    }
}
