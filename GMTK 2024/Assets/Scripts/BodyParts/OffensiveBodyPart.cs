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
        [SerializeField] Collider2D _hitBox;
        Creature _parentCreature;
        float _lastInterval = 0;
        Transform currentTargetTransform;

        void Start()
        {
            _hitBox = GetComponentInChildren<Collider2D>();
            _parentCreature = GetComponentInParent<Creature>();

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
                _hitBox.transform.position = currentTargetTransform.position;
            }
            Activate();
        }

        private void Activate()
        {
            if (Time.realtimeSinceStartup < _lastInterval + _settings.Cooldown)
            {
                return;
            }

            switch (_settings.TargetType)
            {
                case TargetType.None:
                    break;
                case TargetType.Closest:
                    Collider2D[] possibleTargets = Physics2D.OverlapCircleAll(transform.position, _settings.TargetRange);
                    if (possibleTargets.Length == 0) return;
                    float shortestDistance = Mathf.Infinity;
                    int targetIndex = -1;
                    for (int i=0; i< possibleTargets.Length;i++){ 
                    
                        Collider2D col = possibleTargets[i];
                        if(col.TryGetComponent<Enemy>(out Enemy targetEnemy))
                        {
                            float distance = Vector2.Distance(transform.position, col.transform.position);
                            if (distance < shortestDistance)
                            {
                                targetIndex = i;
                                shortestDistance = distance;
                            }
                        }
                    }
                    _hitBox.transform.position = possibleTargets[targetIndex].transform.position;
                    currentTargetTransform = possibleTargets[targetIndex].transform;
                    break;
                case TargetType.Random:
                    Collider2D[] possibleTargetsRandom = Physics2D.OverlapCircleAll(transform.position, _settings.TargetRange);
                    if (possibleTargetsRandom.Length == 0) return;
                    int randomIndex = Random.Range(0, possibleTargetsRandom.Length - 1);
                    _hitBox.transform.position = possibleTargetsRandom[randomIndex].transform.position;
                    currentTargetTransform = possibleTargetsRandom[randomIndex].transform;
                    break;
                case TargetType.Frontal:
                    _hitBox.transform.position = _parentCreature.transform.position + _parentCreature.transform.up;
                    currentTargetTransform = _parentCreature.transform;
                    break;
            }

            ContactFilter2D filter = new ContactFilter2D().NoFilter();
            List<Collider2D> hitObjects = new List<Collider2D>();
            _hitBox.OverlapCollider(filter, hitObjects);

            foreach(Collider2D hitObject in hitObjects)
            {
                if (hitObject.TryGetComponent<Enemy>(out Enemy target))
                {
                    if(target != _parentCreature)
                    {
                        ApplyDamageToTarget(_settings.Damage, target);
                    }

                }
            }
            PlayAnimation();
            PlaySound();
            _lastInterval = Time.realtimeSinceStartup;
        }



        private void ApplyDamageToTarget(float damage, Enemy target)
        {
            target.SufferDamage(damage);
        }

        private void PlayAnimation()
        {
            animator?.SetTrigger("Attack");
        }
        private void PlaySound()
        {

        }
        private void ApplyStatusToTarget(StatusEffect status, Enemy target)
        {
            if(status == StatusEffect.None) return;
            target.ApplyStatusEffect(status);
        }

    }
}
