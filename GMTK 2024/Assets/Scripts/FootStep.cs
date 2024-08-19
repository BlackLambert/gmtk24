using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class FootStep : MonoBehaviour
    {
        float t = 0;
        Color colorA = new Color(1, 1, 1, .5f);
        Color colorB = new Color(1, 1, 1, 0);
        public void Start()
        {
            Destroy(gameObject, 2);
        }

        private void Update()
        {
            GetComponent<SpriteRenderer>().color = Color.Lerp(colorA, colorB, t);
            t += Time.deltaTime / 2;
        }

        public void SetFlip(bool flipX, bool flipY)
        {
            GetComponent<SpriteRenderer>().flipX = flipX;
            GetComponent<SpriteRenderer>().flipY = flipY;
        }
    }
}
