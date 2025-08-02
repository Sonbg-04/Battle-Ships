using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sonn.BattleShips
{
    public class ShipManager : MonoBehaviour, IComponentChecking
    {
        public GameObject[] shipPrefabs;
        public Vector3 offsetPos;
        public int shipCount;

        private float m_shipDistance = 1.5f;
        private Ship m_selectedShip;
        private List<Ship> m_shipList;
        private Vector3 m_chosenPos;
        private GridManager m_gridMng;
        private Manage m_manage;

        private void Awake()
        {
            m_selectedShip = null;
            m_chosenPos = Vector3.zero;
            m_gridMng = FindObjectOfType<GridManager>();
            m_shipList = new();
            m_manage = FindObjectOfType<Manage>();
        }
        private void Start()
        {
            SetShipOnScreen();
            OffsetOfShips();
            shipCount = m_shipList.Count;
        }
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                HandleMouseClick();
            }
        }
        private void SetShipOnScreen()
        {
            if (shipPrefabs.Length <= 0 || IsComponentNull())
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
        private void HandleMouseClick()
        {
            if (IsComponentNull())
            {
                return;
            }

            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

            if (hit.collider == null)
            {
                return;
            }

            if (hit.collider.CompareTag(Const.PLAYER_CELL_TAG))
            {
                PlaceShipOnGrid(hit);
            }
            else if (hit.collider.GetComponent<Ship>() != null)
            {
                SelectShip(hit);
            }
        }
        private void SelectShip(RaycastHit2D hit)
        {
            Ship clickedShip = hit.collider.GetComponent<Ship>();
            if (clickedShip == null || IsComponentNull())
                return;

            if (clickedShip.isPlacedShip)
            {
                Debug.Log($"{clickedShip.name} đã được đặt rồi!");
                return;
            }

            foreach (var s in m_shipList)
            {
                s.isSelectedShip = false;
            }

            m_selectedShip = clickedShip;
            m_selectedShip.isSelectedShip = true;
            Debug.Log($"Đã chọn tàu {m_selectedShip.name}!");   
        }
        private void PlaceShipOnGrid(RaycastHit2D hit)
        {
            if (hit.collider == null || 
                !hit.collider.CompareTag(Const.PLAYER_CELL_TAG) || 
                IsComponentNull())
            {
                return;
            }
            
            if (hit.collider.CompareTag(Const.PLAYER_CELL_TAG))
            {
                Debug.Log("Hãy chọn tàu để click vào lưới!");
                return;
            }    

            Cell cell = hit.collider.GetComponent<Cell>();
            if (cell == null)
            {
                return;
            }

            m_chosenPos = hit.transform.position;

            var oldCells = m_selectedShip.GetOccupiedCells();
            if (oldCells != null)
            {
                foreach (var oldCell in oldCells)
                {
                    oldCell.hasShip = false;
                    oldCell.OccupiedShip = null;
                }
            }   
            
            m_selectedShip.MoveShip(m_chosenPos);
            if (m_selectedShip.isPlacedShip)
            {
                var newCells = m_selectedShip.GetOccupiedCells();
                foreach (var newCell in newCells)
                {
                    newCell.hasShip = true;
                    newCell.OccupiedShip = m_selectedShip;
                }

                Debug.Log($"{m_selectedShip.name} đã đặt lên lưới!");
                m_selectedShip.isSunkShip = false;
                Debug.Log($"Trạng thái của {m_selectedShip.name}: {(m_selectedShip.isSunkShip ? "Chìm" : "Nổi")}");
                m_selectedShip.isSelectedShip = false;
                shipCount--;
                m_selectedShip = null;
                if (shipCount <= 0)
                {
                    Debug.Log("Bạn đã đặt hết tàu!");
                    m_manage.playGameBtn.gameObject.SetActive(true);
                    return;
                }
            }
            else
            {
                Debug.Log("Vị trí không hợp lệ. Hãy đặt lại!");
                m_chosenPos = Vector3.zero;
                return;
            }      
        }
        public bool IsComponentNull()
        {
            bool check = m_gridMng == null || m_manage == null;
            if (check)
            {
                Debug.LogWarning("Có component bị rỗng. Hãy kiểm tra lại!");
            }
            return check;
        }
        public void RotateShip()
        {
            if (m_selectedShip != null && m_selectedShip.isSelectedShip)
            {
                m_selectedShip.RotateShip();
            }    
        }    
    }
}