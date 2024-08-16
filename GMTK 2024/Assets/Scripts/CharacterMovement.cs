using System;
using UnityEngine;

namespace Game
{
    public class CharacterMovement : MonoBehaviour
    {
        [SerializeField] 
        private Character _character;

        [SerializeField] 
        private Camera _camera;

        [SerializeField] 
        private MovementSettings _movementSettings;

        private Vector2 _mousePoint;
        private float _lastUsed = float.MinValue;
        
        private void Update()
        {
            Vector3 mousePoint = Input.mousePosition;
            Vector3 worldPoint = _camera.ScreenToWorldPoint(mousePoint);
            Vector3 direction = worldPoint - _character.Transform.position;
            
            LookAt(worldPoint, direction);
            Move(direction);
            LimitSpeed();
        }

        private void LookAt(Vector3 worldPoint, Vector3 direction)
        {
            if (worldPoint == _character.transform.position)
            {
                return;
            }
            
            direction.Normalize(); 
            float rot_z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; 
            _character.transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
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
            
            
            _character.Rigidbody.AddForce(((Vector2)direction).normalized * _movementSettings.Force);
            _lastUsed = Time.realtimeSinceStartup;
        }

        private void LimitSpeed()
        {
            Vector3 velocity = _character.Rigidbody.velocity;
            float velocityMagnitude = velocity.magnitude;
            if (velocityMagnitude > _movementSettings.MaxSpeed)
            {
                _character.Rigidbody.velocity = velocity.normalized * _movementSettings.MaxSpeed;
            }
        }
    }
}