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

        private Dictionary<SplineData, List<BodyPartSlot>> _slots;

        private readonly Dictionary<BodyPartSlot, BodyPart> _slotToBodyPart = new();
        private readonly Dictionary<BodyPart, BodyPartSlot[]> _bodyPartToSlot = new();

        [SerializeField, HideInInspector] private List<BodyPart> _bodyParts = new List<BodyPart>();

        public IEnumerable<BodyPart> BodyParts => GetBodyParts();

        [field: SerializeField, HideInInspector]
        public BodyData BodyData { get; set; }

        [SerializeField] private BodySettings _bodySettings;

        private List<BodyPart> _pendingBodyParts = new List<BodyPart>();
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

        public KeyValuePair<SplineData, BodyPartSlot> GetNextEmptySlotTo(Vector3 point, BodyPartSlotType typeFlags)
        {
            KeyValuePair<SplineData, BodyPartSlot> result = new KeyValuePair<SplineData, BodyPartSlot>();
            float bestSqrDistance = float.MaxValue;

            foreach (KeyValuePair<SplineData,List<BodyPartSlot>> pair in _slots)
            {
                foreach (BodyPartSlot slot in pair.Value)
                {
                    float sqrDistance = (point - slot.Position + transform.position).sqrMagnitude;
                    bool isCorrectType = (int)typeFlags == 0 || (typeFlags & slot.Type) > 0;
                    if (isCorrectType && sqrDistance < bestSqrDistance && !_slotToBodyPart.ContainsKey(slot))
                    {
                        bestSqrDistance = sqrDistance;
                        result = new KeyValuePair<SplineData, BodyPartSlot>(pair.Key, slot);
                    }
                }
            }

            return result;
        }

        public void UpdateMesh()
        {
            OnUpdateMesh?.Invoke();
        }

        public void UpdateSlots(Dictionary<SplineData, List<BodyPartSlot>> slots)
        {
            _slots = slots;

            List<BodyPart> leftOverBodyParts = new List<BodyPart>();
            foreach (BodyPart bodyPart in BodyParts.ToList())
            {
                KeyValuePair<SplineData, BodyPartSlot> slot = GetNextEmptySlotTo(bodyPart.transform.position, bodyPart.BodyPartSettings.SlotType);
                if (slot.Value != null)
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

        public void Add(BodyPart bodyPart, KeyValuePair<SplineData, BodyPartSlot> slot)
        {
            AddInternal(bodyPart, slot);
            OnBodyPartAdded?.Invoke(bodyPart);
        }

        private void Update(BodyPart bodyPart, KeyValuePair<SplineData, BodyPartSlot> targetSlot)
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

        private void AddInternal(BodyPart bodyPart, KeyValuePair<SplineData, BodyPartSlot> targetSlot)
        {
            if (_slotToBodyPart.ContainsKey(targetSlot.Value))
            {
                throw new ArgumentException();
            }

            if (!_bodyParts.Contains(bodyPart))
            {
                _bodyParts.Add(bodyPart);
            }

            foreach (Transform child in GetComponentsInChildren<Transform>())
            {
                child.gameObject.layer = 6;
            }

            bool needsCounterPart = bodyPart.BodyPartSettings.NeedsCounterPartSlot;
            BodyPartSlot[] slots = needsCounterPart
                ? new[] { targetSlot.Value, _slots[targetSlot.Key][targetSlot.Value.CounterPartIndex] }
                : new[] { targetSlot.Value };

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

        private Vector3 GetBodyPartPosition(BodyPart bodyPart, KeyValuePair<SplineData, BodyPartSlot> splineToSlot)
        {
            bool needsCounterPart = bodyPart.BodyPartSettings.NeedsCounterPartSlot;
            Transform trans = transform;
            BodyPartSlot slot = splineToSlot.Value;
            Vector3 slotPos = needsCounterPart
                ? slot.Position + (_slots[splineToSlot.Key][slot.CounterPartIndex].Position - splineToSlot.Value.Position) / 2
                : slot.Position;
            return slotPos * trans.lossyScale.x + trans.position + new Vector3(0, 0, 0.1f);
        }

        public void SnapTo(BodyPart bodyPart, KeyValuePair<SplineData, BodyPartSlot> targetSlot)
        {
            Transform bodyPartTransform = bodyPart.transform;
            bodyPartTransform.position = GetBodyPartPosition(bodyPart, targetSlot);
            bodyPartTransform.rotation = targetSlot.Value.Rotation;
            bodyPartTransform.localScale = Vector3.one + Vector3.one * (targetSlot.Key.Size * bodyPart.BodyPartSettings.ScalePerSizeAddition);
        }
    }
}