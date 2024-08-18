using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "OutlineColorSettings", menuName = "Game/OutlineColorSettings", order = 1)]
    public class OutlineColorSettings : ScriptableObject
    {
        [field: SerializeField]
        public Color Color { get; private set; }
    }
}
