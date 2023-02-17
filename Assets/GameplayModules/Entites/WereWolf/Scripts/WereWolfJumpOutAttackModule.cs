using UnityEngine;
using IEnumerator = System.Collections.IEnumerator;

namespace Entities.WereWolf.Moudles
{
    [System.Serializable]
    public class WereWolfJumpOutAttackModule
    {
        private const string SEEK_TRIGGER_ANIMATION = "Seek";
        private const string JUMPIN_TRIGGER_ANIMATION = "JumpInAttack";
        private const string JUMPOUT_TRIGGER_ANIMATION = "JumpOutAttack";
        private const string JUMPOOUT_ATTACK_SUCCESS_TRIGGER_ANIMATION = "JumpOutAttackSuccess";

        private WereWolf owner;
        private Animator animator;
        [SerializeField] private float damagDistance = .3f;
        [SerializeField] private Vector2Int damageRange = new Vector2Int(10,20);
        [SerializeField] private Animator seekingAnimator;
        [SerializeField] private Transform dropTargetPoint;
        [SerializeField] private float seekDelay = 1.2f;
        [SerializeField] private float jumpOutDelay = 1.2f;

        public void Init(WereWolf controller, Animator animator)
        {
            this.owner = controller;
            this.animator = animator;
        }

        public void DoAttack() => JumpIn();

        private void JumpIn()
        {
            owner.SetState(false);
            owner.ApplyAttack();
            animator.SetTrigger(JUMPIN_TRIGGER_ANIMATION);
            owner.StartCoroutine(JumpOutDelay());
        }

        private IEnumerator JumpOutDelay()
        {
            yield return new WaitForSeconds(seekDelay);
            seekingAnimator.SetTrigger(SEEK_TRIGGER_ANIMATION);
            yield return new WaitForSeconds(jumpOutDelay);
            JumpOut();
        }
        public void JumpOut()
        {
            owner.transform.position = new Vector3(owner.Target.position.x, owner.transform.position.y);
            owner.CreateGroundDamaged();
            animator.SetTrigger(JUMPOUT_TRIGGER_ANIMATION);
        }

        public void Attack()
        {
            bool successAttack =
                FloatHelper.Distance(owner.Target.position.x, owner.transform.position.x) < damagDistance;

            if(successAttack)
                animator.SetTrigger(JUMPOOUT_ATTACK_SUCCESS_TRIGGER_ANIMATION);

            Debug.Log("Jump Out Attack State: " + successAttack);

            if (successAttack)
            {
                owner.Target.GetComponent<IDamagable>().
                    DoDamage(Random.Range(damageRange.x, damageRange.y));
                owner.Target.gameObject.SetActive(false);
            }
        }
        public void AttackEnd()
        {
            owner.AttackEnd();
            owner.SetState(true);

            if (!owner.Target.gameObject.activeSelf)
            {
                owner.Target.position = dropTargetPoint.position;
                owner.Target.gameObject.SetActive(true);
            }
        }

        public void DamageEnd() =>
            owner.DamageState(false);
    }
}