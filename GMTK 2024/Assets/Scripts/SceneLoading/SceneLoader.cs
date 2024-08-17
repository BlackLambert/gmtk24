using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public abstract class SceneLoader : MonoBehaviour
    {
        [SerializeField] protected LoadSettings[] _loadSettings;

        protected void DoLoading()
        {
            foreach (LoadSettings loadSettings in _loadSettings)
            {
                if (loadSettings.Load)
                {
                    SceneManager.LoadScene(loadSettings.SceneName,
                        loadSettings.Additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
                }
                else
                {
                    SceneManager.UnloadSceneAsync(loadSettings.SceneName);
                }
            }
        }

        [Serializable]
        public class LoadSettings
        {
            public bool Load;
            public string SceneName;
            public bool Additive;
        }
    }
}