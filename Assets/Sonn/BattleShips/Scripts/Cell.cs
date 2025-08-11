using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sonn.BattleShips
{
    public class Cell : MonoBehaviour
    {
        public bool hasPlayerOneShip, hasPlayerTwoShip, hasEnemyShip, isHit;
        public Vector2Int cellPosOnGrid;
        public Transform shipPartTransform;

    }
}
