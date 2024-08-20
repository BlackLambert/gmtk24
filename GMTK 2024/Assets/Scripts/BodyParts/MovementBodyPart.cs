using UnityEngine;

namespace Game
{
    public class MovementBodyPart : MonoBehaviour
    {
        [field: SerializeField]
        public MovementSettings MovementSettings { get; private set; }
    }
}
