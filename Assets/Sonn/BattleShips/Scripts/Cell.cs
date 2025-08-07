using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sonn.BattleShips
{
    public class Cell : MonoBehaviour
    {
        public bool hasPlayerShip, hasEnemyShip, isHit;
        public Vector2Int cellPosOnGrid;
        public Transform shipPartTransform;

    }
}
