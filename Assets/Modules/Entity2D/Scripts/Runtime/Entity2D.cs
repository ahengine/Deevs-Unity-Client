using UnityEngine;
using CC2D;
using static UnityEngine.GraphicsBuffer;

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
        public bool IsDamaging { protected set; get; }
        public bool IsDamagable { protected set; get; } = true;
        public bool HorizontalInput { protected set; get; } = true;
        public bool AllowMove { protected set; get; } = true;
        protected virtual bool CantAttack => IsAttacking || cc.Velocity.y != 0;
        protected virtual bool BusyForNewAction => !AllowMove || IsDamaging || IsAttacking;
        [field: SerializeField] public Health Health { protected set; get; }
        protected float pushBackHorizontalSpeed;
        protected bool pushBackHorizontal;
        protected bool pushBackHorizontalAnimate = true;

        protected virtual void Awake()
        {
            tr = transform;
            cc = GetComponent<CharacterController2D>();
            if (animator = GetComponentInChildren<Animator>())
                Health.Init();
            spr = animator.GetComponent<SpriteRenderer>();
        }

        protected virtual void Update()
        {
            if (pushBackHorizontal)
                SetHorizontalSpeed(pushBackHorizontalSpeed, true, true, pushBackHorizontalAnimate);
        }

        public virtual void SetHorizontalSpeed(float value,bool notUserInput = false,bool reverseDirection = false,bool WalkAnimate = true)
        {
            if ((BusyForNewAction || !HorizontalInput) && !notUserInput) return;
            cc.SetHorizontal(value);
            if (value != 0)
                SetFaceDirection(value > 0, reverseDirection);

            FootstepSFXHandling(value, reverseDirection);
            animator.SetFloat(WALK_ANIMATOR_FLOAT, WalkAnimate? Mathf.Abs(cc.Velocity.x) : 0);
        }

        public virtual void SetFaceDirection(bool right, bool reverseDirection = false)
        {
            spr.flipX = !right;
            if (reverseDirection) spr.flipX = !spr.flipX;
        }

        protected virtual void FootstepSFXHandling(float value, bool reverseDirection)
        {

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

        protected virtual void Attack(float distance, int damage, bool many = true, bool allowMove = false)
        {
            if (many)
                tr.Ray2DAll<IDamagable>(distance, attackLayer).ForEach(enemy => enemy.DoDamage(damage));
            else
                tr.Ray2D<IDamagable>(distance, attackLayer)?.DoDamage(damage);

            ApplyAttack(allowMove);
        }
        public virtual void ApplyAttack(bool allowMove = false)
        {
            IsAttacking = true;
            SetHorizontalSpeed(0,true);
            AllowMove = allowMove;
            animator.SetBool(ATTACK_ANIMATOR_BOOL, true);
        }

        public virtual void ApplyAttack() => ApplyAttack(false);

        // Events
        public virtual void AttackEnd()
        {
            IsAttacking = false;
            AllowMove = true;
            animator.SetBool(ATTACK_ANIMATOR_BOOL, false);
        }

        public void SetAllowMove(bool allowMove) =>
            AllowMove = allowMove;

        public void SetState(bool value)
        {
            SetAllowMove(value);
            cc.SetState(value);
        }

        public virtual void DamageState(bool value,bool changeLayer = true)
        {
            if(value) ResetAllStates();
            IsDamaging = value;
            SetHorizontalSpeed(0,true);
        }

        protected virtual void ResetAllStates()
        {
            IsAttacking = false;
            IsDamaging = false;
            SetHorizontalSpeed(0, true);
        }

        public virtual void DoDamageOnSky(int damage) { Health.ApplyDamage(damage); print(name + " [Damaged]: " + damage); }

        public virtual bool DoAttackByDistance(float distance,int damage,bool faceDirectionBase = true) => false;

        #region Push Back
        public virtual void PushBack() =>
            pushBackHorizontal = true;

        public virtual void PushBackSettings(float speed, bool walkAnimate = true)
        {
            pushBackHorizontalSpeed = speed;
            pushBackHorizontalAnimate = walkAnimate;
        }
        public virtual void PushBackStop()
        {
            pushBackHorizontal = false;
            SetHorizontalSpeed(0);
        }
        #endregion

        public int OrderLayer => spr.sortingOrder;
        public void SetOrderLayer(int order) =>
             spr.sortingOrder = order;

        public void SetDamagable(bool value) =>
            IsDamagable = value;

        public void SetAllowHorizontalInput(bool value) =>
            HorizontalInput = value;

        public void SetAllowHorizontalInputAfterDelay(bool value,float delay) =>
           StartCoroutine(CoroutineHelper.CallActionWithDelay(() => HorizontalInput = value,delay));
    }
}