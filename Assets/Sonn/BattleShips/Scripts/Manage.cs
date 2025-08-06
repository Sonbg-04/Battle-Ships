using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Sonn.BattleShips
{
    public class Manage : MonoBehaviour, IComponentChecking
    {
        public static Manage Ins;

        public Button playGameBtn;
        
        private void Awake()
        {
            MakeSingleton(); 
        }
        public bool IsComponentNull()
        {
            bool check = AudioManager.Ins == null || ShipManager.Ins == null;
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
            SceneManager.LoadScene(Const.GAME_PLAY_SCENE);
        }
        public void Rotate()
        {
            if (IsComponentNull())
            {
                return;
            }
            AudioManager.Ins.PlaySFX(AudioManager.Ins.buttonClickSource);
            ShipManager.Ins.RotateShip();
        }
        public void Back()
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
        private void MakeSingleton()
        {
            if (Ins == null)
            {
                Ins = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
