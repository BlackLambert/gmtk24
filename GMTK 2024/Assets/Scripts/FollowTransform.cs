using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class FollowTransform : MonoBehaviour
    {
        [SerializeField] 
        private Transform _transform;
        
        [SerializeField] 
        private Transform _target;

        private void Update()
        {
            Vector3 targetPos = _target.position;
            _transform.position = new Vector3(targetPos.x, targetPos.y, _transform.position.z);
        }
    }
}
