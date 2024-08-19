using UnityEngine;

namespace Game
{
    public class BodyPartPlacer : MonoBehaviour
    {
        private BodyPart _bodyPart;
        private Creature _creature;
        private FollowCursor _followCursor;
        private float _snapDistance;
        private bool _payCosts;
        private bool _getSellFood;

        private FoodParticleAnimationFactory _foodParticleAnimationFactory;
        private CollectedFood _collectedFood;
        private Camera _camera;

        private BodyPartSlot _currentSlot = null;

        private void Awake()
        {
            _foodParticleAnimationFactory = FindObjectOfType<FoodParticleAnimationFactory>();
            _collectedFood = FindObjectOfType<CollectedFood>();
            _camera = FindObjectOfType<MainCamera>().Camera;
        }

        public void Init(BodyPart bodyPart, Creature creature, FollowCursor followCursor, float snapDistance,
            bool payCosts, bool getSellFood, bool applyScale)
        {
            if (applyScale)
            {
                bodyPart.transform.localScale = creature.transform.localScale;
            }
            _bodyPart = bodyPart;
            _creature = creature;
            _followCursor = followCursor;
            _snapDistance = snapDistance;
            _payCosts = payCosts;
            _getSellFood = getSellFood;
        }

        private void Update()
        {
            Vector3 creatureScreenPosition = _camera.WorldToScreenPoint(_creature.transform.position);
            Vector3 mousePosition = Input.mousePosition;
            Vector2 distance = new Vector2(creatureScreenPosition.x, creatureScreenPosition.y) -
                               new Vector2(mousePosition.x, mousePosition.y);
            bool isSnapDistance = distance.magnitude <= _snapDistance;

            if (isSnapDistance)
            {
                Snap();
            }
            
            _followCursor.enabled = !isSnapDistance || _currentSlot == null;

            if (Input.GetMouseButtonUp(0))
            {
                if (!isSnapDistance || _currentSlot == null)
                {
                    Sell();
                }
                else
                {
                    Place();
                }
            }
        }

        private void Sell()
        {
            if (_getSellFood)
            {
                _collectedFood.Add(_bodyPart.BodyPartSettings.Costs);
                foreach (FoodAmount foodAmount in _bodyPart.BodyPartSettings.Costs)
                {
                    _foodParticleAnimationFactory.Create(foodAmount,
                        _camera.WorldToScreenPoint(_bodyPart.transform.position));
                }
            }
            Destroy(gameObject);
        }

        private void Snap()
        {
            Vector2 worldMousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
            _currentSlot = _creature.GetNextEmptySlot(worldMousePos);
            
            if (_currentSlot != null)
            {
                Transform bodyPartTransform = _bodyPart.transform;
                bodyPartTransform.position = (_currentSlot.Position * _creature.transform.localScale.x +
                                              _creature.Body.transform.position);
                bodyPartTransform.rotation = _currentSlot.Rotation;
            }
        }

        private void Place()
        {
            _creature.Add(_bodyPart, _currentSlot);
            _bodyPart.EnableColliders(true);
            Destroy(_followCursor);
            Destroy(this);

            if (_payCosts)
            {
                _collectedFood.ChangeBy(_bodyPart.BodyPartSettings.Costs);
                foreach (FoodAmount foodAmount in _bodyPart.BodyPartSettings.Costs)
                {
                    _foodParticleAnimationFactory.Create(foodAmount,
                        _camera.WorldToScreenPoint(_bodyPart.transform.position), true);
                }
            }
        }
    }
}