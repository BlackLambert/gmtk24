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

        public void Init(FoodAmount amount)
        {
            _text.text = amount.Amount.ToString();
            _icon.sprite = _foodTypeSettings.Get(amount.FoodType).Sprite;
        }
    }
}
