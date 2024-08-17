using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Creature : MonoBehaviour
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
                if (!_bodyParts.Contains(bodyPart))
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
    }
}
