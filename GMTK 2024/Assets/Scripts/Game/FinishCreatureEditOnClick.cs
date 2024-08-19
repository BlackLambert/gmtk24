using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class FinishCreatureEditOnClick : MonoBehaviour
    {
        [SerializeField] 
        private Button _button;

        private Game _game;

        private void Awake()
        {
            _game = Game.Instance;
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
            _game.CurrentStage.EvolvedCharacter = _game.CurrentCharacter;
            _game.CurrentCharacter.gameObject.SetActive(false);
            _game.CurrentCharacter = null;
            _game.CurrentStage.Evolved = true;
            _game.State = GameState.InGame;
        }
    }
}
