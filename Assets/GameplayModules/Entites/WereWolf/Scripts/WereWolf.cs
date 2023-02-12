using UnityEngine;
using Entities.WereWolf.Moudles;

namespace Entities.WereWolf
{
    public class WereWolf : Entity2D
    {
        private const string ATTACK_TRIGGER_ANIMATOR = "Attack";
        private const string CRIES_TRIGGER_ANIMATOR = "Cries";
        private const string DASH_STRIKE_TRIGGER_ANIMATOR = "DashStrike";
        private const string STRIKE_TRIGGER_ANIMATOR = "Strike";
        private const string STRIKE_FIRST_TRIGGER_ANIMATOR = "StrikeFirst";
        private const string FINISHER_DEATH_ANIMATOR_TRIGGER = "FinisherDeath";

        [field:SerializeField] public WereWolfGiantHeadModule GiantHeadModule { private set; get; }
        private bool firstStrike;

        protected override void Awake()
        {
            base.Awake();
            GiantHeadModule.Init(this,animator);
        }

        public override void SetHorizontalSpeed(float value)
        {
            base.SetHorizontalSpeed(value);
            spr.flipX = cc.FaceDirection == 1;
        }

        public override void DoDamage(int damage)
        {
            
        }

        // Do
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

        public void DoFinisherDeath()
        {
            if (!IsDead) return;
            animator.SetTrigger(FINISHER_DEATH_ANIMATOR_TRIGGER);
        }

        // Apply
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
    }
}