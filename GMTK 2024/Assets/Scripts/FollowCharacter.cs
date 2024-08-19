using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class FollowCharacter : MonoBehaviour
    {
        [SerializeField] 
        private Transform _transform;

        private Game _game;

        private void Awake()
        {
            _game = Game.Instance;
        }

        private void Update()
        {
            if (_game.CurrentCharacter != null)
            {
                Vector3 targetPos = _game.CurrentCharacter.Transform.position;
                _transform.position = new Vector3(targetPos.x, targetPos.y, _transform.position.z);
            }
        }
    }
}
