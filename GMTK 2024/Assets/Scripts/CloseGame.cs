using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class CloseGame : MonoBehaviour
    {
        [SerializeField] private KeyCode _key;

        private void Update()
        {
            if (Input.GetKeyDown(_key))
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
                Application.OpenURL(webplayerQuitURL);
#else
                Application.Quit();
#endif
            }
        }
    }
}