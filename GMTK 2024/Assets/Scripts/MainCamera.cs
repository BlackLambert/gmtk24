using UnityEngine;

namespace Game
{
    public class MainCamera : MonoBehaviour
    {
        [field:SerializeField]
        public Camera Camera { get; private set; }
    }
}
