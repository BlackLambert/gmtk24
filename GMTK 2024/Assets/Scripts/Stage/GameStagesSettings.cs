using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "GameStagesSettings", menuName = "Game/GameStagesSettings", order = 1)]
    public class GameStagesSettings : ScriptableObject
    {
        [SerializeField] 
        private StageSettings[] _stages;

        public StageSettings Get(int index)
        {
            return _stages[index];
        }

        public bool TryGet(int index, out StageSettings stageSettings)
        {
            if (index < _stages.Length)
            {
                stageSettings = _stages[index];
                return true;
            }

            stageSettings = null;
            return false;
        }
    }
}
