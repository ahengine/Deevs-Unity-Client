using IEnumerator = System.Collections.IEnumerator;
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
        public bool HeadInGround { private set; get; }
        private Animator animator;

        public void Init(WereWolf controller, Animator animator)
        {
            this.controller = controller;
            this.animator = animator;
        }

        public bool DoAttack(int count)
        {
            if (HeadInGround) return true;

            controller.StartCoroutine(ApplyAttackAnimation(count));

            return false;
        }

        private IEnumerator ApplyAttackAnimation(int count)
        {
            HeadInGround = true;
            animator.SetTrigger(HEAD_IN_TRIGGER_ANIMATOR);
            yield return new WaitForSeconds(2.0f);
            int counter = count;
            while (counter > 0)
            {
                giantHead.DoWaitingAttack();
                yield return new WaitForSeconds(4.0f);
                counter--;
            }
            animator.SetTrigger(Random.value > .5 ? HEAD_OUT_TRIGGER_ANIMATOR: HEAD_OUT_SHUT_TRIGGER_ANIMATOR);
            HeadInGround = false;
        }
    }
}
