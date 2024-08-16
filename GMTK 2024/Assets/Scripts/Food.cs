using System.Collections;
using UnityEngine;

namespace Game
{
    public class Food : MonoBehaviour
    {
        [field: SerializeField]
        public FoodType FoodType { get; private set; }

        [SerializeField] 
        private Animator _animator;

        [SerializeField] 
        private AnimationClip _eatClip;

        [SerializeField]
        private string _eatenBoolKey = "Eaten";

        public void Collect()
        {
            StartCoroutine(DoCollect());
        }

        private IEnumerator DoCollect()
        {
            _animator.SetBool(_eatenBoolKey, true);
            yield return new WaitForSeconds(_eatClip.length);
            Destroy(gameObject);
        }
    }
}
