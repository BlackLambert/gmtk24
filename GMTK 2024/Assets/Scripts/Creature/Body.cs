using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Body : MonoBehaviour
    {
        public event Action<BodyPart> OnBodyPartAdded;
        public event Action<BodyPart> OnBodyPartRemoved;
        
        [SerializeField] private MeshFilter _meshFilter;
        
        private BodyPartSlot[] _slots;
        
        private readonly Dictionary<BodyPartSlot, BodyPart> _slotToBodyPart = new Dictionary<BodyPartSlot, BodyPart>();
        private readonly Dictionary<BodyPart, BodyPartSlot> _bodyPartToSlot = new Dictionary<BodyPart, BodyPartSlot>();

        [SerializeField, HideInInspector] 
        private List<BodyPart> _bodyParts = new List<BodyPart>();

        public IEnumerable<BodyPart> BodyParts => GetBodyParts();
        
        private List<BodyPart> _pendingBodyParts = new List<BodyPart>();

        public BodyPartSlot GetNextEmptySlotTo(Vector3 point)
        {
            BodyPartSlot result = null;
            float bestSqrDistance = float.MaxValue;

            foreach (BodyPartSlot slot in _slots)
            {
                float sqrDistance = (point - slot.Position + transform.position).sqrMagnitude;
                if (sqrDistance < bestSqrDistance && !_slotToBodyPart.ContainsKey(slot))
                {
                    bestSqrDistance = sqrDistance;
                    result = slot;
                }
            }

            return result;
        }

        public void UpdateSlots()
        {
            Mesh mesh = _meshFilter.mesh;
            Vector3[] vertices = mesh.vertices;
            Vector3[] normals = mesh.normals;
            _slots = new BodyPartSlot[vertices.Length];

            for (int i = 0; i < vertices.Length; i++)
            {
                float angle = Vector2.Angle(normals[i], Vector2.up);
                float factor = vertices[i].x <= 0 ? 1 : -1;
                Quaternion rotation = Quaternion.Euler(0, 0, angle * factor);
                _slots[i] = new BodyPartSlot()
                {
                    Position = vertices[i],
                    Rotation = rotation,
                    VertexIndex = i
                };
            }

            foreach (BodyPart bodyPart in _pendingBodyParts)
            {
                Add(bodyPart, GetNextEmptySlotTo(bodyPart.transform.position));
            }
        }

        public void Add(BodyPart bodyPart)
        {
            if (_slots != null)
            {
                Add(bodyPart, GetNextEmptySlotTo(bodyPart.transform.position));
            }
            else
            {
                _pendingBodyParts.Add(bodyPart);
                OnBodyPartAdded?.Invoke(bodyPart);
            }
        }

        public void Add(BodyPart bodyPart, BodyPartSlot slot)
        {
            AddInternal(bodyPart, slot);
            OnBodyPartAdded?.Invoke(bodyPart);
        }

        private void AddInternal(BodyPart bodyPart, BodyPartSlot slot)
        {
            if (_slotToBodyPart.ContainsKey(slot))
            {
                throw new ArgumentException();
            }

            _slotToBodyPart[slot] = bodyPart;
            _bodyPartToSlot[bodyPart] = slot;
        }

        public void Remove(BodyPart bodyPart)
        {
            BodyPartSlot slot = _bodyPartToSlot[bodyPart];
            _slotToBodyPart.Remove(slot);
            _bodyPartToSlot.Remove(bodyPart);
            OnBodyPartRemoved?.Invoke(bodyPart);
        }

        public bool Contains(BodyPart bodyPart)
        {
            return _bodyParts.Contains(bodyPart);
        }

        private IEnumerable<BodyPart> GetBodyParts()
        {
            foreach (BodyPart bodyPart in _bodyParts)
            {
                yield return bodyPart;
            }

            foreach (BodyPart bodyPart in _pendingBodyParts)
            {
                yield return bodyPart;
            }
        }
    }
}