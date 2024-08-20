using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "EnemySettings", menuName = "Game/EnemySettings", order = 1)]

    public class EnemySettings : ScriptableObject
    {
        [field: SerializeField]
        public int speciesID { get; private set; }
        [field: SerializeField]
        public string speciesName { get; private set; }

        [field: SerializeField]
        public SizeStage size { get; private set; }

        [field: SerializeField]
        public int HitPoints { get; private set; }

        [SerializeField] public Loot[] lootTable;

        [field: SerializeField]
        public MovementSettings MovementSettings { get; private set; }
        [field: SerializeField]
        public CreatureDiet CreatureDiet { get; private set; }
        [field: SerializeField]
        public float FleeRange { get; private set; }
        [field: SerializeField]
        public float FleeDuration { get; private set; }
        [field: SerializeField]
        public float FollowRange { get; private set; }
        public List<Loot> GetLoot()
        {
            List<Loot> dropList = new List<Loot>();
            foreach (Loot loot in lootTable)
            {
                float roll = Random.Range(0, 100f);
                if(roll <= loot.propability)
                {
                    loot.Quantity = Random.Range(loot.minAmount, loot.maxAmount);
                    dropList.Add(loot);
                }
            }
            return dropList;
        }
    }
}
