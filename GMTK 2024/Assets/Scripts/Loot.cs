using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [System.Serializable]
    public class Loot
    {
        public GameObject drop;
        public float propability;
        public int minAmount;
        public int maxAmount;

        public int Quantity { get; set; }
    }
}
