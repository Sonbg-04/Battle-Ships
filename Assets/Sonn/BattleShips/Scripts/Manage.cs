using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sonn.BattleShips
{
    public class Manage : MonoBehaviour, IComponentChecking
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

        public void PlayGame()
        {
            if (IsComponentNull())
            {
                return;
            }
            AudioManager.Ins.PlaySFX(AudioManager.Ins.buttonClickSource);
        }    

        public void RotateShip()
        {
            if (IsComponentNull())
            {
                return;
            }
            AudioManager.Ins.PlaySFX(AudioManager.Ins.buttonClickSource);
        }   
        
        public void Back()
        {
            if (IsComponentNull())
            {
                return;
            }
            AudioManager.Ins.PlaySFX(AudioManager.Ins.buttonClickSource);
            SceneManager.LoadScene(Const.MAIN_MENU_SCENE);
        }    
    }
}
