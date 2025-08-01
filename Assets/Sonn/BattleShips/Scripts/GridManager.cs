using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sonn.BattleShips
{
    public class GridManager : MonoBehaviour, IComponentChecking
    {
        public GameObject cellPrefab;
        public Vector3 offsetPos, offsetScale;
        
        private int m_row = 10, m_col = 10;
        private float m_cellDistance = -0.9f;
        private Cell[,] m_cells;

        private void Start()
        {
            DrawGridMap();
            OffsetOfGridMap();
        }

        private void DrawGridMap()
        {
            m_cells = new Cell[m_row, m_col];
            for (int x = 0; x < m_row; x++)
            {
                for (int y = 0; y < m_col; y++)
                {
                    GameObject cell = Instantiate(cellPrefab, Vector3.zero, Quaternion.identity);
                    Vector3 cellPos = new(x * m_cellDistance, y * m_cellDistance, 0);
                    cell.transform.position = cellPos;
                    cell.transform.SetParent(transform);
                    cell.name = "Cell[" + x + "]" + "[" + y + "]";

                    Cell c = cell.GetComponent<Cell>();
                    if (c == null)
                    {
                        Debug.LogWarning("Có cell bị rỗng. Hãy kiểm tra lại!");
                        continue;
                    }

                    m_cells[x, y] = c;
                }
            }
        }

        public void CheckCollisionOfShipWithCell()
        {
            if (IsComponentNull())
            {
                return;
            }    
            for (int x = 0; x < m_row; x++)
            {
                for (int y = 0; y < m_col; y++)
                {
                    Cell c = m_cells[x, y];
                    if (c == null)
                    {
                        Debug.LogWarning("Trong m_cells không có 1 cell nào đó. Hãy kiểm tra lại!");
                        continue;
                    }    
                    Vector2 centerPos = c.transform.position;
                    Vector2 cellSize = c.GetComponent<Collider2D>().bounds.size;

                    Collider2D col = Physics2D.OverlapBox(centerPos, cellSize,
                                     0, LayerMask.GetMask(Const.SHIP_LAYER));

                    if (col != null)
                    {
                        Debug.Log("Ship đã được đặt lên trên Cell!");
                        c.hasShip = true;
                    }
                    else
                    {
                        Debug.Log("Ship đã rời va chạm Cell!");
                        c.hasShip = false;
                    }    
                }    
            }    
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

        public bool IsComponentNull()
        {
            bool check = m_cells == null;
            if (check)
            {
                Debug.LogWarning("Có component bị rỗng. Hãy kiểm tra lại!");
            }
            return check;
        }
    }
}
