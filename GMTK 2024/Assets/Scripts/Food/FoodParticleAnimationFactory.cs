using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

namespace Game
{
    public class FoodParticleAnimationFactory : MonoBehaviour
    {
        [SerializeField] private FoodParticleAnimation _animationPrefab;

        private Dictionary<FoodType, FoodAmountDisplay> _foodAmountDisplays = new();
        private ParticlesCanvas _particlesCanvas;

        private void Awake()
        {
            _particlesCanvas = FindObjectOfType<ParticlesCanvas>();
        }

        public void Create(FoodAmount amount, Vector2 startPos, bool reverse = false)
        {
            if (!_foodAmountDisplays.TryGetValue(amount.FoodType, out FoodAmountDisplay display) || display == null)
            {
                display = FindObjectsOfType<FoodAmountDisplay>().FirstOrDefault(d => d.FoodType == amount.FoodType);
                _foodAmountDisplays[amount.FoodType] = display;
            }

            if (display == null)
            {
                return;
            }

            FoodParticleAnimation anim = Instantiate(_animationPrefab, _particlesCanvas.Hook);
            StartCoroutine(!reverse
                ? anim.StartAnimation(startPos, display.Icon.transform.position, amount)
                : anim.StartAnimation(display.Icon.transform.position, startPos, amount));
        }
    }
}