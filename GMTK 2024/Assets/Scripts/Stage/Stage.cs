using System;

namespace Game
{
    public class Stage
    {
        public event Action OnProgressChanged;
        public event Action OnFinishedChanged;
        public event Action OnEvolvedChanged;
        public int StageIndex { get; private set; }
        public StageSettings StageSettings { get; private set; }

        public float Progress
        {
            get => _progress;
            set
            {
                _progress = value;
                OnProgressChanged?.Invoke();
            }
        }

        public bool Finished
        {
            get => _finished;
            set
            {
                _finished = value;
                OnFinishedChanged?.Invoke();
            }
        }

        public bool Evolved
        {
            get => _evolved;
            set
            {
                _evolved = value;
                OnEvolvedChanged?.Invoke();
            }
        }

        private float _progress;
        private bool _finished;
        private bool _evolved;
        
        public Stage(int stageIndex, StageSettings stageSettings)
        {
            StageIndex = stageIndex;
            Progress = 0;
            StageSettings = stageSettings;
        }
    }
}
