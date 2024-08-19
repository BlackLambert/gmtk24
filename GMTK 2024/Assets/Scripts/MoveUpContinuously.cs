using UnityEngine;

namespace Game
{
    public class MoveUpContinuously : MonoBehaviour
    {
        [SerializeField] private Transform _transform;
        [SerializeField] private float _speed;

        private void Update()
        {
            float delta = Time.deltaTime * _speed;
            _transform.position += Vector3.up * delta;
        }
    }
}
