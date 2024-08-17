using UnityEngine;

namespace Game
{
    public class Character : MonoBehaviour
    {
        [field: SerializeField, HideInInspector]
        public Creature Creature { get; private set; }
        public Transform Transform { get; private set; }
        
        private void Awake()
        {
            Transform = transform;
        }

        public void Init(Creature creature)
        {
            Creature = creature;
        }
    }
}
