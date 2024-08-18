using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "BodySettings", menuName = "Game/BodySettings", order = 1)]
    public class BodySettings : ScriptableObject
    {
        [field: SerializeField] public float SlotsPerSize { get; private set; } = 1;
        
        [field: SerializeField] public SplineData[] StartSplines { get; private set; }

        [field: SerializeField] public float MinScale { get; private set; } = 0.5f;

        [field: SerializeField] public float ScalePerSize { get; private set; } = 0.25f;
        
        [field: SerializeField] public int MaxSplines { get; private set; } = 10;
        
        [field: SerializeField] public int MinSplines { get; private set; } = 1;
        
        [field: SerializeField] public int MaxSize { get; private set; } = 5;
        
        [field: SerializeField] public float SplineSpacing { get; private set; } = 0.25f;
        
        [field: SerializeField] public int HalfCircleVerticesPerSize { get; private set; } = 7;
        
        [field: SerializeField] public int StartHalfCircleVertices { get; private set; } = 15;
        
        [field: SerializeField] public int SideVertices { get; private set; } = 5;
        
        [field: SerializeField] public AnimationCurve WeightCurve { get; private set; }

        [field: SerializeField]
        public FoodAmount Costs { get; private set; } = new FoodAmount { Amount = 5, FoodType = FoodType.Purple };

        [field: SerializeField] public float HealthPerSize { get; private set; } = 3f;
    }
}
