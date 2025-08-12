using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Sonn.BattleShips
{
    public class Manage : MonoBehaviour
    {
        public Button btnNext, btnPlay;

        private static Dictionary<Type, MonoBehaviour> m_ins;
        
        public static T GetIns<T>() where T : MonoBehaviour
        {
            if (m_ins.TryGetValue(typeof(T), out var ins))
            {
                return ins as T;
            }    
            return null;
        }    

        private void Awake()
        {
            m_ins = new();
            MakeSingleton();
        }
        public void ShowBtnNextOrPlay(string nameScene)
        {
            if (Pref.currentMode == GameMode.Player_AI)
            {
                btnPlay.gameObject.SetActive(true);
            }
            else if (Pref.currentMode == GameMode.Player_Player)
            {
                if (nameScene == Const.SET_PLACESHIP_PLAYER_1_SCENE)
                {
                    btnNext.gameObject.SetActive(true);
                }
                else if (nameScene == Const.SET_PLACESHIP_PLAYER_2_SCENE)
                {
                    btnPlay.gameObject.SetActive(true);
                }
            }
        }    
        public void NextScene()
        {
            AudioManager.Ins.PlaySFX(AudioManager.Ins.buttonClickSource);

            if (Pref.currentMode == GameMode.Player_Player)
            {
                SceneManager.LoadScene(Const.SET_PLACESHIP_PLAYER_2_SCENE);
            }
        }    
        public void PlayGame()
        {
            AudioManager.Ins.PlaySFX(AudioManager.Ins.buttonClickSource);

            if (Pref.currentMode == GameMode.Player_AI)
            {
                SceneManager.LoadScene(Const.GAME_PLAY_1_VS_AI_SCENE);
            }
            else if (Pref.currentMode == GameMode.Player_Player)
            {
                SceneManager.LoadScene(Const.GAME_PLAY_1_VS_1_SCENE);
            }
        }
        public void Rotate()
        {
            AudioManager.Ins.PlaySFX(AudioManager.Ins.buttonClickSource);

            ShipManager.GetInstance<ShipManager>().RotateShip();
        }
        public void Back()
        {
            AudioManager.Ins.PlaySFX(AudioManager.Ins.buttonClickSource);

            SceneManager.LoadScene(Const.MAIN_MENU_SCENE);

            GameObject[] obj_1 = GameObject.FindGameObjectsWithTag(Const.SET_PLACESHIP_PLAYER_1_TAG);
            GameObject[] obj_2 = GameObject.FindGameObjectsWithTag(Const.SET_PLACESHIP_PLAYER_2_TAG);
            if (obj_1.Length > 0 || obj_2.Length > 0)
            {
                foreach (var obj in obj_1)
                {
                    Destroy(obj);
                }

                foreach (var obj in obj_2)
                {
                    Destroy(obj);
                }
            }
        }
        private void MakeSingleton()
        {
            var key = GetType();
            if (!m_ins.ContainsKey(key) || m_ins[key] == null)
            {
                m_ins[key] = this;
                DontDestroyOnLoad(this);
                SceneManager.sceneLoaded += OnSceneLoaded;
            }
            else
            {
                Destroy(gameObject);
            }    
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            
            var key = GetType();
            if (m_ins.ContainsKey(key) && m_ins[key] != null)
            {
                m_ins.Remove(key);
            }    
        }

        private void OnSceneLoaded(Scene sc, LoadSceneMode mode)
        {
            string sceneName = sc.name;
            bool checkTag = gameObject.CompareTag(Const.SET_PLACESHIP_PLAYER_1_TAG);

            if (sceneName == Const.SET_PLACESHIP_PLAYER_2_SCENE)
            {
                if (checkTag)
                {
                    gameObject.SetActive(false);
                }
            }
            else if (sceneName == Const.GAME_PLAY_1_VS_1_SCENE)
            {
                if (checkTag)
                {
                    gameObject.SetActive(true);
                }
            }
            
            if (sceneName == Const.MAIN_MENU_SCENE ||
                sceneName == Const.GAME_PLAY_1_VS_1_SCENE ||
                sceneName == Const.GAME_PLAY_1_VS_AI_SCENE)
            {
                Destroy(gameObject);
            }
        }
    }
}
