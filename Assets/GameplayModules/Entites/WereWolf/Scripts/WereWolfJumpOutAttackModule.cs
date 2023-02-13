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
        private const string JUMPOOUT_ATTACK_SUCCESS_BOOL_ANIMATION = "JumpOutAttackSuccess";

        private WereWolf controller;
        private Animator animator;
        [SerializeField] private float damagDistance = .3f;
        [SerializeField] private Vector2Int damageRange = new Vector2Int(10,20);
        [SerializeField] private Animator seekingAnimator;
        [SerializeField] private Transform dropTargetPoint;
        [SerializeField] private float seekDelay = 1.2f;
        [SerializeField] private float jumpOutDelay = 1.2f;

        public void Init(WereWolf controller, Animator animator)
        {
            this.controller = controller;
            this.animator = animator;
        }

        public void DoAttack() => JumpIn();

        private void JumpIn()
        {
            controller.SetState(false);
            controller.ApplyAttack();
            animator.SetTrigger(JUMPIN_TRIGGER_ANIMATION);
            controller.StartCoroutine(JumpOutDelay());
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
            controller.transform.position = new Vector3(controller.Target.position.x, controller.transform.position.y);
            controller.CreateGroundDamaged();
            animator.SetTrigger(JUMPOUT_TRIGGER_ANIMATION);
        }

        public void Attack()
        {
            bool successAttack =
                FloatHelper.Distance(controller.Target.position.x, controller.transform.position.x) < damagDistance;
            animator.SetBool(JUMPOOUT_ATTACK_SUCCESS_BOOL_ANIMATION, successAttack);

            if (successAttack)
            {
                controller.Target.GetComponent<IDamagable>().
                    DoDamage(Random.Range(damageRange.x, damageRange.y));
                controller.Target.gameObject.SetActive(false);
            }
        }
        public void AttackEnd()
        {
            controller.AttackEnd();
            controller.SetState(true);

            if (!controller.Target.gameObject.activeSelf)
            {
                controller.Target.position = dropTargetPoint.position;
                controller.Target.gameObject.SetActive(true);
            }
        }
    }
}