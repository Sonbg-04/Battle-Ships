using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sonn.BattleShips
{
    public class Ship : MonoBehaviour, IComponentChecking
    {
        public bool isSunkShip, isSelectedShip, 
                    isPlacedShip, isRotatedShip;
        public Vector3 offsetPos;

        private List<Cell> m_occupiedCells;
        private int m_rotateCounter = 0;
        private Coroutine m_coroutine;

        public List<Cell> OccupiedCells { get => m_occupiedCells; }

        private void Awake()
        {
            m_occupiedCells = new();
            m_coroutine = null;
        }
        private void SetAlphaShip(float alpha)
        {
            SpriteRenderer sp = GetComponentInChildren<SpriteRenderer>();
            if (sp != null)
            {
                Color cl = sp.color;
                cl.a = alpha;
                sp.color = cl;
            }
        }
        public void StartFlashing()
        {
            if (m_coroutine == null)
            {
                m_coroutine = StartCoroutine(FlashCoroutine());
            }    
            
        }
        public void StopFlashing()
        {
            if (m_coroutine != null)
            {
                StopCoroutine(m_coroutine);
                SetAlphaShip(1f);
                m_coroutine = null;
            }    
        }
        IEnumerator FlashCoroutine()
        {
            while (true)
            {
                SetAlphaShip(0.5f);
                yield return new WaitForSeconds(0.2f);
                SetAlphaShip(1f);
                yield return new WaitForSeconds(0.2f);
            }
        }    
        public List<Cell> GetOccupiedCells()
        {
            m_occupiedCells.Clear();

            Collider2D[] shipParts = GetComponentsInChildren<Collider2D>();

            foreach (var part in shipParts)
            {
                Vector2 centerPos = part.bounds.center;
                Vector2 sizePos = part.bounds.size;

                Collider2D[] results = Physics2D.OverlapBoxAll(
                    centerPos, sizePos, 0, LayerMask.GetMask(Const.CELL_LAYER)
                    );

                foreach (var cell in results)
                {
                    if (cell.CompareTag(Const.PLAYER_CELL_TAG))
                    {
                        var c = cell.GetComponent<Cell>();
                        if (c != null && !m_occupiedCells.Contains(c))
                        {
                            m_occupiedCells.Add(c);
                        }
                    }
                }
            }
            return m_occupiedCells;
        }
        public void MoveShip(Vector3 pos)
        {
            if (IsComponentNull())
            {
                return;
            }   
            
            if (isRotatedShip)
            {
                transform.position = new Vector3(pos.x + offsetPos.x, pos.y, 0);
            }
            else
            {
                transform.position = new Vector3(pos.x, pos.y + offsetPos.y, 0);
            }

            Physics2D.SyncTransforms();
            
        }
        public bool IsWithInGridBounds()
        {
            Renderer rd = GetComponentInChildren<Renderer>();
            if (rd == null)
            {
                return false;
            }

            Bounds b = rd.bounds;
            return b.min.x >= GridManager.Ins.minBound.x 
                && b.max.x <= GridManager.Ins.maxBound.x
                && b.min.y >= GridManager.Ins.minBound.y 
                && b.max.y <= GridManager.Ins.maxBound.y;
        }
        public bool CheckForOverlappingShips()
        {
            bool check = false;
            Collider2D[] shipParts = GetComponentsInChildren<Collider2D>();
            if (shipParts == null)
            {
                return check;
            }
            foreach (var part in shipParts)
            {
                Vector2 centerPos = part.bounds.center;
                Vector2 sizePos = part.bounds.size;
                Collider2D[] hits = Physics2D.OverlapBoxAll(
                    centerPos, sizePos, 
                    0, LayerMask.GetMask(Const.SHIP_PLAYER_LAYER));

                foreach (var hit in hits)
                {
                    if (hit != part && !hit.transform.IsChildOf(transform))
                    {
                        Debug.Log($"Đã va chạm với {hit.name}. Vui lòng đặt lại!");
                        check = true;
                    }    
                }    
            }
            return check;
        }
        public bool IsComponentNull()
        {
            bool check = Manage.Ins == null || GridManager.Ins == null;
            if (check)
            {
                Debug.LogWarning("Có component bị rỗng. Hãy kiểm tra lại!");
            }
            return check;
        }
        public void RotateShip()
        {
            float rotationZ = -90f;
            transform.Rotate(Vector3.forward, rotationZ);

            if (isRotatedShip)
            {
                transform.position -= offsetPos;
            }
            else
            {
                transform.position += offsetPos;
            }

            m_rotateCounter++;
            isRotatedShip = m_rotateCounter % 2 != 0;
        }
        public bool IsVertical()
        {
            float rotZ = transform.eulerAngles.z;
            return Mathf.Approximately(rotZ % 180, 90f);
        }

    }
}
