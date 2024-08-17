using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class SceneOnButtonClickLoader : SceneLoader
    {
        [SerializeField] 
        private Button _button;

        private void Start()
        {
            _button.onClick.AddListener(OnClick);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnClick);
        }

        private void OnClick()
        {
            DoLoading();
        }
    }
}
