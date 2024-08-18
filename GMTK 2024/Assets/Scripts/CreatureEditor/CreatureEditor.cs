using UnityEngine;

namespace Game
{
    public class CreatureEditor : MonoBehaviour
    {
        [SerializeField] 
        private BodyPartsTabContent[] _tabContent;

        [SerializeField] 
        private float _snapDistance = 10f;
        
        private Game _game;
        private Character _character;
        private CollectedFood _collectedFood;
        private Camera _camera;
        private FoodParticleAnimationFactory _particleAnimationFactory;

        private void Awake()
        {
            _game = FindObjectOfType<Game>();
            _collectedFood = FindObjectOfType<CollectedFood>();
            _camera = FindObjectOfType<MainCamera>().Camera;
            _particleAnimationFactory = FindObjectOfType<FoodParticleAnimationFactory>();
        }

        private void Start()
        {
            TryInitEditing();
            FindObjectOfType<MainCamera>().Camera.orthographicSize = _game.CurrentStage.StageSettings.EditorCameraSize;

            foreach (BodyPartsTabContent tabContent in _tabContent)
            {
                tabContent.OnBodyPartButtonDragExit += OnTryCreateBodyPart;
            }
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
                }
            
                _character.Creature.Body.OnBodyPartAdded -= OnBodyPartAdded;
                _character.Creature.Body.OnBodyPartRemoved -= OnBodyPartRemoved;
            }

            foreach (BodyPartsTabContent tabContent in _tabContent)
            {
                tabContent.OnBodyPartButtonDragExit -= OnTryCreateBodyPart;
            }
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
            }

            _character.Creature.Body.OnBodyPartAdded += OnBodyPartAdded;
            _character.Creature.Body.OnBodyPartRemoved += OnBodyPartRemoved;
        }

        private void OnBodyPartRemoved(BodyPart bodyPart)
        {
            bodyPart.OnRightClick -= OnBodyPartRightClick;
            bodyPart.OnDragStart -= OnBodyPartDrag;
        }

        private void OnBodyPartAdded(BodyPart bodyPart)
        {
            bodyPart.OnRightClick += OnBodyPartRightClick;
            bodyPart.OnDragStart += OnBodyPartDrag;
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
            placer.Init(bodyPartInstance, _game.CurrentCharacter.Creature, followCursor, _snapDistance, true);
            bodyPartInstance.EnableColliders(false);
        }
        
        private void OnBodyPartDrag(BodyPart bodyPart)
        {
            _character.Creature.Remove(bodyPart);
            FollowCursor followCursor = bodyPart.gameObject.AddComponent<FollowCursor>();
            BodyPartPlacer placer = bodyPart.gameObject.AddComponent<BodyPartPlacer>();
            placer.Init(bodyPart, _game.CurrentCharacter.Creature, followCursor, _snapDistance, false);
            bodyPart.EnableColliders(false);
        }
    }
}
