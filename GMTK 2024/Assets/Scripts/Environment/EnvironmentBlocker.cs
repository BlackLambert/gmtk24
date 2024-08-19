using UnityEngine;

namespace Game
{
    public class EnvironmentBlocker : MonoBehaviour
    {
        [SerializeField] private int _blockTillStage = 1;
        [SerializeField] private LayerMask _maskToExclude = 1<<6;

        private Game _game;

        private void Awake()
        {
            _game = Game.Instance;
        }

        private void Start()
        {
            bool exclude = _game.CurrentStage.StageIndex > _blockTillStage;
            if (exclude)
            {
                foreach (Collider2D collider2D in GetComponentsInChildren<Collider2D>())
                {
                    collider2D.excludeLayers |= _maskToExclude;
                }
            } 
        }
    }
}
