using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class CloseGameButton : MonoBehaviour
    {
        [SerializeField] private Button _button;

        private void Start()
        {
            _button.onClick.AddListener(CloseGame);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(CloseGame);
        }

        private void CloseGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
            Application.OpenURL(webplayerQuitURL);
#else
            Application.Quit();
#endif
        }
    }
}