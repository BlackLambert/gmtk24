using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game
{
    public class BodyPartButton : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IDragHandler, IPointerExitHandler
    {
        public event Action<BodyPart> OnDragExit;
        
        [SerializeField] 
        private Image _icon;

        [SerializeField] 
        private Transform _costsHook;

        [SerializeField] 
        private BodyPartCostsDisplay _costsPrefab;

        [SerializeField] 
        private GameObject _blocker;

        private BodyPart _bodyPart;
        private bool _dragging;
        private CollectedFood _collectedFood;

        private void Awake()
        {
            _collectedFood = FindObjectOfType<CollectedFood>();
        }

        private void Update()
        {
            _blocker.SetActive(!_collectedFood.Has(_bodyPart.BodyPartSettings.Costs));
        }

        public void Init(BodyPart bodyPartPrefab)
        {
            _bodyPart = bodyPartPrefab;
            BodyPartSettings settings = bodyPartPrefab.BodyPartSettings;
            _icon.sprite = bodyPartPrefab.BodyPartSettings.Icon;
            foreach (FoodAmount amount in settings.Costs)
            {
                BodyPartCostsDisplay costsDisplay = Instantiate(_costsPrefab, _costsHook);
                costsDisplay.Init(amount);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.pointerId == -1)
            {
                _dragging = false;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _dragging = eventData.pointerId == -1;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            
        }

        public void OnDrag(PointerEventData eventData)
        {
           
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_dragging)
            {
                _dragging = false;
                OnDragExit?.Invoke(_bodyPart);
            }
        }
    }
}
