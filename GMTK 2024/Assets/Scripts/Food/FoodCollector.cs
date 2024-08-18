using UnityEngine;
using static Codice.Client.Common.EventTracking.TrackFeatureUseEvent.Features.DesktopGUI.Filters;

namespace Game
{
    public class FoodCollector : MonoBehaviour
    {
        private CollectedFood _collectedFood;
        private FoodParticleAnimationFactory _particleAnimationFactory;
        private Camera _camera;
        private bool _attachedToPlayerCreature;
        
        private void Awake()
        {
            _attachedToPlayerCreature = TryGetComponent<Creature>(out Creature c);
            _collectedFood = FindObjectOfType<CollectedFood>();
            _particleAnimationFactory = FindObjectOfType<FoodParticleAnimationFactory>();
            _camera = FindObjectOfType<MainCamera>().Camera;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Food food = other.GetComponent<Food>();
            if (food != null)
            {
                Collect(food);
            }
        }

        private void Collect(Food food)
        {
            food.Collect();
            if (_attachedToPlayerCreature)
            {
                _collectedFood.Collect(food.FoodType);
                Vector2 screenPos = _camera.WorldToScreenPoint(food.transform.position);
                _particleAnimationFactory.Create(new FoodAmount(){FoodType = food.FoodType, Amount = 1}, screenPos);
            }
        }
    }
}
