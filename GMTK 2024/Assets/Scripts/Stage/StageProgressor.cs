using UnityEngine;

namespace Game
{
    public class StageProgressor : MonoBehaviour
    {
        [SerializeField]
        private GameStagesSettings _gameStages;

        [SerializeField] 
        private Creature _defaultCreature;

        [SerializeField] 
        private Camera _camera;

        private CollectedFood _collectedFood;
        private Game _game;
        private int _currentStage = 0;
        private int _minFood = 0;

        private void Awake()
        {
            _collectedFood = FindObjectOfType<CollectedFood>();
            _game = FindObjectOfType<Game>();
        }

        private void Start()
        {
            StageSettings settings = _gameStages.Get(_currentStage);
            Creature creature = Instantiate(_defaultCreature, Vector3.zero, Quaternion.identity);
            Character character = creature.gameObject.AddComponent<Character>();
            character.Init(creature);
            character.gameObject.SetActive(false);
            SetStage(null, new Stage(_currentStage, settings, character));
            _collectedFood.OnFoodCollected += OnFoodCollected;
        }

        private void OnDestroy()
        {
            _collectedFood.OnFoodCollected -= OnFoodCollected;
            _game.CurrentStage.OnEvolvedChanged -= OnEvolved;
        }

        private void OnFoodCollected(FoodType foodType)
        {
            UpdateProgress();
        }

        private void UpdateProgress()
        {
            Stage stage = _game.CurrentStage;
            int amountToCollect = stage.StageSettings.FoodToCollect;
            float progress = (float)(_collectedFood.TotalCollected - _minFood) / (amountToCollect - _minFood);
            stage.Progress = progress;
            if (progress >= 1 && !stage.Finished)
            {
                stage.Finished = true;
            }
        }

        private void OnEvolved()
        {
            _currentStage++;
            _minFood += _game.CurrentStage.StageSettings.FoodToCollect;
            Stage stage = _game.CurrentStage;
            SetStage(stage, new Stage(_currentStage, _gameStages.Get(_currentStage), stage.EvolvedCharacter));
        }

        private void SetStage(Stage currentStage, Stage nextStage)
        {
            if (currentStage != null)
            {
                currentStage.OnEvolvedChanged -= OnEvolved;
                _game.FormerStage = currentStage;
            }
            
            _game.CurrentStage = nextStage;
            _camera.orthographicSize = nextStage.StageSettings.CameraSize;
            _game.CurrentStage.OnEvolvedChanged += OnEvolved;
            UpdateProgress();
        }
    }
}
