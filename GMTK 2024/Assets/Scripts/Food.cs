using UnityEngine;

namespace Game
{
    public class Food : MonoBehaviour
    {
        [field: SerializeField]
        public FoodType FoodType { get; private set; }

        public void Destruct()
        {
            Destroy(gameObject);
        }
    }
}
