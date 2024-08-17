using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game
{
    public class BodyPartButton : MonoBehaviour, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField] 
        private Image _icon;

        [SerializeField] 
        private Transform _costsHook;

        [SerializeField] 
        private BodyPartCostsDisplay _costsPrefab;

        public void Init(BodyPart bodyPartPrefab)
        {
            BodyPartSettings settings = bodyPartPrefab.BodyPartSettings;
            _icon.sprite = bodyPartPrefab.BodyPartSettings.Icon;
            foreach (FoodAmount amount in settings.Costs)
            {
                BodyPartCostsDisplay costsDisplay = Instantiate(_costsPrefab, _costsHook);
                costsDisplay.Init(amount);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            
        }
    }
}
