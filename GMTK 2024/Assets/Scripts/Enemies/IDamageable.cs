using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public interface IDamageable
    {
        Transform Transform { get; }
        public void SufferDamage(float damage);
        public void ApplyStatusEffect(StatusEffect status);
        public int GetID();
    }
}
