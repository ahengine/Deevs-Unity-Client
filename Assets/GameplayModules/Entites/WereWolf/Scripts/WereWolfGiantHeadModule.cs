using UnityEngine;
using Entities.WereWolf.HeadGiant;
using IEnumerator = System.Collections.IEnumerator;

namespace Entities.WereWolf.Moudles
{
    [System.Serializable]
    public class WereWolfGiantHeadModule
    {
        private WereWolf controller;

        private const string HEAD_IN_TRIGGER_ANIMATOR = "HeadIn";
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

        public bool AllowAttack => controller && !HeadInGround || controller.DoAttack() && Target;

        public Transform Target => controller.Target;

        public bool DoAttack()
        {
            if (AllowAttack) return false;

            animator.ResetTrigger(HEAD_IN_TRIGGER_ANIMATOR);
            animator.ResetTrigger(HEAD_OUT_TRIGGER_ANIMATOR);
            animator.ResetTrigger(HEAD_OUT_SHUT_TRIGGER_ANIMATOR);

            HeadInGround = true;
            animator.SetTrigger(HEAD_IN_TRIGGER_ANIMATOR);
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
            animator.SetTrigger(success ? HEAD_OUT_TRIGGER_ANIMATOR : HEAD_OUT_SHUT_TRIGGER_ANIMATOR);
            HeadInGround = false;
        }

        public void Attack()
        {
            if (FloatHelper.TargetIsFrontOfSelf(controller.FaceDirection, controller.transform.position.x, Target.position.x) && 
                FloatHelper.Distance(controller.transform.position.x, Target.position.x) < distanceHoriozntalAttack)
                Target.GetComponent<IDamagable>().DoDamage(Random.Range(damageRange.x, damageRange.y));
        }
    }
}
