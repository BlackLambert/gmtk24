using System;
using UnityEngine;

namespace Game
{
    public class Game : MonoBehaviour
    {
        private static Game _instance;

        public static Game Instance
        {
            get
            {
                _instance ??= new GameObject("Game").AddComponent<Game>();
                return _instance;
            }
        }

        [HideInInspector] public bool Paused = false;

        private Stage _stage;
        public event Action OnStageChanged;

        public Stage CurrentStage
        {
            get => _stage;
            set
            {
                _stage = value;
                OnStageChanged?.Invoke();
            }
        }

        private GameState _state = GameState.InGame;
        public event Action OnGameStateChanged;

        public GameState State
        {
            get => _state;
            set
            {
                _state = value;
                OnGameStateChanged?.Invoke();
            }
        }

        private Character _currentCharacter;
        public event Action OnCurrentCharacterChanged;
        
        public Character CurrentCharacter
        {
            get => _currentCharacter;
            set
            {
                _currentCharacter = value;
                OnCurrentCharacterChanged?.Invoke();
            }
        }

        public Stage FormerStage { get; set; }
    }
}