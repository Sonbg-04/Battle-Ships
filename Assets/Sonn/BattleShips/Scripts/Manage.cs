using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Sonn.BattleShips
{
    public class Manage : MonoBehaviour, IComponentChecking
    {
        public Button playGameBtn;

        private ShipManager m_shipMng;
        private void Awake()
        {
            m_shipMng = FindObjectOfType<ShipManager>();
        }
        public bool IsComponentNull()
        {
            bool check = AudioManager.Ins == null || m_shipMng == null;
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
        public void Rotate()
        {
            if (IsComponentNull())
            {
                return;
            }
            AudioManager.Ins.PlaySFX(AudioManager.Ins.buttonClickSource);
            m_shipMng.RotateShip();
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
