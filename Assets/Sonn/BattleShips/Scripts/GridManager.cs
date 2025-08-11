using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sonn.BattleShips
{
    public class GridManager : MonoBehaviour
    {
        public GameObject cellPrefab;
        public Vector3 offsetPos, offsetScale;
        public Vector2 minBound, maxBound;

        private static Dictionary<Type, MonoBehaviour> m_ins;
     
        private readonly int m_row = 10;
        private readonly int m_col = 10;
        private readonly float m_cellDistance = -0.9f;
        private Cell[,] m_cells;
        private List<Cell> m_cellList;

        public List<Cell> CellList { get => m_cellList; }
        public int Row { get => m_row; }
        public int Col { get => m_col; }
        public float CellDistance { get => m_cellDistance; }

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
            m_cellList = new();
            m_cells = new Cell[m_row, m_col];
            MakeSingleton();
        }
        private void Start()
        {
            DrawGridMap(SceneManager.GetActiveScene());
            OffsetOfGridMap();
        }
        private void DrawGridMap(Scene sc)
        {
            for (int x = 0; x < m_row; x++)
            {
                for (int y = 0; y < m_col; y++)
                {
                    var cell = Instantiate(cellPrefab, Vector3.zero, Quaternion.identity);
                    Vector3 cellPos = new(x * m_cellDistance, y * m_cellDistance, 0);
                    cell.transform.position = cellPos;
                    cell.transform.SetParent(transform);
                    cell.name = "Cell[" + x + "]" + "[" + y + "]";

                    if (sc.name == Const.SET_PLACESHIP_PLAYER_2_SCENE)
                    {
                        cell.tag = Const.PLAYER_2_CELL_TAG;
                        cell.layer = LayerMask.NameToLayer(Const.PLAYER_2_CELL_LAYER);
                    }   
                    
                    var c = cell.GetComponent<Cell>();

                    m_cells[x, y] = c;
                    m_cells[x, y].cellPosOnGrid = new Vector2Int(
                    Mathf.RoundToInt(m_cells[x, y].transform.position.x / m_cellDistance),
                    Mathf.RoundToInt(m_cells[x, y].transform.position.y / m_cellDistance));

                    m_cellList.Add(m_cells[x, y]);
                }
            }
            Debug.Log($"Có {m_cellList.Count} ô được lưu vào list cell!");

        }
        private void OffsetOfGridMap()
        {
            transform.position += offsetPos;

            transform.localScale = new Vector3(
                transform.localScale.x * offsetScale.x,
                transform.localScale.y * offsetScale.y,
                transform.localScale.z
                );
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
                    gameObject.SetActive(false);
            }
            else if (sceneName == Const.GAME_PLAY_1_VS_1_SCENE)
            {
                if (IsSceneSetPlaceShipPlayer_1_Object())
                    gameObject.SetActive(true);
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
