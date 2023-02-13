using UnityEngine;
using Entities.WereWolf.Moudles;

namespace Entities.WereWolf
{
    public class WereWolf : Entity2D
    {
        private const string CRIES_TRIGGER_ANIMATOR = "Cries";
        private const string DASH_STRIKE_TRIGGER_ANIMATOR = "DashStrike";
        private const string STRIKE_TRIGGER_ANIMATOR = "Strike";
        private const string STRIKE_FIRST_TRIGGER_ANIMATOR = "StrikeFirst";
        private const string FINISHER_DEATH_ANIMATOR_TRIGGER = "FinisherDeath";

        [field:SerializeField] public Transform Target { private set; get; }
        [field:SerializeField] public WereWolfGiantHeadModule GiantHeadModule { private set; get; }
        [field:SerializeField] public WereWolfJumpOutAttackModule JumpOutAttackModule { private set; get; }
        [field:SerializeField] private bool firstStrike;

        private bool deathIsCompleted;
        [SerializeField] private Transform finisherDeathDropTargetPoint;

        protected override void Awake()
        {
            base.Awake();
            GiantHeadModule.Init(this,animator);
            JumpOutAttackModule.Init(this,animator);
        }

        public override void SetHorizontalSpeed(float value)
        {
            base.SetHorizontalSpeed(value);
            spr.flipX = cc.FaceDirection == 1;
        }

        public override void DoDamage(int damage)
        {
            base.DoDamage(damage);
        }

        // Do
        public void DoFinisherDeath()
        {
            if (deathIsCompleted) return;

            ApplyFinisherDeath();
        }
        public void DoCries()
        {
            if (IsAttacking) return;

            ApplyCries();
        }

        public void DoDashStrike()
        {
            if (IsAttacking) return;

            ApplyDashStrike();
        }
        public void DoStrike()
        {
            if (IsAttacking) return;

            ApplyStrike();
        }

        // Apply
        private void ApplyFinisherDeath()
        {
            deathIsCompleted = true;
            Target.gameObject.SetActive(false);
            animator.SetTrigger(FINISHER_DEATH_ANIMATOR_TRIGGER);
        }

        private void ApplyCries()
        {
            ApplyAttack(2, 5);
            animator.SetTrigger(CRIES_TRIGGER_ANIMATOR);
        }

        private void ApplyDashStrike()
        {
            ApplyAttack(2, 5);
            animator.SetTrigger(DASH_STRIKE_TRIGGER_ANIMATOR);
        }
        private void ApplyStrike()
        {
            ApplyAttack(2,5);

            if (!firstStrike)
            {
                firstStrike = true;
                animator.SetTrigger(STRIKE_FIRST_TRIGGER_ANIMATOR);
            }
            else
                animator.SetTrigger(STRIKE_TRIGGER_ANIMATOR);
        }

        public void ApplyHeadOutAttack() =>
            GiantHeadModule.Attack();

        public void ApplyFinisherDeathEnd()
        {
            Target.gameObject.SetActive(true);
            finisherDeathDropTargetPoint.localPosition = 
                new Vector3(-FaceDirection * finisherDeathDropTargetPoint.localPosition.x, finisherDeathDropTargetPoint.localPosition.y);
            Target.position = finisherDeathDropTargetPoint.position;
        }

        public void SetTarget(Transform target) =>
            Target = target;
    }
}