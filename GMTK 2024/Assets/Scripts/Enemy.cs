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
            Destroy(this.gameObject);
        }
    }
}
