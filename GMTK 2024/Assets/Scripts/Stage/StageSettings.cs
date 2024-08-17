using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "StageSettings", menuName = "Game/StageSettings", order = 1)]
    public class StageSettings : ScriptableObject 
    {
        [field: SerializeField]
        public int FoodToCollect { get; private set; }
    }
}
