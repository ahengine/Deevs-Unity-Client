using UnityEngine;
using CC2D;

namespace Entities
{
    [RequireComponent(typeof(CharacterController2D))]
    public abstract class Entity2D : MonoBehaviour, IDamagable
    {
        protected const string DEATH_ANIMATOR_TRIGGER = "Death";
        protected const string DEATH_INDEX_ANIMATOR_INT = "DeathIndex";
        [SerializeField] protected int deathAnimationCount = 1;
        protected Transform tr;
        protected CharacterController2D cc;
        protected Animator animator;
        protected SpriteRenderer spr;

        public bool IsDead { protected set; get; }
        [field:SerializeField] public Health Health { protected set; get; }

        public virtual void DoAddHealth(int value) => ApplyAddHealth(value);

        public virtual void ApplyAddHealth(int value)
        {
            Health.Add(value);
        }

        public abstract void DoDamage(int damage);
        protected virtual void ApplyDamage(int damage)
        {
            if (IsDead) return;

            if (Health.Current >= damage)
                Health.ApplyDamage(damage);
            else
            {
                ApplyDeath();
            }
        }

        public virtual void DoDeath()
        {
            if (IsDead) return;

            ApplyDeath();
        }
        protected virtual void ApplyDeath()
        {
            cc.SetAllowAction(false);
            Health.ApplyDeath();

            if (animator)
            {
                animator.SetTrigger(DEATH_ANIMATOR_TRIGGER);
                if(deathAnimationCount > 1) animator.SetInteger(DEATH_INDEX_ANIMATOR_INT, Random.Range(0, deathAnimationCount));
            }
        }

        protected virtual void Awake()
        {
            tr = transform;
            cc = GetComponent<CharacterController2D>();
            if (animator = GetComponentInChildren<Animator>())
            Health.Init();
                spr = animator.GetComponent<SpriteRenderer>();
        }

        public virtual void SetHorizontalSpeed(float value)
        {
            cc.SetHorizontal(value);

            if(spr) spr.flipX = cc.FaceDirection == -1;
        }
    }
}