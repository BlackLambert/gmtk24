using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    public class Body : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public event Action<BodyPart> OnBodyPartAdded;
        public event Action<BodyPart> OnBodyPartRemoved;

        public event Action<SplineData> OnPointerStay;
        public event Action OnPointerLeft;
        public event Action OnUpdateMesh;
        
        [SerializeField] private MeshFilter _meshFilter;
        
        private BodyPartSlot[] _slots;
        
        private readonly Dictionary<BodyPartSlot, BodyPart> _slotToBodyPart = new();
        private readonly Dictionary<BodyPart, BodyPartSlot> _bodyPartToSlot = new();

        [SerializeField, HideInInspector] 
        private List<BodyPart> _bodyParts = new List<BodyPart>();

        public IEnumerable<BodyPart> BodyParts => GetBodyParts();
        
        [field: SerializeField, HideInInspector]
        public BodyData BodyData { get; set; }
        
        private List<BodyPart> _pendingBodyParts = new List<BodyPart>();
        private SplineData _nextSpine;
        private bool _updateNextSpine;
        private Camera _camera;

        private void Awake()
        {
            _camera = FindObjectOfType<MainCamera>().Camera;
        }

        private void Update()
        {
            if (_updateNextSpine)
            {
                OnPointerStay?.Invoke(GetNextSpline());
            }
        }

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

        public void UpdateMesh()
        {
            OnUpdateMesh?.Invoke();
        }

        public void UpdateSlots(BodyPartSlot[] slots)
        {
            _slots = slots;
            
            foreach (BodyPart bodyPart in BodyParts)
            {
                Update(bodyPart, GetNextEmptySlotTo(bodyPart.transform.position));
            }
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

            foreach (BodyPart bodyPart in BodyParts)
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

        private void Update(BodyPart bodyPart, BodyPartSlot slot)
        {
            if (_bodyPartToSlot.Remove(bodyPart, out BodyPartSlot formerSlot))
            {
                _slotToBodyPart.Remove(formerSlot);
            }
            
            AddInternal(bodyPart, slot);
        }

        private void AddInternal(BodyPart bodyPart, BodyPartSlot slot)
        {
            if (_slotToBodyPart.ContainsKey(slot))
            {
                throw new ArgumentException();
            }

            if (!_bodyParts.Contains(bodyPart))
            {
                _bodyParts.Add(bodyPart);
            }
            _slotToBodyPart[slot] = bodyPart;
            _bodyPartToSlot[bodyPart] = slot;
            
            Transform bodyPartTransform = bodyPart.transform;
            bodyPartTransform.position = slot.Position * transform.lossyScale.x + transform.position;
            bodyPartTransform.rotation = slot.Rotation;
        }

        public void Remove(BodyPart bodyPart)
        {
            BodyPartSlot slot = _bodyPartToSlot[bodyPart];
            _slotToBodyPart.Remove(slot);
            _bodyPartToSlot.Remove(bodyPart);
            _bodyParts.Remove(bodyPart);
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

        public void OnPointerEnter(PointerEventData eventData)
        {
            _updateNextSpine = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _updateNextSpine = false;
            OnPointerLeft?.Invoke();
        }

        private SplineData GetNextSpline()
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3 mouseWorldPos = _camera.ScreenToWorldPoint(mousePos);
            SplineData result = null;
            float bestSqrDistance = float.MaxValue;

            foreach (SplineData spline in BodyData.Splines)
            {
                Vector3 pos = (Vector3)spline.Center + transform.position;
                Vector3 distance = mouseWorldPos - pos;
                float sqrDistance = distance.sqrMagnitude;

                if (sqrDistance < bestSqrDistance)
                {
                    bestSqrDistance = sqrDistance;
                    result = spline;
                }
            }

            return result;
        }
    }
}