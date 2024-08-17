using System;
using UnityEngine;

namespace Game
{
    public class Game : MonoBehaviour
    {
        [HideInInspector]
        public bool Paused = false;

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
    }
}
