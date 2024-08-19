using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "MovementSettings", menuName = "Game/MovementSettings", order = 1)]
    public class MovementSettings : ScriptableObject 
    {
        [field: SerializeField] 
        public float Force { get; private set; } = 10;

        [field: SerializeField] 
        public float MaxSpeed { get; private set; } = 2;

        [field: SerializeField] 
        public float Cooldown { get; private set; } = 0;

        [field: SerializeField] 
        public float RotationSpeed { get; private set; } = 300;
    }
}
