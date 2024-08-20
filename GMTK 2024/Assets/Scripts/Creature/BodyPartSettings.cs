using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "BodyPartSettings", menuName = "Game/BodyPartSettings", order = 1)]
    public class BodyPartSettings : ScriptableObject
    {
        [field: SerializeField] public string Id { get; private set; } = "BodyPart";

        [field: SerializeField] public BodyPartType BodyPartType { get; private set; } = BodyPartType.Attack;

        public IReadOnlyList<FoodAmount> Costs => _costs;

        [SerializeField] private List<FoodAmount> _costs;

        [field: SerializeField] public Sprite Icon { get; private set; }
        
        [field: SerializeField] public BodyPartSlotType SlotType { get; private set; }
        
        [field: SerializeField] public bool NeedsCounterPartSlot { get; private set; }

        [field: SerializeField] public float ScalePerSizeAddition { get; private set; } = 0;
        
        [field: SerializeField] public string DisplayName { get; private set; }
    }
}