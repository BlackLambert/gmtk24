using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Creature : MonoBehaviour
    {
        [field: SerializeField]
        public Rigidbody2D Rigidbody { get; private set; }
        
        [field: SerializeField] 
        public Body Body { get; private set; }

        [SerializeField] 
        private BodyPart[] _startParts;

        [SerializeField] 
        private Transform _hook;

        public Transform Transform { get; private set; }
        
        private Dictionary<BodyPartSlot, BodyPart> _slotToBodyParts = new Dictionary<BodyPartSlot, BodyPart>();

        private void Awake()
        {
            Transform = transform;
            foreach (BodyPart bodyPart in _startParts)
            {
                if (bodyPart != null && !Body.Contains(bodyPart))
                {
                    Body.Add(bodyPart);
                }
            }
        }

        public void Add(BodyPart bodyPart, BodyPartSlot slot)
        {
            Body.Add(bodyPart, slot);
            Transform trans = bodyPart.transform;
            trans.SetParent(_hook);
            Vector3 pos = trans.position;
            trans.localPosition = new Vector3(pos.x, pos.y, 0.1f);
        }

        public void Remove(BodyPart bodyPart)
        {
            Body.Remove(bodyPart);
        }

        public void DisableCollider()
        {
            Rigidbody.isKinematic = true;
            foreach (Collider2D collider in GetComponentsInChildren<Collider2D>())
            {
                collider.enabled = false;
            }
        }

        public void SetAlphaTo(float alpha)
        {
            foreach (SpriteRenderer spriteRenderer in GetComponentsInChildren<SpriteRenderer>())
            {
                Color c = spriteRenderer.color;
                spriteRenderer.color = new Color(c.r, c.g, c.b, alpha);
            }
            
            foreach (MeshRenderer meshRenderer in GetComponentsInChildren<MeshRenderer>())
            {
                Material material = meshRenderer.material;
                Color c = material.color;
                material.color = new Color(c.r, c.g, c.b, alpha);
            }
        }

        public BodyPartSlot GetNextEmptySlot(Vector3 position)
        {
            return Body.GetNextEmptySlotTo(position);
        }
    }
}
