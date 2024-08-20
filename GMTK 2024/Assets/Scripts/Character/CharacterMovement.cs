using System;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class CharacterMovement : MonoBehaviour
    {
        private Camera _camera;
        private Creature _creature;
        private Game _game;
        private Vector2 _mousePoint;
        private float[] _lastUsed;
        private MovementBodyPart[] _movementSettings;
        private MovementSettings _baseMovement;

        private void Awake()
        {
            _game = Game.Instance;
            _camera = FindObjectOfType<MainCamera>().Camera;
        }

        public void Init(Creature creature, MovementSettings baseMovement)
        {
            _creature = creature;
            _movementSettings = _creature.GetComponentsInChildren<MovementBodyPart>().ToArray();
            _baseMovement = baseMovement;
            _lastUsed = new float[_movementSettings.Length];
        }

        private void FixedUpdate()
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
            Quaternion currentRotation = _creature.transform.rotation;
            Vector3 currentDirection = currentRotation * Vector3.up;
            float zRot = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float currentZRot = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, zRot - 90);
            float angle = Quaternion.Angle(targetRotation, currentRotation);
            float maxDeltaRot = (_movementSettings.Sum(s => s.MovementSettings.RotationSpeed) + _baseMovement.RotationSpeed) * Time.fixedDeltaTime;
            float delta = Mathf.Min(maxDeltaRot, angle);
            float absDelta = Mathf.Abs(zRot - currentZRot);
            if (zRot < currentZRot && absDelta < 180 || zRot > currentZRot && absDelta > 180)
            {
                delta *= -1;
            }

            //Debug.Log($"Delta {delta} | Angle {angle} | Max {maxDeltaRot}");
            _creature.transform.rotation = Quaternion.Euler(0, 0, currentRotation.eulerAngles.z + delta);
        }

        private void Move(Vector3 direction)
        {
            if (!Input.GetMouseButton(0))
            {
                return;
            }

            Vector2 baseForce = ((Vector2)direction).normalized *
                            (_baseMovement.Force * _game.CurrentStage.StageSettings.SpeedFactor);
            _creature.Rigidbody.AddForce(baseForce);
            
            for (int index = 0; index < _movementSettings.Length; index++)
            {
                MovementBodyPart part = _movementSettings[index];
                if (Time.realtimeSinceStartup < _lastUsed[index] + part.MovementSettings.Cooldown)
                {
                    return;
                }

                LegAnimationController legs = part.GetComponentInChildren<LegAnimationController>();
                legs?.Jump();

                Vector2 force = ((Vector2)direction).normalized *
                                (part.MovementSettings.Force * _game.CurrentStage.StageSettings.SpeedFactor);
                _creature.Rigidbody.AddForce(force);
                _lastUsed[index] = Time.realtimeSinceStartup;
            }
        }

        private void LimitSpeed()
        {
            Vector3 velocity = _creature.Rigidbody.velocity;
            float velocityMagnitude = velocity.magnitude;
            float maxSpeed = _movementSettings.Sum(s => s.MovementSettings.MaxSpeed) + _baseMovement.MaxSpeed;
            if (velocityMagnitude > maxSpeed)
            {
                _creature.Rigidbody.velocity = velocity.normalized * maxSpeed;
            }
        }
    }
}