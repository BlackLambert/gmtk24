using UnityEngine;

namespace Game
{
    public class CharacterCreator : MonoBehaviour
    {
        [SerializeField] 
        private float _scalesAlpha = 0.2f;
        [SerializeField] 
        private MovementSettings _baseMovement;

        private Game _game;

        private void Awake()
        {
            _game = Game.Instance;
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
                Vector3 pos = new Vector3(0, 0, -_game.CurrentStage.StageIndex * 0.5f - 3.1f);
                Character character = Instantiate(_game.CurrentStage.Character, pos, Quaternion.identity);
                character.transform.localScale *= _game.CurrentStage.StageSettings.CharacterSizeFactor;
                character.name = $"Character{_game.CurrentStage.StageIndex}";
                character.gameObject.SetActive(true);
                CharacterMovement characterMovement = character.gameObject.AddComponent<CharacterMovement>();
                character.gameObject.AddComponent<FoodCollector>();
                characterMovement.Init(character.GetComponent<Creature>(), _baseMovement);
                _game.CurrentCharacter = character;
            }

            if (_game.FormerStage != null)
            {
                Vector3 pos = new Vector3(0, 0, -_game.FormerStage.StageIndex * 0.5f - 3);
                Character formerCharacter = Instantiate(_game.FormerStage.Character, pos, Quaternion.identity);
                FormerCharacterHook hook = FindObjectOfType<FormerCharacterHook>();
                formerCharacter.transform.localScale *= _game.FormerStage.StageSettings.CharacterSizeFactor;
                formerCharacter.transform.SetParent(hook.transform);
                formerCharacter.name = $"FormerCharacter{_game.FormerStage.StageIndex}";
                formerCharacter.Creature.DisableCollider();
                formerCharacter.Creature.SetAlphaTo(_scalesAlpha);
                formerCharacter.gameObject.SetActive(true);
                foreach (Animator animator in formerCharacter.GetComponentsInChildren<Animator>())
                {
                    animator.enabled = false;
                }
            }
        }
    }
}