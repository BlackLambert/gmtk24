using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class CreatureEditor : MonoBehaviour
    {
        [SerializeField] private BodyPartsTabContent[] _tabContent;

        [SerializeField] private float _snapDistance = 10f;

        [SerializeField] private BodySplineIndicator _splineIndicator;

        [SerializeField] private BodySettings _bodySettings;

        [SerializeField] private Button _addSplineButton;

        [SerializeField] private Button _removeSplineButton;

        private Game _game;
        private Character _character;
        private CollectedFood _collectedFood;
        private Camera _camera;
        private FoodParticleAnimationFactory _particleAnimationFactory;
        private SplineData _hoveredSpline;

        private void Awake()
        {
            _game = FindObjectOfType<Game>();
            _collectedFood = FindObjectOfType<CollectedFood>();
            _camera = FindObjectOfType<MainCamera>().Camera;
            _particleAnimationFactory = FindObjectOfType<FoodParticleAnimationFactory>();
            _splineIndicator.gameObject.SetActive(false);
        }

        private void Start()
        {
            TryInitEditing();
            FindObjectOfType<MainCamera>().Camera.orthographicSize = _game.CurrentStage.StageSettings.EditorCameraSize;

            foreach (BodyPartsTabContent tabContent in _tabContent)
            {
                tabContent.OnBodyPartButtonDragExit += OnTryCreateBodyPart;
            }
            
            _addSplineButton.onClick.AddListener(AddSpline);
            _removeSplineButton.onClick.AddListener(RemoveSpline);
        }

        private void OnDestroy()
        {
            _game.OnCurrentCharacterChanged -= OnCurrentCharacterChanged;

            if (_character != null)
            {
                foreach (BodyPart bodyPart in _character.Creature.Body.BodyParts)
                {
                    bodyPart.OnRightClick -= OnBodyPartRightClick;
                    bodyPart.OnDragStart -= OnBodyPartDrag;
                    bodyPart.OnMouseHoverStart -= OnBodyPartHover;
                    bodyPart.OnMouseHoverEnd -= OnBodyPartHoverEnd;
                }

                _character.Creature.Body.OnBodyPartAdded -= OnBodyPartAdded;
                _character.Creature.Body.OnBodyPartRemoved -= OnBodyPartRemoved;
                _character.Creature.Body.OnPointerStay -= OnPointerInBody;
                _character.Creature.Body.OnPointerLeft -= HideSplineIndicator;
            }

            foreach (BodyPartsTabContent tabContent in _tabContent)
            {
                tabContent.OnBodyPartButtonDragExit -= OnTryCreateBodyPart;
            }
            
            _addSplineButton.onClick.RemoveListener(AddSpline);
            _removeSplineButton.onClick.RemoveListener(RemoveSpline);
        }

        private void Update()
        {
            DoSplineResize();
        }


        private void TryInitEditing()
        {
            _character = _game.CurrentCharacter;
            if (_character != null)
            {
                InitEditing();
            }
            else
            {
                _game.OnCurrentCharacterChanged += OnCurrentCharacterChanged;
            }
        }

        private void OnCurrentCharacterChanged()
        {
            if (_game.CurrentCharacter != null)
            {
                _character = _game.CurrentCharacter;
                InitEditing();
                _game.OnCurrentCharacterChanged -= OnCurrentCharacterChanged;
            }
        }

        private void OnBodyPartRightClick(BodyPart bodyPart)
        {
            SellBodyPart(bodyPart);
        }

        private void SellBodyPart(BodyPart bodyPart)
        {
            _character.Creature.Remove(bodyPart);
            BodyPartSettings bodyPartSettings = bodyPart.BodyPartSettings;
            _collectedFood.Add(bodyPartSettings.Costs);
            Vector2 screenPos = _camera.WorldToScreenPoint(bodyPart.transform.position);
            foreach (FoodAmount amount in bodyPart.BodyPartSettings.Costs)
            {
                _particleAnimationFactory.Create(amount, screenPos);
            }

            Destroy(bodyPart.gameObject);
        }

        private void InitEditing()
        {
            foreach (BodyPart bodyPart in _character.Creature.Body.BodyParts)
            {
                bodyPart.OnRightClick += OnBodyPartRightClick;
                bodyPart.OnDragStart += OnBodyPartDrag;
                bodyPart.OnMouseHoverStart += OnBodyPartHover;
                bodyPart.OnMouseHoverEnd += OnBodyPartHoverEnd;
            }

            _character.Creature.Body.OnBodyPartAdded += OnBodyPartAdded;
            _character.Creature.Body.OnBodyPartRemoved += OnBodyPartRemoved;
            _character.Creature.Body.OnPointerStay += OnPointerInBody;
            _character.Creature.Body.OnPointerLeft += HideSplineIndicator;
            UpdateSplineButtons();
        }

        private void OnBodyPartRemoved(BodyPart bodyPart)
        {
            bodyPart.OnRightClick -= OnBodyPartRightClick;
            bodyPart.OnDragStart -= OnBodyPartDrag;
            bodyPart.OnMouseHoverStart -= OnBodyPartHover;
            bodyPart.OnMouseHoverEnd -= OnBodyPartHoverEnd;
        }

        private void OnBodyPartAdded(BodyPart bodyPart)
        {
            bodyPart.OnRightClick += OnBodyPartRightClick;
            bodyPart.OnDragStart += OnBodyPartDrag;
            bodyPart.OnMouseHoverStart += OnBodyPartHover;
            bodyPart.OnMouseHoverEnd += OnBodyPartHoverEnd;
        }

        private void OnTryCreateBodyPart(BodyPart bodyPart)
        {
            bool hasFood = true;
            foreach (FoodAmount amount in bodyPart.BodyPartSettings.Costs)
            {
                hasFood = hasFood && _collectedFood.Has(amount);
            }

            if (!hasFood)
            {
                return;
            }

            BodyPart bodyPartInstance = Instantiate(bodyPart);
            FollowCursor followCursor = bodyPartInstance.gameObject.AddComponent<FollowCursor>();
            BodyPartPlacer placer = bodyPartInstance.gameObject.AddComponent<BodyPartPlacer>();
            placer.Init(bodyPartInstance, _game.CurrentCharacter.Creature, followCursor, _snapDistance, true, false,
                true);
            bodyPartInstance.EnableColliders(false);
        }

        private void OnBodyPartDrag(BodyPart bodyPart)
        {
            _character.Creature.Remove(bodyPart);
            FollowCursor followCursor = bodyPart.gameObject.AddComponent<FollowCursor>();
            BodyPartPlacer placer = bodyPart.gameObject.AddComponent<BodyPartPlacer>();
            placer.Init(bodyPart, _game.CurrentCharacter.Creature, followCursor, _snapDistance, false, true, false);
            bodyPart.EnableColliders(false);
            bodyPart.ShowOutline(false);
        }

        private void OnBodyPartHoverEnd(BodyPart bodyPart)
        {
            bodyPart.ShowOutline(false);
        }

        private void OnBodyPartHover(BodyPart bodyPart)
        {
            bodyPart.ShowOutline(true);
        }

        private void OnPointerInBody(SplineData spline)
        {
            _splineIndicator.gameObject.SetActive(true);
            Transform bodyTransform = _character.Creature.Body.transform;
            _splineIndicator.SetPosition(
                _camera.WorldToScreenPoint((Vector3)spline.Center * bodyTransform.lossyScale.x + bodyTransform.position));
            _hoveredSpline = spline;
        }

        private void HideSplineIndicator()
        {
            _hoveredSpline = null;
            _splineIndicator.gameObject.SetActive(false);
        }

        private void DoSplineResize()
        {
            if (_hoveredSpline == null)
            {
                return;
            }

            if (Input.mouseScrollDelta.y == 0)
            {
                return;
            }

            int sizeDelta = Input.mouseScrollDelta.y > 0 ? 1 : -1;

            if (sizeDelta > 0 && !_collectedFood.Has(_bodySettings.Costs))
            {
                return;
            }

            int targetSize = Mathf.Clamp(_hoveredSpline.Size + sizeDelta, 1, _bodySettings.MaxSize);

            if (targetSize == _hoveredSpline.Size)
            {
                return;
            }
            
            Transform bodyTransform = _character.Creature.Body.transform;
            Vector2 point = _camera.WorldToScreenPoint(
                (Vector3)_hoveredSpline.Center * bodyTransform.lossyScale.x + bodyTransform.position);
            
            if (sizeDelta > 0)
            {
                _collectedFood.Remove(_bodySettings.Costs);
                _particleAnimationFactory.Create(_bodySettings.Costs, point, true);
            }
            else
            {
                _collectedFood.Add(_bodySettings.Costs);
                _particleAnimationFactory.Create(_bodySettings.Costs, point);
            }
            
            _hoveredSpline.Size = targetSize;
            _character.Creature.Body.UpdateMesh();
        }

        private void RemoveSpline()
        {
            RemoveSplineResult result = _character.Creature.RemoveSpline();
            foreach (BodyPart bodyPart in result.BodyParts)
            {
                _collectedFood.Add(bodyPart.BodyPartSettings.Costs);
                _particleAnimationFactory.Create(bodyPart.BodyPartSettings.Costs, _camera.WorldToScreenPoint(bodyPart.transform.position));
            }

            foreach (FoodAmount amount in _bodySettings.Costs)
            {
                FoodAmount combinedAmount = new FoodAmount
                    { FoodType = amount.FoodType, Amount = amount.Amount * result.Spline.Size };
                _collectedFood.Add(combinedAmount);
                _particleAnimationFactory.Create(combinedAmount, _camera.WorldToScreenPoint(Vector3.zero));
            }
            UpdateSplineButtons();
        }

        private void AddSpline()
        {
            _collectedFood.Remove(_bodySettings.Costs);
            _particleAnimationFactory.Create(_bodySettings.Costs, _camera.WorldToScreenPoint(Vector3.zero), true);
            _character.Creature.AddSpline();
            UpdateSplineButtons();
        }

        private void UpdateSplineButtons()
        {
            _addSplineButton.interactable = _character.Creature.Body.BodyData.Splines.Count < _bodySettings.MaxSplines 
                                            && _collectedFood.Has(_bodySettings.Costs);
            _removeSplineButton.interactable = _character.Creature.Body.BodyData.Splines.Count > _bodySettings.MinSplines;
        }
        
        
    }
}