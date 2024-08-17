using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class BodyPartsTab : MonoBehaviour
    {
        public event Action<BodyPartType> OnClick;
        
        [field: SerializeField] 
        public BodyPartType Type { get; private set; }

        [SerializeField] 
        private Button _button;

        private void Start()
        {
            _button.onClick.AddListener(InvokeOnClick);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(InvokeOnClick);
        }

        public void SetInteractable(bool interactable)
        {
            _button.interactable = interactable;
        }

        private void InvokeOnClick()
        {
            OnClick?.Invoke(Type);
        }
    }
}
