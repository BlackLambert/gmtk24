using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "EnemySettings", menuName = "Game/EnemySettings", order = 1)]

    public class EnemySettings : ScriptableObject
    {
        [field: SerializeField]
        public int HitPoints { get; private set; }
    }
}
