using System.Collections;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class FoodParticleAnimation : MonoBehaviour
    {
        [SerializeField] private FoodTypeSettings _settings;
        [SerializeField] private AnimationCurve _curve;
        [SerializeField] private RectTransform _hook;
        [SerializeField] private FoodParticle _particlePrefab;
        [SerializeField] private float _duration = 1.5f;
        [SerializeField] private float _pause = 0.2f;

        private float[] _time;
        private Coroutine[] _routines;
        
        public IEnumerator StartAnimation(Vector2 start, Vector2 end, FoodAmount amount)
        {
            FoodTypeSettings.TypeSettings settings = _settings.Get(amount.FoodType);
            _time = new float[amount.Amount];
            _routines = new Coroutine[amount.Amount];

            for (int i = 0; i < amount.Amount; i++)
            {
                _routines[i] = StartCoroutine(Animate(start, end, settings, i * _pause, i));
            }

            while (_routines.Any(r => r != null))
            {
                yield return 0;
            }
            
            Destroy(gameObject);
        }

        private IEnumerator Animate(Vector2 start, Vector2 end, FoodTypeSettings.TypeSettings settings, float pause, int index)
        {
            yield return new WaitForSeconds(pause);
            FoodParticle particle = Instantiate(_particlePrefab, _hook, false);
            particle.Image.sprite = settings.Sprite;
            particle.transform.position = start;
            _time[index] = Time.realtimeSinceStartup;
            Vector2 direction = end - start;
            while (Time.realtimeSinceStartup < _time[index] + _duration)
            {
                float currentTime = Time.realtimeSinceStartup - _time[index];
                float portion = currentTime / _duration;
                float pathPortion = _curve.Evaluate(portion);
                particle.transform.position = start + direction * pathPortion;
                yield return 0;
            }

            _routines[index] = null;
        }
    }
}
