using UnityEngine;
using CC2D;
using CC2D.Modules;
using Entities.WereWolf.HeadGiant;
using Entities.WereWolf.Moudles;

//using Entities.WereWolf.Moudles;

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

        private bool attack;

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
            if (attack) return;

            ApplyCries();
        }

        public void DoDashStrike()
        {
            if (attack) return;

            ApplyDashStrike();
        }
        public void DoStrike()
        {
            if (attack) return;

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
            ApplyAttack();
            animator.SetTrigger(CRIES_TRIGGER_ANIMATOR);
        }

        private void ApplyDashStrike()
        {
            ApplyAttack();
            animator.SetTrigger(DASH_STRIKE_TRIGGER_ANIMATOR);
        }
        private void ApplyStrike()
        {
            ApplyAttack();

            if (!firstStrike)
            {
                firstStrike = true;
                animator.SetTrigger(STRIKE_FIRST_TRIGGER_ANIMATOR);
            }
            else
                animator.SetTrigger(STRIKE_TRIGGER_ANIMATOR);
        }

        private void ApplyAttack()
        {
            attack = true;
            animator.SetTrigger(ATTACK_TRIGGER_ANIMATOR);
        }

        public void AttackEnd()
        {
            attack = false;
        }
    }
}