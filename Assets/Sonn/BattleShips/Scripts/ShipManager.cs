using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Sonn.BattleShips
{
    public class ShipManager : MonoBehaviour, IComponentChecking
    {
        public static ShipManager Ins;

        public GameObject[] shipPrefabs;
        public Vector3 offsetPos;
        public int shipCount;
        public bool isPlacingShip;

        private float m_shipDistance = 1.3f;
        private Ship m_selectedShip;
        private List<Ship> m_shipList;
        private Vector3 m_chosenPos;
        private Manage m_manage;

        private void Awake()
        {
            m_selectedShip = null;
            m_chosenPos = Vector3.zero;
            m_shipList = new();
            m_manage = FindObjectOfType<Manage>();
            MakeSingleton();
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
            {
                return;
            }

            if (isPlacingShip)
            {
                Debug.Log("Hãy chờ đặt tàu xong!");
                if (m_selectedShip != null)
                {
                    m_selectedShip.isSelectedShip = true;
                    m_selectedShip.StartFlashing();
                }
                return;
            }

            if (clickedShip.isPlacedShip)
            {
                Debug.Log($"{clickedShip.name} đã được đặt rồi!");
                return;
            }

            foreach (var s in m_shipList)
            {
                s.isSelectedShip = false;
                s.StopFlashing();
            }

            m_selectedShip = clickedShip;
            m_selectedShip.isSelectedShip = true;
            m_selectedShip.isPlacedShip = false;
            m_selectedShip.isSunkShip = false;
            m_selectedShip.StartFlashing();
            Debug.Log($"Đã chọn tàu {m_selectedShip.name}!");
            isPlacingShip = true;
        }
        private void PlaceShipOnGrid(RaycastHit2D hit)
        {
            if (hit.collider == null || 
                !hit.collider.CompareTag(Const.PLAYER_CELL_TAG) || 
                IsComponentNull())
            {
                return;
            }
            
            if (hit.collider.CompareTag(Const.PLAYER_CELL_TAG) 
                && m_selectedShip == null)
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
            m_selectedShip.MoveShip(m_chosenPos);

            if (!m_selectedShip.IsWithInGridBounds())
            {
                Debug.Log("Tàu đặt ngoài phạm vi lưới. Hãy đặt lại!");
                return;
            }
            if (m_selectedShip.CheckForOverlappingShips())
            {
                return;
            }

            var newCells = m_selectedShip.GetOccupiedCells();
            if (IsShipNextToAnotherShip(newCells))
            {
                Debug.Log("Không được đặt tàu cạnh nhau!");
                return;
            }
            else
            {
                foreach (var newCell in newCells)
                {
                    newCell.hasShip = true;
                }

                Debug.Log($"{m_selectedShip.name} đã đặt lên lưới!");

                m_selectedShip.isSelectedShip = false;
                m_selectedShip.isSunkShip = false;
                m_selectedShip.isPlacedShip = true;
                m_selectedShip.StopFlashing();

                Debug.Log($"Trạng thái của {m_selectedShip.name}: {(m_selectedShip.isSunkShip ? "Chìm" : "Nổi")}");

                shipCount--;

            }

            if (shipCount == 0)
            {
                Debug.Log("Bạn đã đặt hết tàu!");
                m_manage.playGameBtn.gameObject.SetActive(true);
            }

            m_selectedShip = null;
            m_chosenPos = Vector3.zero;
            isPlacingShip = false;

        }
        private bool IsShipNextToAnotherShip(List<Cell> occupiedCells)
        {
            foreach (var cell in occupiedCells)
            {
                Vector2 cellPos = cell.cellPosOnGrid;
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        if (x == 0 && y == 0)
                        {
                            continue;
                        }
                        Vector2 neighborCellPos = new(cellPos.x + x, cellPos.y + y);
                        foreach (var c in GridManager.Ins.CellList)
                        {
                            if (c.cellPosOnGrid == neighborCellPos 
                                && c.hasShip)
                            {
                                return true;
                            }    
                        }    
                    }
                }
            }    
            return false;
        }    
        public bool IsComponentNull()
        {
            bool check = GridManager.Ins == null || m_manage == null;
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
        private void MakeSingleton()
        {
            if (Ins == null)
            {
                Ins = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}