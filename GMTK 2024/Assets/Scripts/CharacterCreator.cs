using System;
using UnityEngine;

namespace Game
{
    public class CharacterCreator : MonoBehaviour
    {
        [SerializeField] private Camera _camera;

        [SerializeField] private MovementSettings _movementSettings;

        private Game _game;

        private void Awake()
        {
            _game = FindObjectOfType<Game>();
        }

        private void Start()
        {
            TryCreateCharacter();
            _game.OnStageChanged += OnStageChanged;
        }

        private void OnDestroy()
        {
            _game.OnStageChanged -= OnStageChanged;
        }

        private void OnStageChanged()
        {
            TryCreateCharacter();
        }

        private void TryCreateCharacter()
        {
            if (_game.CurrentStage != null)
            {
                Character character = Instantiate(_game.CurrentStage.Character, Vector3.zero, Quaternion.identity);
                character.gameObject.SetActive(true);
                CharacterMovement characterMovement = character.gameObject.AddComponent<CharacterMovement>();
                character.gameObject.AddComponent<FoodCollector>();
                characterMovement.Init(character.GetComponent<Creature>(), _camera, _movementSettings);
                _game.CurrentCharacter = character;
            }
        }
    }
}