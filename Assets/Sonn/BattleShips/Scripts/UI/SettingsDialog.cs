using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Sonn.BattleShips
{
    public class SettingsDialog : Dialog, IComponentChecking
    {
        public Slider musicSlider, sfxSlider;
        public AudioMixer audioMixer;

        public override void Show(bool isShow)
        {
            AudioManager.Ins.PlaySFX(AudioManager.Ins.buttonClickSource);
            base.Show(isShow);
            LoadSettings();
            
        }

        public override void Close()
        {
            base.Close();
        }

        public void SetMusicVolume()
        {
            if (IsComponentNull())
            {
                return;
            }
            float volume = musicSlider.value;
            audioMixer.SetFloat(Const.MUSIC_VOL_MIXER, Mathf.Log10(volume) * 20);
            Pref.MusicVolume = volume;
        }    

        public void SetSFXVolume()
        {
            if (IsComponentNull())
            {
                return;
            }
            float volume = sfxSlider.value;
            audioMixer.SetFloat(Const.SFC_VOL_MIXER, Mathf.Log10(volume) * 20);
            Pref.SfxVolume = volume;
        }  

        public void LoadSettings()
        {
            if (IsComponentNull())
            {
                return;
            }
            float musicVolume = Pref.MusicVolume;
            float sfxVolume = Pref.SfxVolume;

            musicSlider.value = musicVolume;
            sfxSlider.value = sfxVolume;

            SetMusicVolume();
            SetSFXVolume();
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
