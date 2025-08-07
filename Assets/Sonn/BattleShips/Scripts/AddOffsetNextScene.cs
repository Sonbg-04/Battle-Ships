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
                if (nameScene == Const.GAME_PLAY_SCENE)
                {
                    isBattle = true;
                    if (gameObject.layer == LayerMask.NameToLayer(Const.SHIP_PLAYER_LAYER)
                        || gameObject.CompareTag(Const.PLAYER_CELL_TAG))
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
            }
        }    

    }
}
