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
    }
}
