using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class StageProgressDisplay : MonoBehaviour
    {
        [SerializeField] 
        private Slider _slider;

        private Game _game;
        private Stage _currentStage;

        private void Awake()
        {
            _game = Game.Instance;
        }

        private void Start()
        {
            _game.OnStageChanged += OnStageChanged;
            _currentStage = _game.CurrentStage;
            if (_currentStage != null)
            {
                _currentStage.OnProgressChanged += OnProgressChanged;
            }
        }

        private void OnDestroy()
        {
            _game.OnStageChanged -= OnStageChanged;
            if (_currentStage != null)
            {
                _currentStage.OnProgressChanged -= OnProgressChanged;
            }
        }

        private void OnStageChanged()
        {
            if (_currentStage != null)
            {
                _currentStage.OnProgressChanged -= OnProgressChanged;
            }
            _currentStage = _game.CurrentStage;
            if (_currentStage != null)
            {
                _currentStage.OnProgressChanged += OnProgressChanged;
            }
        }

        private void OnProgressChanged()
        {
            UpdateProgress();
        }

        private void UpdateProgress()
        {
            _slider.value = _currentStage.Progress;
        }
    }
}
