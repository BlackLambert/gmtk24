using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class CollectedFood : MonoBehaviour
    {
        public event Action<FoodType, int> OnAmountChanged;
        public event Action<FoodType> OnFoodCollected;
        public int TotalCollected { get; private set; } = 0;
        
        private Dictionary<FoodType, int> _typeToAmount = new();
        
        private void Awake()
        {
            foreach (FoodType foodType in Enum.GetValues(typeof(FoodType)))
            {
                _typeToAmount.Add(foodType, 0);
            }
        }

        public void Collect(FoodType type)
        {
            _typeToAmount[type] += 1;
            TotalCollected++;
            OnFoodCollected?.Invoke(type);
            OnAmountChanged?.Invoke(type, _typeToAmount[type]);
        }

        public void ChangeBy(FoodType type, int amount)
        {
            int newAmount = _typeToAmount[type] + amount;

            if (newAmount < 0)
            {
                throw new ArgumentException();
            }

            _typeToAmount[type] = newAmount;
            OnAmountChanged?.Invoke(type, newAmount);
        }

        public int GetAmountOf(FoodType type)
        {
            return _typeToAmount[type];
        }

        public void Add(IReadOnlyList<FoodAmount> costs)
        {
            foreach (FoodAmount amount in costs)
            {
                ChangeBy(amount.FoodType, amount.Amount);
            }
        }
    }
}
