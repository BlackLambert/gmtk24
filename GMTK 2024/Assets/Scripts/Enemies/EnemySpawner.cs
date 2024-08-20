using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private float _startTimeOffset = 0;
        [SerializeField] private float _cooldown = 0;
        [SerializeField] private Enemy _enemyPrefab;
        [SerializeField] private int _limit = 3;

        private float _nextSpawn = float.MaxValue;
        private GameHook _gameHook;
        private Transform _transform;
        private List<Enemy> _enemies = new List<Enemy>();

        private void Start()
        {
            _nextSpawn = Time.time + _startTimeOffset;
            _gameHook = FindObjectOfType<GameHook>();
            _transform = transform;
        }

        private void Update()
        {
            _enemies.RemoveAll(e => e == null);
            
            if (Time.time >= _nextSpawn && _enemies.Count < _limit)
            {
                Enemy enemy = Instantiate(_enemyPrefab, _gameHook.transform, false);
                Transform enemyTransform = enemy.transform;
                enemyTransform.gameObject.SetActive(true);
                enemyTransform.rotation = _transform.rotation;
                enemyTransform.position = _transform.position;
                _nextSpawn = Time.time + _cooldown;
                _enemies.Add(enemy);
            }
        }
    }
}
