using System;
using UnityEngine;

namespace Game
{
    public class CreatureEditorCreatureCreator : MonoBehaviour
    {
        private Game _game;

        private void Awake()
        {
            _game = FindObjectOfType<Game>();
        }

        private void Start()
        {
            Character character = Instantiate(_game.CurrentStage.Character, Vector3.zero, Quaternion.Euler(0,0, 0));
            character.gameObject.SetActive(true);
            _game.CurrentCharacter = character;
        }
    }
}
