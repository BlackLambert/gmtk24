using UnityEngine;

namespace Game
{
    public class StateOnStartSetter : MonoBehaviour
    {
        [SerializeField] private GameState _state;

        private Game _game;
        
        private void Awake()
        {
            _game = Game.Instance;
        }

        private void Start()
        {
            _game.State = _state;
        }
    }
}
