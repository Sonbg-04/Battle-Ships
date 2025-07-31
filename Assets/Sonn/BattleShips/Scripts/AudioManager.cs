using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sonn.BattleShips
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Ins;

        public AudioSource backgroundSource, buttonClickSource,
            victorySource, defeatSource;

        public SettingsDialog settingsDialog; 

        private void Awake()
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

        private void Start()
        {
            backgroundSource.Play();
            settingsDialog.LoadSettings();
        }

        public void PlaySFX(AudioSource audio)
        {
            if (audio)
            {
                audio.PlayOneShot(audio.clip);
            }    
        }    
        
    }
}
