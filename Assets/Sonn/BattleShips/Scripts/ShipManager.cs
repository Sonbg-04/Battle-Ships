using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sonn.BattleShips
{
    public class ShipManager : MonoBehaviour, IComponentChecking
    {
        public GameObject[] shipPrefabs;
        public Vector3 offsetPos;
        public int shipCount;
        public bool isPlacingShip;
        
        private static Dictionary<Type, MonoBehaviour> m_ins;
        
        private readonly float m_shipDistance = 1.3f;
        private Ship m_selectedShip;
        private List<Ship> m_shipList;
        private Vector3 m_chosenPos;

        public List<Ship> ShipList { get => m_shipList; }
        
        public static T GetInstance<T>() where T : MonoBehaviour
        {
            if (m_ins.TryGetValue(typeof(T), out var ins))
            {
                return ins as T;
            }
            return null;
        }

        private void Awake()
        {
            m_ins = new();
            m_selectedShip = null;
            m_chosenPos = Vector3.zero;
            m_shipList = new();
            MakeSingleton();
        }
        private void Start()
        {
            SetShipOnScreen(SceneManager.GetActiveScene());
            OffsetOfShips();
            shipCount = m_shipList.Count;
        }
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                HandleMouseClick(SceneManager.GetActiveScene());
            }
        }
        private void SetShipOnScreen(Scene sc)
        {
            if (shipPrefabs.Length <= 0 || IsComponentNull())
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
                
                Vector3 shipPos = new((i * m_shipDistance), 0, 0);
                ship.transform.position = shipPos;
                ship.transform.SetParent(transform);
                ship.name = shipPrefabs[i].name;

                if (sc.name == Const.SET_PLACESHIP_PLAYER_2_SCENE)
                {
                    SetLayerShip(ship, LayerMask.NameToLayer(Const.PLAYER_2_SHIP_LAYER));
                }    

                var shipClone = ship.GetComponent<Ship>();
                m_shipList.Add(shipClone);
            }
            Debug.Log($"Có {m_shipList.Count} tàu đã được lưu lại!");
        }
        private void SetLayerShip(GameObject ship, int layer)
        {
            ship.layer = layer;
            foreach (Transform t in ship.transform)
            {
                t.gameObject.layer = layer;
            }
        }
        private void OffsetOfShips()
        {
            transform.position += offsetPos;
        }
        private void HandleMouseClick(Scene sc)
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
            
            if (sc.name == Const.SET_PLACESHIP_PLAYER_1_SCENE)
            {
                if (hit.collider.CompareTag(Const.PLAYER_1_CELL_TAG))
                {
                    PlaceShipOnGrid(hit, sc.name, Const.PLAYER_1_CELL_TAG, Const.PLAYER_1_SHIP_LAYER);
                }
            }
            else if (sc.name == Const.SET_PLACESHIP_PLAYER_2_SCENE)
            {
                if (hit.collider.CompareTag(Const.PLAYER_2_CELL_TAG))
                {
                    PlaceShipOnGrid(hit, sc.name, Const.PLAYER_2_CELL_TAG, Const.PLAYER_2_SHIP_LAYER);
                }
            }

            SelectShip(hit);
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
                Debug.Log("Hãy chờ tàu đặt xong!");
                m_selectedShip.StartFlashing();
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
        private void PlaceShipOnGrid(RaycastHit2D hit, string nameScene, 
                        string tagname, string layername)
        {
            if (hit.collider == null || 
                !hit.collider.CompareTag(tagname) ||
                m_selectedShip == null ||
                IsComponentNull())
            {
                Debug.LogWarning("Không có tàu nào được chọn để đặt!");
                return;
            }

            m_chosenPos = hit.transform.position;
            m_selectedShip.MoveShip(m_chosenPos);

            if (!m_selectedShip.IsWithInGridBounds())
            {
                Debug.Log("Tàu đặt ngoài phạm vi lưới. Hãy đặt lại!");
                return;
            }

            if (m_selectedShip.CheckForOverlappingShips(layername))
            {
                return;
            }

            var newCells = m_selectedShip.GetOccupiedCells(tagname);
            if (IsShipNextToAnotherShip(newCells, nameScene))
            {
                Debug.Log("Không được đặt tàu cạnh nhau!");
                return;
            }

            List<Transform> shipParts = new();
            foreach (Transform part in m_selectedShip.transform)
            {
                shipParts.Add(part);
            }
            bool isVertical = m_selectedShip.IsVertical();
            if (isVertical)
            {
                shipParts.Sort((a, b) => a.position.y.CompareTo(b.position.y));
            }
            else
            {
                shipParts.Sort((a, b) => a.position.x.CompareTo(b.position.x));
            }

            for (int i = 0; i < newCells.Count; i++)
            {
                var playerCell = newCells[i];
                var shipPart = shipParts[i];

                shipPart.position = playerCell.transform.position;
                
                if (nameScene == Const.SET_PLACESHIP_PLAYER_1_SCENE)
                {
                    playerCell.hasPlayerOneShip = true;
                }
                else if (nameScene == Const.SET_PLACESHIP_PLAYER_2_SCENE)
                {
                    playerCell.hasPlayerTwoShip = true;
                }    

                playerCell.shipPartTransform = shipPart;
            }

            Debug.Log($"{m_selectedShip.name} đã đặt lên lưới!");

            m_selectedShip.isSelectedShip = false;
            m_selectedShip.isPlacedShip = true;
            m_selectedShip.StopFlashing();

            Debug.Log($"Trạng thái của {m_selectedShip.name}: {(m_selectedShip.isSunkShip ? "Chìm" : "Nổi")}");

            shipCount--;

            if (shipCount == 0)
            {
                Debug.Log("Bạn đã đặt hết tàu!");
                Manage.GetIns<Manage>().ShowBtnNextOrPlay(nameScene);
            }

            m_selectedShip = null;
            m_chosenPos = Vector3.zero;
            isPlacingShip = false;

        }
        private bool IsShipNextToAnotherShip(List<Cell> occupiedCells, string nameSc)
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
                        foreach (var c in GridManager.GetInstance<GridManager>().CellList)
                        {
                            if (nameSc == Const.SET_PLACESHIP_PLAYER_1_SCENE)
                            {
                                if (c.cellPosOnGrid == neighborCellPos
                                && c.hasPlayerOneShip)
                                {
                                    return true;
                                }
                            }
                            else if (nameSc == Const.SET_PLACESHIP_PLAYER_2_SCENE)
                            {
                                if (c.cellPosOnGrid == neighborCellPos
                                && c.hasPlayerTwoShip)
                                {
                                    return true;
                                }
                            }    
                        }    
                    }
                }
            }    
            return false;
        }    
        public bool IsComponentNull()
        {
            var gridMng = GridManager.GetInstance<GridManager>();
            var Mng = Manage.GetIns<Manage>();
            bool check = gridMng == null || Mng == null;
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
            var key = GetType();

            if (!m_ins.ContainsKey(key) || m_ins[key] == null)
            {
                m_ins[key] = this;
                DontDestroyOnLoad(this);
                SceneManager.sceneLoaded += OnSceneLoaded;
            }
            else
            {
                Destroy(gameObject);
            }

        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;

            var key = GetType();

            if (m_ins.ContainsKey(key) && m_ins[key] == this)
            {
                m_ins.Remove(key);
            }
        }
        private void OnSceneLoaded(Scene sc, LoadSceneMode mode)
        {
            string sceneName = sc.name;

            if (sceneName == Const.SET_PLACESHIP_PLAYER_2_SCENE)
            {
                if (IsSceneSetPlaceShipPlayer_1_Object())
                {
                    gameObject.SetActive(false);
                }
            }
            else if (sceneName == Const.GAME_PLAY_1_VS_1_SCENE)
            {
                if (IsSceneSetPlaceShipPlayer_1_Object())
                {
                    gameObject.SetActive(true);
                }
            }
            else if (sceneName == Const.MAIN_MENU_SCENE)
            {
                Destroy(gameObject);
            }
        }
        private bool IsSceneSetPlaceShipPlayer_1_Object()
        {
            return gameObject.CompareTag(Const.SET_PLACESHIP_PLAYER_1_TAG);
        }
    }
}