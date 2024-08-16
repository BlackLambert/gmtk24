using System;
using UnityEngine;

namespace Game
{
    public class CharacterMovement : MonoBehaviour
    {
        [SerializeField] 
        private Character _character;

        [SerializeField]
        private float _speed = 5f;

        [SerializeField] 
        private Camera _camera;

        private Vector2 _mousePoint;
        
        private void Update()
        {
            if (!Input.GetMouseButton(0))
            {
                return;
            }

            Vector3 mousePoint = Input.mousePosition;
            Vector3 worldPoint = _camera.ScreenToWorldPoint(mousePoint);
            Vector3 direction = worldPoint - _character.Transform.position;
            _character.transform.LookAt(new Vector3(worldPoint.x, worldPoint.y, 0));
            _character.Rigidbody.AddForce(direction.normalized * _speed);
        }
    }
}