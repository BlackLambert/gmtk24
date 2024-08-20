using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "OffensiveBodypartSettings", menuName = "Game/OffensiveBodypartSettings", order = 1)]
    public class OffensiveBodyPartSettings : ScriptableObject
    {
        [field: SerializeField]
        public TargettingMode TargetType { get; private set; }
        [field: SerializeField]
        public float Cooldown { get; private set; }
        [field: SerializeField]
        public StatusEffect StatusEffect { get; private set;  }
        [field: SerializeField]
        public float Damage { get; private set; }
        [field: SerializeField]
        public float TargetRange { get; private set; }

        [field: SerializeField]
        public float DamageRange { get; private set; }

        [field: SerializeField] 
        public AudioClip[] sounds { get; private set; }
        [field: SerializeField]
        public float Volume { get; private set; }

    }
}

