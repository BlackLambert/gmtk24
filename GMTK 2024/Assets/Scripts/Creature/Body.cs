using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Body : MonoBehaviour
    {
        [SerializeField] 
        private MeshFilter _meshFilter;

        public Vector3 GetNextHookTo(Vector3 point)
        {
            throw new NotImplementedException();
        }
    }
}
