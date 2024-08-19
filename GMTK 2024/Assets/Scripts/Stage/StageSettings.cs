using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "StageSettings", menuName = "Game/StageSettings", order = 1)]
    public class StageSettings : ScriptableObject 
    {
        [field: SerializeField]
        public int FoodToCollect { get; private set; }

        [field: SerializeField] 
        public float CameraSize { get; private set; } = 10f;

        [field: SerializeField] 
        public float CharacterSizeFactor { get; private set; } = 3f;

        [field: SerializeField] 
        public float EditorCameraSize { get; private set; } = 5f;

        [field: SerializeField]
        public float SpeedFactor { get; private set; } = 1f;
        
        [field: SerializeField]
        public string LevelToLoad { get; private set; }
    }
}
