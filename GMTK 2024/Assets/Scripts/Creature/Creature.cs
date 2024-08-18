using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class Creature : MonoBehaviour, IDamageable
    {
        [field: SerializeField]
        public Rigidbody2D Rigidbody { get; private set; }
        
        [SerializeField] 
        private Body _body;

        [SerializeField] 
        private BodyPart[] _startParts;

        public IReadOnlyList<BodyPart> BodyParts => _bodyParts;
        public Transform Transform { get; private set; }

        [SerializeField, HideInInspector]
        private List<BodyPart> _bodyParts = new List<BodyPart>();

        private void Awake()
        {
            foreach (BodyPart bodyPart in _startParts)
            {
                if (bodyPart != null && !_bodyParts.Contains(bodyPart))
                {
                    _bodyParts.Add(bodyPart);
                }
            }
            Transform = transform;
        }

        public void Add(BodyPart bodyPart)
        {
            _bodyParts.Add(bodyPart);
        }

        public void Remove(BodyPart bodyPart)
        {
            _bodyParts.Remove(bodyPart);
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

        public void SufferDamage(float damage)
        {
            throw new System.NotImplementedException();
        }

        public void ApplyStatusEffect(StatusEffect status)
        {
            throw new System.NotImplementedException();
        }

        public int GetID()
        {
            return 0;
        }
    }
}
