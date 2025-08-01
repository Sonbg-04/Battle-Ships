using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sonn.BattleShips
{
    public class ShipManager : MonoBehaviour, IComponentChecking
    {
        public GameObject[] shipPrefabs;
        public Vector3 offsetPos;
        public Collider2D dockCollider;

        private GridManager m_gridManager;
        private float m_shipDistance = 1.5f;
        private Ship m_selectShip;
        private List<Ship> m_shipList = new();

        private void Awake()
        {
            m_gridManager = FindObjectOfType<GridManager>();
        }

        private void Start()
        {
            m_selectShip = null;
            SetShipOnScreen();
            OffsetOfShips();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                ClickSelectOnShip();
            }
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
                GameObject ship = Instantiate(shipPrefabs[i], Vector3.zero, Quaternion.identity);
                if (ship == null)
                {
                    continue;
                }
                
                Vector3 shipPos = new((i * m_shipDistance), 0, 0);
                ship.transform.position = shipPos;
                ship.transform.SetParent(transform);
                ship.name = shipPrefabs[i].name;

                Ship shipClone = ship.GetComponent<Ship>();
                if (shipClone != null)
                {
                    m_shipList.Add(shipClone);
                }
            }
            Debug.Log($"Có {m_shipList.Count} tàu đã được lưu lại!");
        }
        private void OffsetOfShips()
        {
            transform.position += offsetPos;
        }    
        private void ClickSelectOnShip()
        {
            if (IsComponentNull())
            {
                return;
            }  
            
            RaycastHit2D ray = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition),
                Vector2.zero);
            if (ray.collider == null)
            {
                return;
            }
            SelectShipFromDock(ray);
        }
        private void SelectShipFromDock(RaycastHit2D hit)
        {
            if (IsComponentNull())
            {
                return;
            }

            Ship ship = hit.collider.GetComponent<Ship>();
            if (ship == null || !dockCollider.OverlapPoint(hit.point))
            {
                return;
            }
            if (!ship.isSelectedShip)
            {
                m_selectShip = ship;
                m_selectShip.isSelectedShip = true;
                Debug.Log($"Chọn {m_selectShip.name}!");
            }
            else
            {
                Debug.Log($"{m_selectShip.name} đã được chọn. Hãy đặt vào lưới!");
                PlaceShipOnGrid(hit);
            }    

        }    
        private void PlaceShipOnGrid(RaycastHit2D hit)
        {
            
        }    
    }
}