using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class FootStepSpawn : MonoBehaviour
    {
        [field: SerializeField] LegAnimationController controller;

        [field: SerializeField] SpriteRenderer LegRenderer;

        [field: SerializeField] FootStep FootStep;
        [SerializeField] Transform spawnTransform;

        private GameHook _hook;

        public void SpawnFootstep(int i)
        {
            if(i == 0)
            {
                if (LegRenderer.flipY == true) return; 
            } else
            {
                if (LegRenderer.flipY == false) return;
            }

            if (_hook == null)
            {
                _hook = FindObjectOfType<GameHook>();
            }
            if(controller != null)
            {
                controller.PlayFootstep(0.3f);
            }
            FootStep footstep = GameObject.Instantiate(FootStep, _hook.transform, false);
            var trans = footstep.transform;
            trans.position = spawnTransform.position;
            trans.rotation = spawnTransform.rotation;
            footstep.SetFlip(LegRenderer.flipX, LegRenderer.flipY);
        }
    }
}
