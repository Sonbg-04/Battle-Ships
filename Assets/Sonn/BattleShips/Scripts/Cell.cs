using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sonn.BattleShips
{
    public class Cell : MonoBehaviour
    {
        public bool hasShip, isHit;
        public int countHit;
        public Vector2 cellPosOnGrid;

        private Ship m_occupiedShip;

        public Ship OccupiedShip { set => m_occupiedShip = value; }

    }
}
