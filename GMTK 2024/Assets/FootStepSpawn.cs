using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class FootStepSpawn : MonoBehaviour
    {
        [field: SerializeField] SpriteRenderer LegRenderer;

        [field: SerializeField] FootStep FootStep;
        [SerializeField] Transform spawnTransform;
        
        public void SpawnFootstep(int i)
        {
            if(i == 0)
            {
                if (LegRenderer.flipY == true) return; 
            } else
            {
                if (LegRenderer.flipY == false) return;
            }
            FootStep footstep = GameObject.Instantiate(FootStep, spawnTransform.position, spawnTransform.rotation);
            footstep.SetFlip(LegRenderer.flipX, LegRenderer.flipY);
        }
    }
}
