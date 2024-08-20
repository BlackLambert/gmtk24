using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    public class EnemyBehaviour : MonoBehaviour
    {
        [SerializeField] Enemy _enemy;

        List<Transform> _fleeList = new List<Transform>();
        List<Transform> _followList = new List<Transform>();

        Transform currentFollowTarget;
        float _lastUsed = 0;

        bool _fleeing = false;
        bool _inFleeCooldown = false;
        float _fleeDuration = 5;
        float _fleeTime = 0;
        Vector3 _fleeDirection;

        float[] _speedAdjustments = new float[3];
        float _speedAdjustment = 1;
        Creature _creature;

        private void Start()
        {
            _speedAdjustments[0] = 0.05f;
            _speedAdjustments[1] = 0.1f;
            _speedAdjustments[2] = 0.2f;
            _speedAdjustment = _speedAdjustments[(int)_enemy.Settings.size];
            _creature = FindFirstObjectByType<Creature>();

        }

        // Update is called once per frame
        void Update()
        {
            UpdateFollowList(_enemy.Settings.FollowRange);
            UpdateFleeList(_enemy.Settings.FleeRange);

            if (_fleeList.Count > 0)
            {
                Flee();
                LookAt(_fleeDirection);
                Move(_fleeDirection);
                return;
            } else if (_fleeing)
            {
                _inFleeCooldown = true;
                _fleeing = false;
                _fleeTime = Time.realtimeSinceStartup;
            }
            if (_inFleeCooldown)
            {
                if (Time.realtimeSinceStartup > _fleeTime + _fleeDuration)
                {
                    _inFleeCooldown = false;
                } else
                {
                    LookAt(_fleeDirection);
                    Move(_fleeDirection);
                    return;
                }
            }

            FindMovementTarget();
            if (currentFollowTarget != null)
            {
                Vector3 dir = currentFollowTarget.position - _enemy.transform.position;

                LookAt(dir);
                Move(dir);
            }
        }

        void LookAt(Vector3 direction)
        {

            direction.Normalize();
            Quaternion currentRotation = _enemy.transform.rotation;
            Vector3 currentDirection = currentRotation * Vector3.up;
            float zRot = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float currentZRot = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, zRot - 90);
            float angle = Quaternion.Angle(targetRotation, currentRotation);
            float maxDeltaRot = _enemy.MovementSettings.RotationSpeed * Time.deltaTime;
            float delta = Mathf.Min(maxDeltaRot, angle);
            float absDelta = Mathf.Abs(zRot - currentZRot);
            if (zRot < currentZRot && absDelta < 180 || zRot > currentZRot && absDelta > 180)
            {
                delta *= -1;
            }
            _enemy.transform.rotation = Quaternion.Euler(0, 0, currentRotation.eulerAngles.z + delta);
        }

        void Move(Vector3 direction)
        {
            if (Time.realtimeSinceStartup < _lastUsed + _enemy.MovementSettings.Cooldown)
            {
                return;
            }

            LegAnimationController legs = GetComponentInChildren<LegAnimationController>();
            legs?.Jump();

            _enemy.Rigidbody.AddForce(((Vector2)direction).normalized * (_enemy.MovementSettings.Force * _speedAdjustment));
            _lastUsed = Time.realtimeSinceStartup;
        }
        void Flee()
        {
            Transform closest = FindClosestTransform(_fleeList);
            _fleeDirection = (closest.position - _enemy.transform.position).normalized*-1.3f + Random.onUnitSphere;
            _fleeing = true;
        }

        void FindMovementTarget()
        {
            if(_followList.Count == 0)
            {
                if (_creature != null)
                {
                    currentFollowTarget = _creature.transform;
                }
                return;
            }

            currentFollowTarget = FindClosestTransform(_followList);
        }

        Transform FindClosestTransform(List<Transform> list)
        {
            Transform toReturn = null;
            float distance = Mathf.Infinity;
            foreach (Transform t in list)
            {
                float tempDistance = Vector3.Distance(transform.position, t.position);
                if (tempDistance <= distance)
                {
                    toReturn = t;
                    distance = tempDistance;
                }
            }
            return toReturn;
        }

        public void UpdateFollowList(float followRange)
        {
            _followList.Clear();
            Collider2D[] possibleFollowTargets = Physics2D.OverlapCircleAll(transform.position, followRange*transform.localScale.x);
            foreach(Collider2D target in possibleFollowTargets)
            {
                //Check if target is valid (food or creature)
                bool isValid = false;
                if (target.GetComponent<Food>() == null && target.attachedRigidbody?.GetComponent<IDamageable>() == null)
                {
                    continue;
                }

                switch (_enemy.Settings.CreatureDiet)
                {
                    case CreatureDiet.Carnivore:
                        if(target.GetComponent<Food>() != null)
                        {
                            if (target.GetComponent<Food>().FoodType == FoodType.Meat && target.GetComponent<Food>().size == _enemy.GetSize())
                            {
                                isValid = true;
                            }
                        }
                        else if(target.attachedRigidbody?.GetComponent<IDamageable>() != null)
                        {
                            IDamageable damageable = target.attachedRigidbody?.GetComponent<IDamageable>();
                            if ((int)damageable.GetSize() <= (int)_enemy.GetSize()) isValid = true;
                            if (damageable.GetID() == _enemy.GetID()) isValid = false;
                        }

                        break;
                    case CreatureDiet.Herbivore:
                        if (target.GetComponent<Food>() != null)
                        {
                            if (target.GetComponent<Food>().FoodType == FoodType.Fruit || target.GetComponent<Food>().FoodType == FoodType.Fungi)
                            {
                                if(target.GetComponent<Food>().size == _enemy.GetSize())
                                {
                                    isValid = true;
                                }
                            }
                        }
                        break;
                    case CreatureDiet.Omnivore:
                        if (target.GetComponent<Food>() != null)
                        {
                            isValid = true;
                        } else if (target.attachedRigidbody?.GetComponent<IDamageable>()!= null)
                        {
                            IDamageable damageable = target.attachedRigidbody?.GetComponent<IDamageable>();
                            if ((int)damageable.GetSize() <= (int)_enemy.GetSize()) isValid = true;
                            if (damageable.GetID() == _enemy.GetID()) isValid = false;
                        }
                        break;
                }

                if (isValid)
                {
                    if(target.attachedRigidbody != null)
                    {
                        if (_followList.Contains(target.attachedRigidbody.transform)) continue;
                        _followList.Add(target.attachedRigidbody.transform);
                    }
                    else
                    {
                        if (_followList.Contains(target.transform)) continue;
                        _followList.Add(target.transform);

                    }
                }
            }


        }

        public void RemoveFollowTarget(Transform target)
        {
            _followList.Remove(target); 
        }


        public void UpdateFleeList(float fleeRange)
        {
            _fleeList.Clear();
            Collider2D[] possibleFleeTargets = Physics2D.OverlapCircleAll(transform.position, fleeRange * transform.localScale.x);
            foreach (Collider2D target in possibleFleeTargets)
            {
                bool isValid = false;
                if (target.attachedRigidbody?.GetComponent<IDamageable>() == null)
                {
                    continue;
                }
                else if (target.attachedRigidbody.Equals(GetComponent<Rigidbody2D>()))
                {
                    continue;
                }

                IDamageable damageable = target.attachedRigidbody?.GetComponent<IDamageable>();
                if(target.attachedRigidbody.TryGetComponent<Enemy>(out Enemy enemy))
                {
                    if(enemy.Settings.CreatureDiet == CreatureDiet.Herbivore) //Dont flee if other is herbivore.
                    {
                        continue;
                    }
                    if (enemy.GetID() == _enemy.GetID()) //Dont flee if other species is the same. 
                    {
                        continue;
                    }
                } 

                switch (_enemy.Settings.CreatureDiet)
                {
                    case CreatureDiet.Herbivore:
                        if ((int)damageable.GetSize() >= (int)_enemy.GetSize())
                        {
                            isValid = true;
                        }
                        break;
                    case CreatureDiet.Carnivore:
                    case CreatureDiet.Omnivore:
                        if((int) damageable.GetSize() > (int)_enemy.GetSize())
                        {
                            isValid = true;
                        }

                        break;
                }
                if (isValid)
                {
                    if (_fleeList.Contains(target.attachedRigidbody.transform)) continue;
                    _fleeList.Add(target.attachedRigidbody.transform);
                }
            }
        }

        public void RemoveCreatureToFlee(Transform trans)
        {
            _fleeList.Remove(trans);
        }
    }
}
