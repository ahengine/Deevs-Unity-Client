using UnityEngine;

namespace Entities.WereWolf.HeadGiant
{
    public class WereWolfGiantHead : MonoBehaviour
    {
        private const string ATTACK_SUCCESS_TRIGGER = "AttackSuccess";
        private const string ATTACK_FAILED_TRIGGER = "AttackFailed";
        private const string PREPARING_TRIGGER = "Preparing";

        private Transform tr;
        private Vector2 position;
        [SerializeField] private Transform target;
        private Animator animator;
        private SpriteRenderer sr;

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

        public void DoWaitingAttack()
        {
            if(applyWaitingAttackAnimationCoroutine != null)
                StopCoroutine(applyWaitingAttackAnimationCoroutine);

            applyWaitingAttackAnimationCoroutine = StartCoroutine(ApplyWaitingAttackAnimation());
        }

        private Coroutine applyWaitingAttackAnimationCoroutine;
        private System.Collections.IEnumerator ApplyWaitingAttackAnimation()
        {
            DoPreparing();
            yield return new WaitForSeconds(2.0f);
            DoAttack();
            yield return new WaitForSeconds(2.0f);
            applyWaitingAttackAnimationCoroutine = null;
        }

        public void DoAttack() =>
            animator.SetTrigger(Random.value > .5 ? ATTACK_SUCCESS_TRIGGER : ATTACK_FAILED_TRIGGER);

        public void DoPreparing() =>
            animator.SetTrigger(PREPARING_TRIGGER);
    }
}
