using System.Linq;
using UnityEngine;

namespace Game
{
    public class BodyPartsTabContent : MonoBehaviour
    {
        [field: SerializeField] 
        public BodyPartType Type { get; private set; }

        [SerializeField] 
        private BodyPartCollectionSettings _bodyPartCollection;

        [SerializeField] 
        private Transform _hook;

        [SerializeField] 
        private BodyPartButton _buttonPrefab;

        private void Start()
        {
            CreateButtons();
        }

        public void Show(bool show)
        {
            gameObject.SetActive(show);
        }

        private void CreateButtons()
        {
            foreach (BodyPart part in _bodyPartCollection.BodyParts.Where(p => p.BodyPartSettings.BodyPartType == Type))
            {
                BodyPartButton button = Instantiate(_buttonPrefab, _hook);
                button.Init(part);
            }
        }
    }
}
