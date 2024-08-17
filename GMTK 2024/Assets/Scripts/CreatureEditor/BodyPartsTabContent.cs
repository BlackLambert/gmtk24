using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class BodyPartsTabContent : MonoBehaviour
    {
        public event Action<BodyPart> OnBodyPartButtonDragExit;
        
        [field: SerializeField] 
        public BodyPartType Type { get; private set; }

        [SerializeField] 
        private BodyPartCollectionSettings _bodyPartCollection;

        [SerializeField] 
        private Transform _hook;

        [SerializeField] 
        private BodyPartButton _buttonPrefab;

        private List<BodyPartButton> _buttons = new List<BodyPartButton>();

        private void Start()
        {
            CreateButtons();
        }

        private void OnDestroy()
        {
            foreach (BodyPartButton button in _buttons)
            {
                button.OnDragExit -= OnButtonDragExit;
            }
        }

        public void Show(bool show)
        {
            gameObject.SetActive(show);
        }

        private void CreateButtons()
        {
            foreach (BodyPart part in _bodyPartCollection.BodyParts.Where(p => p.BodyPartSettings.BodyPartType == Type))
            {
                BodyPartButton button = Instantiate(_buttonPrefab, _hook);
                button.Init(part);
                button.OnDragExit += OnButtonDragExit;
                _buttons.Add(button);
            }
        }

        private void OnButtonDragExit(BodyPart part)
        {
            OnBodyPartButtonDragExit?.Invoke(part);
        }
    }
}
