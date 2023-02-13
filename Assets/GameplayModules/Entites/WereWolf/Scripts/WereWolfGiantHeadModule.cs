using UnityEngine;
using Entities.WereWolf.HeadGiant;

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

        public bool HeadInGround { private set; get; }
        private Animator animator;

        public void Init(WereWolf controller, Animator animator)
        {
            this.controller = controller;
            this.animator = animator;
            giantHead.SetOwner(this);
        }

        public bool DoAttack()
        {
            if (HeadInGround) return true;

            HeadInGround = true;
            animator.SetTrigger(HEAD_IN_TRIGGER_ANIMATOR);
            giantHead.DoAttack();
            return false;
        }

        public void EndAttack(bool success)
        {
            animator.SetTrigger(success ? HEAD_OUT_TRIGGER_ANIMATOR : HEAD_OUT_SHUT_TRIGGER_ANIMATOR);
            HeadInGround = false;
        }

        public void Attack()
        {
            if (FloatHelper.TargetIsFrontOfSelf(controller.FaceDirection, controller.transform.position.x, giantHead.AttackTarget.position.x) && 
                FloatHelper.Distance(controller.transform.position.x, giantHead.AttackTarget.position.x) < distanceHoriozntalAttack)
                giantHead.AttackTarget.GetComponent<IDamagable>().DoDamage(Random.Range(damageRange.x, damageRange.y));
        }
    }
}
