using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class StageProgressor : MonoBehaviour
    {
        [SerializeField]
        private GameStagesSettings _gameStages;

        [SerializeField] 
        private Creature _defaultCreature;

        [SerializeField] 
        private BasicSceneLoader _creditsLoader;

        private CollectedFood _collectedFood;
        private Game _game;
        private int _currentStage = 0;
        private int _minFood = 0;
        private Camera _camera;

        private void Awake()
        {
            _camera = FindObjectOfType<MainCamera>().Camera;
            _collectedFood = FindObjectOfType<CollectedFood>();
            _game = Game.Instance;
        }

        private void Start()
        {
            StageSettings settings = _gameStages.Get(_currentStage);
            Creature creature = Instantiate(_defaultCreature, Vector3.zero, Quaternion.identity);
            int health = (int)(settings.HealthBaseValue * creature.Body.BodyData.Splines.Sum(s => s.Size));
            creature.UpdateMaxHealth(health);
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
            float progress = (float)(_collectedFood.TotalCollected - _minFood) / amountToCollect;
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
            if (_gameStages.TryGet(_currentStage, out StageSettings stageSettings))
            {
                SetStage(stage, new Stage(_currentStage, stageSettings, stage.EvolvedCharacter));
                SceneManager.LoadScene("Game", LoadSceneMode.Additive);
                SceneManager.LoadScene(stageSettings.LevelToLoad, LoadSceneMode.Additive);
            }
            else
            {
                SetStage(stage, new Stage(_currentStage, _gameStages.FinalStage, stage.EvolvedCharacter));
                _creditsLoader.Load();
            }
            PlayerPrefs.SetInt("Stage", _currentStage);
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
