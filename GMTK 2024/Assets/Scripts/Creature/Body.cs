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
        public event Action<List<BodyPart>> OnLeftOverBodyParts;

        [SerializeField] private MeshFilter _meshFilter;

        private BodyPartSlot[] _slots;

        private readonly Dictionary<BodyPartSlot, BodyPart> _slotToBodyPart = new();
        private readonly Dictionary<BodyPart, BodyPartSlot[]> _bodyPartToSlot = new();

        [SerializeField, HideInInspector] private List<BodyPart> _bodyParts = new List<BodyPart>();

        public IEnumerable<BodyPart> BodyParts => GetBodyParts();

        [field: SerializeField, HideInInspector]
        public BodyData BodyData { get; set; }

        [SerializeField] private BodySettings _bodySettings;

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

        public BodyPartSlot GetNextEmptySlotTo(Vector3 point, BodyPartSlotType typeFlags)
        {
            BodyPartSlot result = null;
            float bestSqrDistance = float.MaxValue;

            foreach (BodyPartSlot slot in _slots)
            {
                float sqrDistance = (point - slot.Position + transform.position).sqrMagnitude;
                bool isCorrectType = (int)typeFlags == 0 || (typeFlags & slot.Type) > 0;
                if (isCorrectType && sqrDistance < bestSqrDistance && !_slotToBodyPart.ContainsKey(slot))
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

            List<BodyPart> leftOverBodyParts = new List<BodyPart>();
            foreach (BodyPart bodyPart in BodyParts.ToList())
            {
                BodyPartSlot slot = GetNextEmptySlotTo(bodyPart.transform.position, bodyPart.BodyPartSettings.SlotType);
                if (slot != null)
                {
                    Update(bodyPart, slot);
                }
                else
                {
                    Remove(bodyPart);
                    leftOverBodyParts.Add(bodyPart);
                }
            }

            if (leftOverBodyParts.Count > 0)
            {
                OnLeftOverBodyParts?.Invoke(leftOverBodyParts);
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
                Add(bodyPart, GetNextEmptySlotTo(bodyPart.transform.position, bodyPart.BodyPartSettings.SlotType));
            }
        }

        public void Add(BodyPart bodyPart)
        {
            if (_slots != null)
            {
                Add(bodyPart, GetNextEmptySlotTo(bodyPart.transform.position, bodyPart.BodyPartSettings.SlotType));
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

        private void Update(BodyPart bodyPart, BodyPartSlot targetSlot)
        {
            if (_bodyPartToSlot.Remove(bodyPart, out BodyPartSlot[] formerSlots))
            {
                foreach (BodyPartSlot partSlot in formerSlots)
                {
                    _slotToBodyPart.Remove(partSlot);
                }
            }

            AddInternal(bodyPart, targetSlot);
        }

        private void AddInternal(BodyPart bodyPart, BodyPartSlot targetSlot)
        {
            if (_slotToBodyPart.ContainsKey(targetSlot))
            {
                throw new ArgumentException();
            }

            if (!_bodyParts.Contains(bodyPart))
            {
                _bodyParts.Add(bodyPart);
            }

            bool needsCounterPart = bodyPart.BodyPartSettings.NeedsCounterPartSlot;
            BodyPartSlot[] slots = needsCounterPart
                ? new[] { targetSlot, _slots[targetSlot.CounterPartIndex] }
                : new[] { targetSlot };

            foreach (BodyPartSlot slot in slots)
            {
                _slotToBodyPart[slot] = bodyPart;
            }

            _bodyPartToSlot[bodyPart] = slots;

            SnapTo(bodyPart, targetSlot);
        }

        public void Remove(BodyPart bodyPart)
        {
            BodyPartSlot[] slots = _bodyPartToSlot[bodyPart];
            foreach (BodyPartSlot slot in slots)
            {
                _slotToBodyPart.Remove(slot);
            }

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

        public SplineData RemoveSpline()
        {
            int index = BodyData.Splines.Count - 1;
            SplineData splineData = BodyData.Splines[index];
            BodyData.Splines.Remove(splineData);
            UpdateSplinesCenters();
            UpdateMesh();
            return splineData;
        }

        public void AddSpline()
        {
            SplineData newSplineData = new SplineData { Size = 1, Center = new Vector2(0, 0) };
            BodyData.Splines.Add(newSplineData);
            UpdateSplinesCenters();
            UpdateMesh();
        }

        private void UpdateSplinesCenters()
        {
            float totalLength = _bodySettings.SplineSpacing * (BodyData.Splines.Count - 1);
            float halfLength = totalLength / 2;

            for (int i = 0; i < BodyData.Splines.Count; i++)
            {
                SplineData splineData = BodyData.Splines[i];
                splineData.Center = new Vector2(0, -halfLength + _bodySettings.SplineSpacing * i);
            }
        }

        private Vector3 GetBodyPartPosition(BodyPart bodyPart, BodyPartSlot slot)
        {
            bool needsCounterPart = bodyPart.BodyPartSettings.NeedsCounterPartSlot;
            Transform trans = transform;
            Vector3 slotPos = needsCounterPart
                ? (_slots[slot.CounterPartIndex].Position - slot.Position) / 2
                : slot.Position;
            return slotPos * trans.lossyScale.x + trans.position + new Vector3(0, 0, 0.1f);
        }

        public void SnapTo(BodyPart bodyPart, BodyPartSlot targetSlot)
        {
            Transform bodyPartTransform = bodyPart.transform;
            bodyPartTransform.position = GetBodyPartPosition(bodyPart, targetSlot);
            bodyPartTransform.rotation = targetSlot.Rotation;
        }
    }
}