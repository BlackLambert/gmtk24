using UnityEngine;

namespace Game
{
    public class SetOutlineColor : MonoBehaviour
    {
        [SerializeField] private OutlineColorSettings _outlineColorSettings;

        private void Start()
        {
            foreach (SpriteRenderer sprite in GetComponentsInChildren<SpriteRenderer>())
            {
                sprite.color = _outlineColorSettings.Color;
            }
        }
    }
}
