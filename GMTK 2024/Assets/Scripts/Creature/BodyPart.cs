using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    public class BodyPart : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private const int _rightMouseButtonId = -2;

        public event Action<BodyPart> OnRightClick;
        public event Action<BodyPart> OnMouseHoverStart;
        public event Action<BodyPart> OnMouseHoverEnd;
        public event Action<BodyPart> OnDragStart;

        [field: SerializeField] public BodyPartSettings BodyPartSettings { get; private set; }
        [SerializeField] private GameObject _outline;

        private void Awake()
        {
            ShowOutline(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.pointerId == _rightMouseButtonId)
            {
                OnRightClick?.Invoke(this);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            OnDragStart?.Invoke(this);
        }

        public void EnableColliders(bool enable)
        {
            foreach (Collider2D collider in GetComponentsInChildren<Collider2D>())
            {
                collider.enabled = enable;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
        }

        public void ShowOutline(bool show)
        {
            _outline.SetActive(show);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnMouseHoverStart?.Invoke(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnMouseHoverEnd?.Invoke(this);
        }
    }
}