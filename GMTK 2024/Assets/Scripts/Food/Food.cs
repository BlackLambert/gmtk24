using System.Collections;
using System.Security.Permissions;
using UnityEngine;

namespace Game
{
    public class Food : MonoBehaviour
    {
        [field: SerializeField]
        public FoodType FoodType { get; private set; }
        [field:SerializeField]
        public SizeStage size { get; private set; }
        [field: SerializeField] 
        private AudioClip[] _eatSounds { get; set; }

        [SerializeField] 
        private Animator _animator;

        [SerializeField] 
        private AnimationClip _eatClip;

        [SerializeField]
        private string _eatenBoolKey = "Eaten";

        [SerializeField] 
        private Collider2D _collider;

        public void Collect(bool eatenByPlayer)
        {
            StartCoroutine(DoCollect());
            float volume = eatenByPlayer ? 1.0f : 0.2f;
            SoundFXManager.Instance.PlayRandomSoundClip(_eatSounds, transform, volume);
        }

        private IEnumerator DoCollect()
        {
            _collider.enabled = false;
            _animator.SetBool(_eatenBoolKey, true);
            yield return new WaitForSeconds(_eatClip.length);
            Destroy(gameObject);
        }
    }
}
