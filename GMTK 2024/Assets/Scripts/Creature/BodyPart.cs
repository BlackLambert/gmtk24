using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    public class BodyPart : MonoBehaviour, IPointerClickHandler, IBeginDragHandler
    {
        private const int _rightMouseButtonId = -2;
        
        public event Action<BodyPart> OnRightClick;
        public event Action<BodyPart> OnDragStart;

        [field: SerializeField] public BodyPartSettings BodyPartSettings { get; private set; }
        
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
    }
}
