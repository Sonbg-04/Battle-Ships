using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sonn.BattleShips
{
    public class Dialog : MonoBehaviour
    {
        public virtual void Show(bool isShow)
        {
            gameObject.SetActive(isShow);
        }   
        public virtual void Close()
        {
            gameObject.SetActive(false);
        }    
        public virtual void UpdateDialog() {}
        
    }
}
