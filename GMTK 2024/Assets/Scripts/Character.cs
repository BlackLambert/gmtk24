using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Character : MonoBehaviour
    {
        [field: SerializeField]
        public Rigidbody2D Rigidbody { get; private set; }

        public Transform Transform { get; private set; }
        
        private void Start()
        {
            Transform = transform;
        }
    }
}
