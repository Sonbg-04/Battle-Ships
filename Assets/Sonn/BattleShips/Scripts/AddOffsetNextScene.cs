using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sonn.BattleShips
{
    public class AddOffsetNextScene : MonoBehaviour
    {
        public bool isBattle;
        public Vector3 offsetNextScenePos;

        private void Update()
        {
            if (!isBattle)
            {
                AddOffsetToGameObject();
            }    
        }
        private void AddOffsetToGameObject()
        {
            Scene scene = SceneManager.GetActiveScene();
            if (scene != null)
            {
                string nameScene = scene.name;
                if (nameScene == Const.GAME_PLAY_1_VS_AI_SCENE)
                {
                    isBattle = true;
                    if (gameObject.layer == LayerMask.NameToLayer(Const.PLAYER_1_SHIP_LAYER)
                        || gameObject.layer == LayerMask.NameToLayer(Const.PLAYER_1_CELL_LAYER))
                    {
                        transform.position = new Vector3(
                                  transform.position.x + offsetNextScenePos.x,
                                  transform.position.y + offsetNextScenePos.y,
                                  0);
                    }
                    else
                    {
                        offsetNextScenePos = Vector3.zero;
                        transform.position = new Vector3(
                                  transform.position.x + offsetNextScenePos.x,
                                  transform.position.y + offsetNextScenePos.y,
                                  0);
                    }    
                }
                else if (nameScene == Const.GAME_PLAY_1_VS_1_SCENE)
                {
                    isBattle = true;
                    if (gameObject.layer == LayerMask.NameToLayer(Const.PLAYER_1_SHIP_LAYER)
                        || gameObject.layer == LayerMask.NameToLayer(Const.PLAYER_1_CELL_LAYER))
                    {
                        if (gameObject.layer == LayerMask.NameToLayer(Const.PLAYER_1_SHIP_LAYER))
                        {
                            var shipRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
                            if (shipRenderer != null)
                            {
                                shipRenderer.enabled = false;
                            }

                            var shipCol = gameObject.GetComponentInChildren<Collider2D>();
                            if (shipCol != null)
                            {
                                shipCol.enabled = false;
                            }
                        }
                        transform.position = new Vector3(
                                  transform.position.x + offsetNextScenePos.x,
                                  transform.position.y + offsetNextScenePos.y,
                                  0);
                    }
                    else
                    {
                        if (gameObject.layer == LayerMask.NameToLayer(Const.PLAYER_2_SHIP_LAYER))
                        {
                            var shipRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
                            if (shipRenderer != null)
                            {
                                shipRenderer.enabled = false;
                            }

                            var shipCol = gameObject.GetComponentInChildren<Collider2D>();
                            if (shipCol != null)
                            {
                                shipCol.enabled = false;
                            }    
                        }    
                        offsetNextScenePos = new Vector3(8.15f, 0, 0);
                        transform.position = new Vector3(
                                  transform.position.x + offsetNextScenePos.x,
                                  transform.position.y + offsetNextScenePos.y,
                                  0);
                    }
                }    
            }
        }    

    }
}
