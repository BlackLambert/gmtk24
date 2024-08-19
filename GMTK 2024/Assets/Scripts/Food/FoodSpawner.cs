using UnityEngine;

namespace Game
{
    public class FoodSpawner : MonoBehaviour
    {
        [SerializeField] 
        private Food _foodPrefab;

        [SerializeField] 
        private float _cooldown = 15f;

        [SerializeField] 
        private Transform _hook;

        [SerializeField] 
        private float _scale = 1;

        private Food _food;
        private float _spawnTime = float.MinValue;
        private bool _spawned;

        private void Awake()
        {
            Spawn();
        }

        private void Update()
        {
            if (!_spawned && _spawnTime <= Time.time)
            {
                Spawn();
            }
            else if (_food == null && _spawned)
            {
                _spawned = false;
                _spawnTime = Time.time + _cooldown;
            }
        }

        private void Spawn()
        {
            _food = Instantiate(_foodPrefab, _hook, false);
            _food.transform.localScale = new Vector3(_scale, _scale, _scale);
            _spawned = true;
        }
    }
}
