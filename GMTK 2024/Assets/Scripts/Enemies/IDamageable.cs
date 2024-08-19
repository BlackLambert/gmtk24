using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public interface IDamageable
    {
        Transform Transform { get; }
        public void SufferDamage(float damage, OffensiveBodyPart source);
        public void ApplyStatusEffect(StatusEffect status);
        public int GetID();
        public SizeStage GetSize();
    }
}
