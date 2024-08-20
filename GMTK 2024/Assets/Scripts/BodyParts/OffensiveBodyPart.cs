using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

namespace Game
{
    public class OffensiveBodyPart : MonoBehaviour
    {
        [SerializeField] Animator animator;

        [SerializeField] OffensiveBodyPartSettings _settings;
        [SerializeField] Transform _hitBox;
        IDamageable _parentCreature;
        float _lastInterval = 0;
        Transform currentTargetTransform;

        private Game _game;

        void Start()
        {
            _game = Game.Instance;
            _parentCreature = GetComponentInParent<IDamageable>();

            if(_parentCreature == null)
            {
                Debug.LogError("Parent Creature is null, body part is detached!");
            }
            if(_hitBox == null)
            {
                Debug.LogError("No Hitbox detected, ALAAAARM!");
                return;
            }
            if(_settings == null)
            {
                Debug.LogError("Bodypart has no settings!");
                return;
            }
            _lastInterval = Time.realtimeSinceStartup + Random.Range(0f, _settings.Cooldown);
        }

        private void Update()
        {
            if (currentTargetTransform != null) 
            { 
                _hitBox.position = currentTargetTransform.position;
            }

            if (_game.State == GameState.InGame)
            {
                Activate();
            }
        }

        private void Activate()
        {
            if (Time.realtimeSinceStartup < _lastInterval + _settings.Cooldown)
            {
                return;
            }


            List<IDamageable> targets = FindTargets(_settings.TargetType);


            foreach (IDamageable damageable in targets)
            {
                ApplyDamageToTarget(_settings.Damage, damageable);
            }

            PlayAnimation();
            PlaySound();
            _lastInterval = Time.realtimeSinceStartup;
        }

        private List<IDamageable> FindTargets(TargettingMode targetType)
        {
            List<IDamageable> finalTargets = new List<IDamageable>();

            if (_parentCreature == null)
            {
                return finalTargets;
            }
           
            List<Collider2D> colliderInRange = new List<Collider2D>();
            colliderInRange.AddRange(Physics2D.OverlapCircleAll(transform.position, _settings.TargetRange * _parentCreature.Transform.localScale.x));
            List<IDamageable> allTargets = CleanUpTargetList(colliderInRange);

            if (allTargets.Count == 0) return finalTargets;

            switch (targetType)
            {
                case TargettingMode.None:
                    break;
                case TargettingMode.Closest:
                    float shortestDistance = Mathf.Infinity;
                    int targetIndex = -1;
                    for (int i = 0; i < allTargets.Count; i++)
                    {
                        IDamageable target = allTargets[i];
                        float distance = Vector2.Distance(transform.position, target.Transform.position);
                        if (distance < shortestDistance)
                        {
                            targetIndex = i;
                            shortestDistance = distance;
                        }
                    }
                    _hitBox.position = allTargets[targetIndex].Transform.position;
                    currentTargetTransform = allTargets[targetIndex].Transform;
                    break;

                case TargettingMode.Random:
                    
                    int randomIndex = Random.Range(0, allTargets.Count - 1);
                    _hitBox.position = allTargets[randomIndex].Transform.position;
                    currentTargetTransform = allTargets[randomIndex].Transform;
                    break;
                case TargettingMode.Frontal:
                    _hitBox.position = _parentCreature.Transform.position + (_parentCreature.Transform.up * _parentCreature.Transform.localScale.x);
                    currentTargetTransform = _parentCreature.Transform;
                    break;
            }

            List<Collider2D> hitObjects = new List<Collider2D>();
            hitObjects.AddRange(Physics2D.OverlapCircleAll(_hitBox.position, _settings.DamageRange * _parentCreature.Transform.localScale.x));

            finalTargets.AddRange(CleanUpTargetList(hitObjects));
            return finalTargets;
        }

        private List<IDamageable> CleanUpTargetList(List<Collider2D> colliderList)
        {
            List<IDamageable> cleansedList = new List<IDamageable>();

            colliderList.RemoveAll(col => col.attachedRigidbody == null);
            colliderList.RemoveAll(col => col.attachedRigidbody.GetComponent<IDamageable>() == null);
            colliderList.RemoveAll(col => col.attachedRigidbody.GetComponent<IDamageable>().GetID() == _parentCreature.GetID());
            foreach(Collider2D collider in colliderList)
            {
                cleansedList.Add(collider.attachedRigidbody.GetComponent<IDamageable>());
            }

            return cleansedList;
        }

        private void ApplyDamageToTarget(float damage, IDamageable target)
        {
            target.SufferDamage(damage, this);
        }

        private void PlayAnimation()
        {
            animator?.SetTrigger("Attack");
        }
        private void PlaySound()
        {
            SoundFXManager manager = SoundFXManager.Instance;
            if (manager == null) return;
            manager.PlayRandomSoundClip(_settings.sounds, transform, _settings.Volume);
        }
        private void ApplyStatusToTarget(StatusEffect status, Enemy target)
        {
            if(status == StatusEffect.None) return;
            target.ApplyStatusEffect(status);
        }
    }
}
