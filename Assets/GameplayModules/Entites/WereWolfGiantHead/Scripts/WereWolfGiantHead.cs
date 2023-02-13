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
        [SerializeField] private Transform target;
        [field:SerializeField] public Transform AttackTarget { private set; get; }
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

        private void Update()
        {
            position.x = target.position.x;
            if(sr.sprite == null)
                tr.position = position;
        }

        public void SetOwner(WereWolfGiantHeadModule owner) => 
            this.owner = owner;

        public void DoAttack()
        {
            animator.SetTrigger(PREPARING_TRIGGER);
            StartCoroutine(CoroutineHelper.CallAction(ApplyAttack, delayAttack));
        }

        private void ApplyAttack()
        {
            AttackTargetFocused = AttackTarget.position;
            animator.SetTrigger(ATTACK_TRIGGER);
        }

        public void Attack()
        {
            animator.SetTrigger(FloatHelper.Distance(AttackTarget.position.x, AttackTargetFocused.x) < attackDistance ?
                ATTACK_SUCCESS_TRIGGER : ATTACK_FAILED_TRIGGER);

            lastAttackState = false;
        }

        public void PickUpTarget() =>
            AttackTarget.gameObject.SetActive(false);

        public void LeftTarget()
        {
            AttackTarget.position = leftTargetPoint.position;
            AttackTarget.gameObject.SetActive(true);
            AttackTarget.GetComponent<IDamagable>().DoDamage(Random.Range(damageRange.x,damageRange.y));
            lastAttackState = true;
            EndAttack();
        }

        public void EndAttack()
        {
            owner.EndAttack(lastAttackState);
        }
    }
}
