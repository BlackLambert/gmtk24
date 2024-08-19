using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class EyeController : MonoBehaviour
    {
        bool offsetApplied = false;
        
        void Start()
        {
            float offset = Random.Range(0, 2f);
            foreach (EyeController eye in transform.parent.GetComponentsInChildren<EyeController>())
            {
                eye.ApplyOffset(offset);
            }
        }

        public void ApplyOffset(float offset)
        {
            if (offsetApplied) return;

            GetComponent<Animator>().Play("Blink", 0, offset);
            offsetApplied = true;
        }
    }
}
