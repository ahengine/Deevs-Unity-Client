using UnityEngine;
using CC2D;

namespace Entities
{
    [RequireComponent(typeof(CharacterController2D))]
    public abstract class Entity2D : MonoBehaviour, IDamagable
    {
        protected const string WALK_ANIMATOR_FLOAT = "Walk";
        protected const string ATTACK_ANIMATOR_BOOL = "Attack";
        protected const string DEATH_ANIMATOR_TRIGGER = "Death";
        protected const string DEATH_INDEX_ANIMATOR_INT = "DeathIndex";
        [SerializeField] protected int deathAnimationCount = 1;
        protected Transform tr;
        protected CharacterController2D cc;
        protected Animator animator;
        protected SpriteRenderer spr;
        [SerializeField] protected LayerMask attackLayer;
        public int FaceDirection => cc.FaceDirection;

        public bool IsDead { protected set; get; }
        public bool IsAttacking { protected set; get; }
        protected virtual bool CantAttack => IsAttacking || cc.Velocity.y != 0;
        [field: SerializeField] public Health Health { protected set; get; }

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
            if (spr) spr.flipX = cc.FaceDirection == -1;
            animator.SetFloat(WALK_ANIMATOR_FLOAT, Mathf.Abs(cc.Velocity.x));
        }

        // Do
        public virtual void DoPlayAnimation(string name) => animator.Play(name);
        public virtual void DoAddHealth(int value) => ApplyAddHealth(value);
        public virtual bool DoAttack() => !IsAttacking;
        public virtual void DoDamage(int damage) => Health.ApplyDamage(damage);
        public virtual void DoDeath()
        {
            if (IsDead) return;

            ApplyDeath();
        }

        // Apply
        public virtual void ApplyAddHealth(int value) => Health.Add(value);
        protected virtual void ApplyDamage(int damage)
        {
            if (IsDead) return;

            if (Health.Current >= damage)
                Health.ApplyDamage(damage);
            else
                ApplyDeath();
        }
        protected virtual void ApplyDeath()
        {
            IsDead = true;
            cc.SetAllowAction(false);
            Health.ApplyDeath();

            if (animator)
            {
                animator.SetTrigger(DEATH_ANIMATOR_TRIGGER);
                if (deathAnimationCount > 1) animator.SetInteger(DEATH_INDEX_ANIMATOR_INT, Random.Range(0, deathAnimationCount));
            }
        }
        protected virtual void ApplyAttack(float distance, int damage, bool many = true)
        {
            IsAttacking = true;
            animator.SetBool(ATTACK_ANIMATOR_BOOL, true);
            if (many)
                tr.Ray2DAll<IDamagable>(distance, attackLayer).ForEach(enemy => enemy.DoDamage(damage));
            else
                tr.Ray2D<IDamagable>(distance, attackLayer)?.DoDamage(damage);
        }

        // Events
        public virtual void AttackEnd()
        {
            IsAttacking = false;
            animator.SetBool(ATTACK_ANIMATOR_BOOL, false);
        }
    }
}