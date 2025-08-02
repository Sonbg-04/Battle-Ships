using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sonn.BattleShips
{
    public static class Pref
    {
        public static void SetBool(string key, bool value)
        {
            if (value)
            {
                PlayerPrefs.SetInt(key, 1);
            }
            else
            {
                PlayerPrefs.SetInt(key, 0);
            }
        }
        public static bool GetBool(string key)
        {
            int check = PlayerPrefs.GetInt(key);
            if (check == 0)
            {
                return false;
            }
            else if (check == 1)
            {
                return true;
            }
            return false;
        }
        public static float MusicVolume
        {
            get => PlayerPrefs.GetFloat(Const.MUSIC_VOLUME, 0.3f);
            set => PlayerPrefs.SetFloat(Const.MUSIC_VOLUME, value);
        }
        public static float SfxVolume
        {
            get => PlayerPrefs.GetFloat(Const.SFX_VOlUME, 0.5f);
            set => PlayerPrefs.SetFloat(Const.SFX_VOlUME, value);
        }
    }
}
