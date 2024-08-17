using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Enemy: MonoBehaviour
    {
        [field: SerializeField]
        public Rigidbody2D Rigidbody { get; private set; }

        [field: SerializeField]
        public EnemySettings Settings { get; private set; }

        [field: SerializeField]
        Animator slashAnimatorPrefab;
        Animator spawnedAnimator;

        public Transform Transform { get; private set; }

        float currentHitpoints = 0;
        
        private void Start()
        {
            Transform = transform;
            currentHitpoints = Settings.HitPoints;
        }

        public void SufferDamage(float damage)
        {
            Debug.Log("I suffered " + damage + " damage! :(");


            if(spawnedAnimator == null)
            {
                spawnedAnimator = Instantiate(slashAnimatorPrefab, transform);
                spawnedAnimator.transform.localScale = Vector3.one*1.5f;
            }
            spawnedAnimator.SetTrigger("Hit");
            currentHitpoints -= damage;
            if (currentHitpoints <= 0)
            {
                currentHitpoints = 0;
                Die();
            }
        }

        public void ApplyStatusEffect(StatusEffect status)
        {

        }

        void Die()
        {
            List<Loot> lootList = Settings.GetLoot();
            foreach(Loot loot in lootList)
            {
                for(int i = 0; i<loot.maxAmount; i++)
                {
                    Vector3 positionOffset = UnityEngine.Random.insideUnitSphere * 3;
                    GameObject.Instantiate(loot.drop, transform.position + positionOffset, transform.rotation);
                }
                
            }

            Destroy(this.gameObject);
        }
    }
}
