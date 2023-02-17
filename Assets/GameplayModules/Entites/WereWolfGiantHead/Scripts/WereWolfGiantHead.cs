using Entities.WereWolf.Moudles;
using UnityEngine;

namespace Entities.WereWolf.HeadGiant
{
    public class WereWolfGiantHead : MonoBehaviour
    {
        private const string ATTACK_SUCCESS_TRIGGER = "AttackSuccess";
        private const string ATTACK_FAILED_TRIGGER = "AttackFailed";
        private const string PREPARING_TRIGGER = "Preparing";
        private const string ATTACK_TRIGGER = "Attack";

        private Transform tr;
        private Vector2 position;
        [SerializeField] private float targetDistance = .4f;
        [SerializeField] private Transform leftTargetPoint;
        [SerializeField] private float attackDistance = 2;
        [SerializeField] private float delayAttack = 2;
        [SerializeField] private Vector2Int damageRange = new Vector2Int(10, 20);
        private Animator animator;
        private SpriteRenderer sr;
        public Vector2 AttackTargetFocused { private set; get; }
        private WereWolfGiantHeadModule owner;
        private bool lastAttackState;

        private void Awake()
        {
            tr = transform;
            position = tr.position;
            animator = GetComponentInChildren<Animator>();
            sr = animator.GetComponent<SpriteRenderer>();
        }

        public void SetOwner(WereWolfGiantHeadModule owner) => 
            this.owner = owner;

        public void DoAttack()
        {
            if (owner == null || !owner.AllowAttack)
                return;

            position.x = owner.Target.position.x + targetDistance;
            tr.position = position;

            animator.SetTrigger(PREPARING_TRIGGER);
            StartCoroutine(CoroutineHelper.CallActionWithDelay(()=> ApplyAttack(), delayAttack));
        }

        private void ApplyAttack()
        {
            AttackTargetFocused = owner.Target.position;
            animator.SetTrigger(ATTACK_TRIGGER);
        }

        public void Attack()
        {
            animator.SetTrigger(FloatHelper.Distance(owner.Target.position.x, AttackTargetFocused.x) < attackDistance ?
                ATTACK_SUCCESS_TRIGGER : ATTACK_FAILED_TRIGGER);

            lastAttackState = false;
        }

        public void PickUpTarget() =>
            owner.Target.gameObject.SetActive(false);

        public void LeftTarget()
        {
            owner.Target.position = leftTargetPoint.position;
            owner.Target.gameObject.SetActive(true);
            var damagableTarget = owner.Target.GetComponent<IDamagable>();
            if (damagableTarget != null) damagableTarget.DoDamageOnSky(Random.Range(damageRange.x,damageRange.y));
            lastAttackState = true;
            EndAttack();
        }

        public void EndAttack() => owner.EndAttack(lastAttackState);
    }
}
