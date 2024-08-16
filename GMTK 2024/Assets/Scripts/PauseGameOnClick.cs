using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class PauseGameOnClick : MonoBehaviour
    {
        [SerializeField] 
        private Button _button;

        [SerializeField] 
        private bool _pause;

        private void Start()
        {
            _button.onClick.AddListener(OnClick);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnClick);
        }

        private void OnClick()
        {
            Time.timeScale = _pause ? 0 : 1;
        }
    }
}
