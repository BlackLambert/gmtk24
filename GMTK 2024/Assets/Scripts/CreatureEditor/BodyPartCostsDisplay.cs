using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class BodyPartCostsDisplay : MonoBehaviour
    {
        [SerializeField] 
        private FoodTypeSettings _foodTypeSettings;

        [SerializeField] 
        private TextMeshProUGUI _text;

        [SerializeField] 
        private Image _icon;

        [SerializeField] 
        private Color _defaultTextColor;

        [SerializeField] 
        private Color _notEnoughTextColor;
        
        private CollectedFood _collectedFood;
        private FoodAmount _amount;

        public void Init(FoodAmount amount)
        {
            _amount = amount;
            _text.text = amount.Amount.ToString();
            _icon.sprite = _foodTypeSettings.Get(amount.FoodType).Sprite;
        }

        private void Awake()
        {
            _collectedFood = FindObjectOfType<CollectedFood>();
        }

        private void Update()
        {
            _text.color = _collectedFood.Has(_amount) ? _defaultTextColor : _notEnoughTextColor;
        }
    }
}
