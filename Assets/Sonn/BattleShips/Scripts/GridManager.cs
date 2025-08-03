using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

namespace Sonn.BattleShips
{
    public class GridManager : MonoBehaviour
    {
        public static GridManager Ins;

        public GameObject cellPrefab;
        public Vector3 offsetPos, offsetScale;
        public Vector2 minBound, maxBound;

        private int m_row = 10, m_col = 10;
        private float m_cellDistance = -0.9f;
        private Cell[,] m_cells;
        private List<Cell> m_cellList;

        public List<Cell> CellList { get => m_cellList; }
        public int Row { get => m_row; }
        public int Col { get => m_col; }
        public float CellDistance { get => m_cellDistance; }

        private void Awake()
        {
            m_cellList = new();
            m_cells = new Cell[m_row, m_col];
            MakeSingleton();
        }
        private void Start()
        {
            DrawGridMap();
            OffsetOfGridMap();
        }
        private void DrawGridMap()
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

                    var c = cell.GetComponent<Cell>();
                    if (c == null)
                    {
                        Debug.LogWarning($"Cell tại [{x}, {y}] bị rỗng!");
                        continue;
                    }

                    m_cells[x, y] = c;
                    m_cells[x, y].cellPosOnGrid = new Vector2(
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
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            Vector3 sizeBox = new(
                Mathf.Abs(maxBound.x - minBound.x),
                Mathf.Abs(maxBound.y - minBound.y),
                0);

            Vector3 centerBox = new(
                ((minBound.x + maxBound.x) / 2f),
                ((minBound.y + maxBound.y) / 2f),
                0);

            Gizmos.DrawWireCube(centerBox, sizeBox);

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
