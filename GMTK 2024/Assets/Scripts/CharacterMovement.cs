using System;
using UnityEngine;

namespace Game
{
    public class CharacterMovement : MonoBehaviour
    {
        private MovementSettings _movementSettings;
        private Camera _camera;
        private Creature _creature;
        private Game _game;
        private Vector2 _mousePoint;
        private float _lastUsed = float.MinValue;

        private void Awake()
        {
            _game = FindObjectOfType<Game>();
        }

        public void Init(Creature creature, Camera camera, MovementSettings movementSettings)
        {
            _creature = creature;
            _camera = camera;
            _movementSettings = movementSettings;
        }

        private void Update()
        {
            if (_game.State != GameState.InGame)
            {
                return;
            }
            
            Vector3 mousePoint = Input.mousePosition;
            Vector3 worldPoint = _camera.ScreenToWorldPoint(mousePoint);
            Vector3 direction = worldPoint - _creature.Transform.position;
            
            LookAt(worldPoint, direction);
            Move(direction);
            LimitSpeed();
        }

        private void LookAt(Vector3 worldPoint, Vector3 direction)
        {
            if (worldPoint == _creature.transform.position)
            {
                return;
            }
            
            direction.Normalize(); 
            float rot_z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; 
            _creature.transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
        }

        private void Move(Vector3 direction)
        {
            if (!Input.GetMouseButton(0))
            {
                return;
            }

            if (Time.realtimeSinceStartup < _lastUsed + _movementSettings.Cooldown)
            {
                return;
            }
            
            
            _creature.Rigidbody.AddForce(((Vector2)direction).normalized * _movementSettings.Force);
            _lastUsed = Time.realtimeSinceStartup;
        }

        private void LimitSpeed()
        {
            Vector3 velocity = _creature.Rigidbody.velocity;
            float velocityMagnitude = velocity.magnitude;
            if (velocityMagnitude > _movementSettings.MaxSpeed)
            {
                _creature.Rigidbody.velocity = velocity.normalized * _movementSettings.MaxSpeed;
            }
        }
    }
}