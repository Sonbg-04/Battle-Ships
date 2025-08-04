using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sonn.BattleShips
{
    public class EnemyAI : MonoBehaviour, IComponentChecking
    {
        public GameObject enemyCellPrefab;
        public GameObject[] enemyShipPrefabs;
        public Vector3 offsetPos, offsetScalePos;
        public bool isEnemyShoot, isEnemySelectedCell;

        private Cell[, ] m_enemyCell;
        private List<Cell> m_enemyCellList;

        private void Awake()
        {
            m_enemyCell = new Cell[GridManager.Ins.Row, GridManager.Ins.Col];
            m_enemyCellList = new();    
        }
        private void Start()
        {
            EnemyDrawGridMap();
            OffsetOfGrid();
        }
        public bool IsComponentNull()
        {
            bool check = GridManager.Ins == null;
            if (check)
            {
                Debug.LogWarning("Có component bị rỗng. Hãy kiểm tra lại!");
            }
            return check;
        }
        private void EnemyDrawGridMap()
        {
            if (IsComponentNull())
            {
                return;
            }

            for (int x = 0; x < GridManager.Ins.Row; x++)
            {
                for (int y = 0; y < GridManager.Ins.Col; y++)
                {
                    var enemyCell = Instantiate(enemyCellPrefab, Vector3.zero, Quaternion.identity);
                    Vector3 pos = new(
                        x * -GridManager.Ins.CellDistance,
                        y * -GridManager.Ins.CellDistance,
                        0
                        );
                    enemyCell.transform.position = pos;
                    enemyCell.transform.SetParent(transform);
                    enemyCell.tag = Const.ENEMY_CELL_TAG;
                    enemyCell.name = $"EnemyCell[{x}][{y}]";

                    var c = enemyCell.GetComponent<Cell>();
                    if (c == null)
                    {
                        return;
                    }
                    m_enemyCell[x, y] = c;
                    m_enemyCell[x, y].cellPosOnGrid = new Vector2(
                        Mathf.RoundToInt(m_enemyCell[x, y].transform.position.x / -GridManager.Ins.CellDistance),
                        Mathf.RoundToInt(m_enemyCell[x, y].transform.position.y / -GridManager.Ins.CellDistance)
                        );

                    m_enemyCellList.Add(m_enemyCell[x, y]);
                }
            }
            Debug.Log($"Có {m_enemyCellList.Count} được lưu vào list Enemy cell!");
        }
        private void OffsetOfGrid()
        {
            transform.position += offsetPos;

            transform.localScale = new Vector3(
                transform.localScale.x * offsetScalePos.x,
                transform.localScale.y * offsetScalePos.y,
                transform.localScale.z
                );

        }

    }
}

