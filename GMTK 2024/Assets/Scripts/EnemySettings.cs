using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "EnemySettings", menuName = "Game/EnemySettings", order = 1)]

    public class EnemySettings : ScriptableObject
    {
        [field: SerializeField]
        SizeStage size;

        [field: SerializeField]
        public int HitPoints { get; private set; }

        [SerializeField] public Loot[] lootTable;

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
