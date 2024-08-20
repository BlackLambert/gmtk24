using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class FoodAmountDisplay : MonoBehaviour
    {
        [SerializeField] 
        private TextMeshProUGUI _text;

        [SerializeField] 
        private FoodType _foodType;

        [field: SerializeField]
        public Image Icon { get; private set; }
        [field: SerializeField]
        public AudioClip Sound{ get; private set; }

        public FoodType FoodType => _foodType;
        
        
        private CollectedFood _collectedFood;

        private void Awake()
        {
            _collectedFood = FindObjectOfType<CollectedFood>();
        }

        private void Start()
        {
            UpdateText(_collectedFood.GetAmountOf(_foodType));
            _collectedFood.OnAmountChanged += OnFoodChanged;
        }

        private void OnDestroy()
        {
            _collectedFood.OnAmountChanged -= OnFoodChanged;
        }

        private void OnFoodChanged(FoodType foodType, int amount)
        {
            if (foodType == _foodType)
            {
                UpdateText(amount);
                SoundFXManager.Instance.PlaySoundClip(Sound, transform, 1);
            }
        }

        private void UpdateText(int amount)
        {
            _text.text = amount.ToString();
        }
    }
}
