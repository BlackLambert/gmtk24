using System;
using UnityEngine;

namespace Game
{
    public class CreatureEditorOnStageFinishedLoader : SceneLoader
    {
        private Game _game;
        private Stage _currentStage;

        private void Awake()
        {
            _game = FindObjectOfType<Game>();
        }
        
        private void Start()
        {
            _game.OnStageChanged += OnStageChanged;
            _currentStage = _game.CurrentStage;
            if (_currentStage != null)
            {
                _currentStage.OnFinishedChanged += OnStageFinished;
            }
        }

        private void OnDestroy()
        {
            _game.OnStageChanged -= OnStageChanged;
            if (_currentStage != null)
            {
                _currentStage.OnProgressChanged -= OnStageFinished;
            }
        }

        private void OnStageChanged()
        {
            if (_currentStage != null)
            {
                _currentStage.OnProgressChanged -= OnStageFinished;
            }
            _currentStage = _game.CurrentStage;
            if (_currentStage != null)
            {
                CheckFinished();
                _currentStage.OnProgressChanged += OnStageFinished;
            }
        }

        private void OnStageFinished()
        {
            CheckFinished();
        }

        private void CheckFinished()
        {
            if (_currentStage.Finished && _game.State != GameState.CreatureEditor)
            {
                _game.CurrentCharacter.gameObject.SetActive(false);
                _game.CurrentCharacter = null;
                _game.State = GameState.CreatureEditor;
                _game.Paused = true;
                DoLoading();
            }
        }
    }
}
