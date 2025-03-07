using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Creature : MonoBehaviour, IDamageable
    {
        [field: SerializeField]
        public Rigidbody2D Rigidbody { get; private set; }
        
        [field: SerializeField] 
        public Body Body { get; private set; }

        [SerializeField] 
        private BodyPart[] _startParts;

        [SerializeField] 
        private Transform _hook;
        [field:SerializeField] public AudioClip[] onHitSounds { get; private set; }

        private PlayerHPBar _hpBar;

        [SerializeField]
        DefeatScreen _defeatScreenPrefab;
        DefeatScreen _defeatScreen;

        public Transform Transform { get; private set; }
        OffensiveBodyPart lastDamageSource;

        [SerializeField, HideInInspector]
        private float _maxHitpoints;
        [SerializeField, HideInInspector]
        private float _currentHitpoints;
        bool _isDead = false;

        Game _game;

        SizeStage _stage;

        private void Awake()
        {
            Transform = transform;
            foreach (BodyPart bodyPart in _startParts)
            {
                if (bodyPart != null && !Body.Contains(bodyPart))
                {
                    Body.Add(bodyPart);
                }
            }
            _game = Game.Instance;
        }

        void Start()
        {
            _hpBar = FindObjectOfType<PlayerHPBar>();
            _stage = _game.CurrentStage.StageSettings.sizeStage;
        }

        void Update()
        {
            lastDamageSource = null;
        }

        public void UpdateMaxHealth(int value)
        {
            _maxHitpoints = value;
            _currentHitpoints = value;
        }

        public void Add(BodyPart bodyPart, KeyValuePair<SplineData, BodyPartSlot> slot)
        {
            Transform trans = bodyPart.transform;
            trans.SetParent(_hook);
            Body.Add(bodyPart, slot);
        }

        public void Remove(BodyPart bodyPart)
        {
            Body.Remove(bodyPart);
        }

        public void DisableCollider()
        {
            Rigidbody.isKinematic = true;
            foreach (Collider2D collider in GetComponentsInChildren<Collider2D>())
            {
                collider.enabled = false;
            }
        }

        public void SetAlphaTo(float alpha)
        {
            foreach (SpriteRenderer spriteRenderer in GetComponentsInChildren<SpriteRenderer>())
            {
                Color c = spriteRenderer.color;
                spriteRenderer.color = new Color(c.r, c.g, c.b, alpha);
            }
            
            foreach (MeshRenderer meshRenderer in GetComponentsInChildren<MeshRenderer>())
            {
                Material material = meshRenderer.material;
                Color c = material.color;
                material.color = new Color(c.r, c.g, c.b, alpha);
            }
        }

        public void SufferDamage(float damage, OffensiveBodyPart source)
        {
            if (lastDamageSource == source) return;
            lastDamageSource = source;

            //TODO: Add OnHit Animations and Sounds
            _currentHitpoints -= Mathf.Round(damage);
            if(_hpBar == null)
            {
                _hpBar = FindObjectOfType<PlayerHPBar>();
                _hpBar.UpdateHP(_currentHitpoints / _maxHitpoints);
            } else
            {
                _hpBar.UpdateHP(_currentHitpoints / _maxHitpoints);
            }
            SoundFXManager.Instance.PlayRandomSoundClip(onHitSounds, transform, 1.5f);

            if (_currentHitpoints <= 0)
            {
                Die();
            }
        }

        public void HealDamage(float amount, bool isPercentage)
        {
            float absoluteAmount = amount;
            if (isPercentage)
            {
                absoluteAmount = amount * _maxHitpoints;
            }
            _currentHitpoints += absoluteAmount;
            _currentHitpoints = Mathf.Clamp(_currentHitpoints, 0, _maxHitpoints);
            
            if (_hpBar == null)
            {
                _hpBar = FindObjectOfType<PlayerHPBar>();
            }
            
            if(_hpBar != null)
            {
                _hpBar.UpdateHP(_currentHitpoints / _maxHitpoints);
            }
            
            if (_currentHitpoints>_maxHitpoints)
            {
                _currentHitpoints = _maxHitpoints;
            }
        }

        public void ApplyStatusEffect(StatusEffect status)
        {
            throw new NotImplementedException();
        }

        public int GetID()
        {
            return -1;
        }
        
        public SizeStage GetSize()
        {
            return _stage;
        }

        public float GetDamageFactor()
        {
            return _game.CurrentStage.StageSettings.DamageBaseValue;
        }

        public SplineData RemoveSpline()
        {
            return Body.RemoveSpline();
        }

        public void AddSpline()
        {
            Body.AddSpline();
        }

        void Die()
        {
            if(_isDead) return;
            _isDead = true;
            //TODO: Add Animations and Sounds! 
            Destroy(gameObject, 1);
            _defeatScreen = Instantiate(_defeatScreenPrefab);
            _defeatScreen.Die();
            _game.State = GameState.Defeat;
        }
    }
}
