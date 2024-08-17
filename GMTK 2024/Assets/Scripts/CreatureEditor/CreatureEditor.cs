using UnityEngine;

namespace Game
{
    public class CreatureEditor : MonoBehaviour
    {
        private Game _game;
        private Character _character;
        private CollectedFood _collectedFood;

        private void Awake()
        {
            _game = FindObjectOfType<Game>();
            _collectedFood = FindObjectOfType<CollectedFood>();
        }

        private void Start()
        {
            TryInitEditing();
        }

        private void OnDestroy()
        {
            _game.OnCurrentCharacterChanged -= OnCurrentCharacterChanged;
            
            if (_character != null)
            {
                foreach (BodyPart bodyPart in _character.Creature.BodyParts)
                {
                    bodyPart.OnRightClick -= OnBodyPartRightClick;
                }
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
            Destroy(bodyPart.gameObject);
        }

        private void InitEditing()
        {
            foreach (BodyPart bodyPart in _character.Creature.BodyParts)
            {
                bodyPart.OnRightClick += OnBodyPartRightClick;
            }
        }
    }
}
