using UnityEngine;
using Entities.WereWolf.HeadGiant;
using IEnumerator = System.Collections.IEnumerator;
using Entities.Heeloy.Moudles;

namespace Entities.WereWolf.Moudles
{
    [System.Serializable]
    public class WereWolfGiantHeadModule
    {
        private WereWolf controller;

        private const string HEAD_IN_STATE_ANIMATOR = "Base Layer.Attack.GiantHead.HeadIn";
        private const string HEAD_OUT_TRIGGER_ANIMATOR = "HeadOut";
        private const string HEAD_OUT_SHUT_TRIGGER_ANIMATOR = "HeadOutShut";

        [SerializeField] private WereWolfGiantHead giantHead;
        [SerializeField] private float distanceHoriozntalAttack = 1;
        [SerializeField] private Vector2Int damageRange = new Vector2Int(10, 20);
        [SerializeField] private float delayGiantAttack = 1.5f;

        public bool HeadInGround { private set; get; }
        private Animator animator;

        public void Init(WereWolf controller, Animator animator)
        {
            this.controller = controller;
            this.animator = animator;
            giantHead.SetOwner(this);
        }

        public bool AllowAttack => controller && !HeadInGround && Target;

        public Transform Target => controller.Target;

        public bool DoAttack()
        {
            if (!AllowAttack) return false;

            controller.SetState(false);

            if(controller.IsAttacking)
                controller.AttackEnd();

            animator.ResetTrigger(HEAD_OUT_TRIGGER_ANIMATOR);
            animator.ResetTrigger(HEAD_OUT_SHUT_TRIGGER_ANIMATOR);

            HeadInGround = true;
            animator.Play(HEAD_IN_STATE_ANIMATOR);
            controller.ApplyAttack();
            controller.StartCoroutine(GiantHeadDelay());
            return true;
        }

        private IEnumerator GiantHeadDelay()
        {
            yield return new WaitForSeconds(delayGiantAttack);
            giantHead.DoAttack();
        }

        public void EndAttack(bool success)
        {
            controller.SetState(true);
            animator.SetTrigger(success ? HEAD_OUT_TRIGGER_ANIMATOR : HEAD_OUT_SHUT_TRIGGER_ANIMATOR);
            HeadInGround = false;
        }

        public void Attack()
        {
            if (controller.TargetFrontOnMe &&
                FloatHelper.Distance(controller.transform.position.x, Target.position.x) < distanceHoriozntalAttack)
            {
                Debug.Log("HeadOut Attack");
                //Target.GetComponent<IDamagable>().DoDamage(Random.Range(damageRange.x, damageRange.y));
                Target.GetComponent<ForlornWereWolfInteraction>().ApplyHitStrikeSword02Part2(controller.FaceDirection == 1, Random.Range(damageRange.x, damageRange.y));
            }
        }
    }
}
