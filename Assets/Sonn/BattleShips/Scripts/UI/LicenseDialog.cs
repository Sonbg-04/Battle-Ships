using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sonn.BattleShips
{
    public class LicenseDialog : Dialog, IComponentChecking
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
            AudioManager.Ins.PlaySFX(AudioManager.Ins.buttonClickSource);
            base.Show(isShow);
        }

        public override void Close()
        {
            base.Close();
        }

    }
}
