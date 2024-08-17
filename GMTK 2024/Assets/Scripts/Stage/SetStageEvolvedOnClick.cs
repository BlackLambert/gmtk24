using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class SetStageEvolvedOnClick : MonoBehaviour
    {
        [SerializeField] 
        private Button _button;

        private Game _game;

        private void Awake()
        {
            _game = FindObjectOfType<Game>();
        }

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
            _game.CurrentStage.Evolved = true;
        }
    }
}
