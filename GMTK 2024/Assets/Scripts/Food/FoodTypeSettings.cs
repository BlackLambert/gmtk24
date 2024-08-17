using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "FoodTypeSettings", menuName = "Game/FoodTypeSettings", order = 1)]
    public class FoodTypeSettings : ScriptableObject
    {
        [SerializeField] private List<TypeSettings> _settings;

        public IReadOnlyList<TypeSettings> Settings => _settings;

        public TypeSettings Get(FoodType type)
        {
            return _settings.First(s => s.Type == type);
        }

        [Serializable]
        public class TypeSettings
        {
            public FoodType Type;
            public Sprite Sprite;
        }
    }
}
