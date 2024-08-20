using UnityEngine;

namespace Game
{
    public class FoodCollector : MonoBehaviour
    {
        private CollectedFood _collectedFood;
        private FoodParticleAnimationFactory _particleAnimationFactory;
        private Camera _camera;
        private bool _attachedToPlayerCreature;
        private IDamageable _attachedDamageable;

        private float _foodHealAmount = 0.02f;
        
        private void Awake()
        {
            if(TryGetComponent<Creature>(out Creature c))
            {
                _attachedToPlayerCreature = c;
                _attachedDamageable = c;
            }
            else
            {
                _attachedDamageable = GetComponent<Enemy>();
            }
            _collectedFood = FindObjectOfType<CollectedFood>();
            _particleAnimationFactory = FindObjectOfType<FoodParticleAnimationFactory>();
            _camera = FindObjectOfType<MainCamera>().Camera;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Food food = other.GetComponent<Food>();
            if (food != null)
            {
                if((int)food.size <= (int) _attachedDamageable.GetSize())
                {
                    Collect(food);
                }
            }
        }

        private void Collect(Food food)
        {
            if (_attachedToPlayerCreature)
            {
                food.Collect(true);

                Creature parent = _attachedDamageable as Creature;
                parent.HealDamage(_foodHealAmount, true);
                _collectedFood.Collect(food.FoodType);
                Vector2 screenPos = _camera.WorldToScreenPoint(food.transform.position);
                _particleAnimationFactory.Create(new FoodAmount(){FoodType = food.FoodType, Amount = 1}, screenPos);
            } else
            {
                food.Collect(false);
            }
        }
    }
}
