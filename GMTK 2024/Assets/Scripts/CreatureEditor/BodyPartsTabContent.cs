using UnityEngine;

namespace Game
{
    public class BodyPartsTabContent : MonoBehaviour
    {
        [field: SerializeField] 
        public BodyPartType Type { get; private set; }
        
        public void Show(bool show)
        {
            gameObject.SetActive(show);
        }
    }
}
