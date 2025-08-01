using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sonn.BattleShips
{
    public class ShipManager : MonoBehaviour, IComponentChecking
    {
        public GameObject[] shipPrefabs;
        public Vector3 offsetPos;

        private GridManager m_gridManager;
        private float m_shipDistance = 1.5f;

        private void Awake()
        {
            m_gridManager = FindObjectOfType<GridManager>();
        }

        private void Start()
        {
            SetShipOnScreen();
            OffsetOfShips();
        }

        public bool IsComponentNull()
        {
            bool check = m_gridManager == null || shipPrefabs == null;
            if (check)
            {
                Debug.LogWarning("Có component bị rỗng. Hãy kiểm tra lại!");
            }
            return check;
        }

        private void SetShipOnScreen()
        {
            if (IsComponentNull() || shipPrefabs.Length <= 0)
            {
                return;
            }
            for (int i = 0; i < shipPrefabs.Length; i++)
            {
                var ship = Instantiate(shipPrefabs[i], Vector3.zero, Quaternion.identity);
                if (ship == null)
                {
                    continue;
                }
                Vector3 shipPos = new(
                            offsetPos.x + (i * m_shipDistance),
                            offsetPos.y,
                            offsetPos.z
                            );

                ship.transform.position = shipPos;
                ship.transform.SetParent(transform);
                ship.name = shipPrefabs[i].name;

            }    
        }
        
        private void OffsetOfShips()
        {
            transform.position += offsetPos;
        }    

    }
}