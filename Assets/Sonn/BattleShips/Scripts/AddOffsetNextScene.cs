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
                    transform.position = new Vector3(
                        transform.position.x + offsetNextScenePos.x,
                        transform.position.y + offsetNextScenePos.y,
                        0
                        );
                    isBattle = true;
                }    
            }
        }    

    }
}
