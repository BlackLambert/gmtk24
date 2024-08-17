using System;
using UnityEngine;

namespace Game
{
    public class FoodCollector : MonoBehaviour
    {
        private CollectedFood _collectedFood;
        
        private void Awake()
        {
            _collectedFood = FindObjectOfType<CollectedFood>();
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
            _collectedFood.Collect(food.FoodType);
        }
    }
}
