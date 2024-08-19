using System;
using UnityEngine;

namespace Game
{
    public class CreatureEditorCreatureCreator : MonoBehaviour
    {
        private Game _game;

        private void Awake()
        {
            _game = Game.Instance;
        }

        private void Start()
        {
            Character character = Instantiate(_game.CurrentStage.Character, Vector3.zero, Quaternion.Euler(0,0, 0));
            character.transform.localScale = Vector3.one;
            character.name = $"EditorCharacter{_game.CurrentStage.StageIndex}";
            character.gameObject.SetActive(true);
            _game.CurrentCharacter = character;
        }
    }
}
