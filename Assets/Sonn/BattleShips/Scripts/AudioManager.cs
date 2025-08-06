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
        }

        public void PlaySFX(AudioSource audio)
        {
            if (audio)
            {
                audio.PlayOneShot(audio.clip);
            }    
        }
        public void StopMusic(AudioSource audio)
        {
            if (audio)
            {
                audio.Stop();
            }
        }
        public void PauseMusic(AudioSource audio)
        {
            if (audio && audio.isPlaying)
            {
                audio.Pause();
            }
        }
        public void ResumeMusic(AudioSource audio)
        {
            if (audio && !audio.isPlaying)
            {
                audio.UnPause();
            }
        }
    }
}
