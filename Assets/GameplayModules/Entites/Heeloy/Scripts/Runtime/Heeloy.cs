using UnityEngine;
using CC2D.Modules;
using Entities.Heeloy.Moudles;

namespace Entities.Heeloy
{
    public class Heeloy : Entity2D
    {
        private const string SIT_ANIMATOR_TRIGGER = "Sit";
        private const string STAND_ANIMATOR_TRIGGER = "Stand";
        private const string WALK_ANIMATOR_INT = "Walk";
        private const string DODGE_ANIMATOR_TRIGGER = "Dodge";
        private const string DODGE_END_ANIMATOR_TRIGGER = "DodgeEnd";
        private const string JUMP_ANIMATOR_TRIGGER = "Jump";
        private const string FALL_ANIMATOR_TRIGGER = "Fall";
        private const string ON_LAND_JUMP_ANIMATOR_TRIGGER = "OnLand";
        private const string FUCK_OFF_REACT_ANIMATOR_TRIGGER = "FuckoffReact";

        private JumpModule jumpModule;
        private DodgeModule dodgeModule;

        public bool Dodging { private set; get; }
        [SerializeField] private SwordAttack swordAttack;
        [SerializeField] private SwordHeavyAttack swordHeavyAttack;
        public bool IsSit { private set; get; }

        [SerializeField] private Transform camTarget;
        private Vector2 camPosition;
        protected override bool CantAttack => base.CantAttack || Dodging;

        protected override void Awake()
        {
            base.Awake();

            dodgeModule = cc.GetModule<DodgeModule>();
            jumpModule = cc.GetModule<JumpModule>();
            jumpModule.OnFall += OnFall;
            jumpModule.OnGround += OnGround;
            camPosition = camTarget.localPosition;
        }

        private void Update()
        {
            if(Dodging && !dodgeModule.IsActive)
                DodgeEnd();
        }

        public override void SetHorizontalSpeed(float value)
        {
            base.SetHorizontalSpeed(value);

            // Flip Cam
            camPosition.x = cc.FaceDirection * Mathf.Abs(camPosition.x);
            camTarget.localPosition = camPosition;
        }

        #region Do

        // Base
        public bool DoSit()
        {
            if (dodgeModule.IsActive || cc.Velocity.y != 0) return false;

            ApplySit();
            return true;
        }
        public bool DoStand()
        {
            if (!IsSit) return false;

            ApplyStand();
            return true;
        }
        public bool DoJump()
        {
            if (!jumpModule.AllowActivate()) return false;

            ApplyJump();
            return true;
        }
        public bool DoDodge()
        {
            if (!dodgeModule.AllowActivate()) return false;
            ApplyDodge();
            return true;
        }

        // Attack
        public override bool DoAttack() => Dodging ? DoDodgeAttack() : cc.Velocity.y != 0 ? DoJumpAttack() : DoSwordAttack();
        public bool DoDodgeAttack()
        {
            if (!Dodging) return false;
            Dodging = false;
            ApplyAttack(10,5,true);
            return true;
        }
        public bool DoJumpAttack()
        {
            if (Dodging || cc.Velocity.y == 0) return false;

            ApplyAttack(10, 5, true);
            return true;
        }
        public bool DoSwordAttack()
        {
            if (CantAttack) return false;

            swordAttack.UpdateIndex();
            ApplySwordAttack();
            return true;
        }
        public bool DoSwordHeavyAttack()
        {
            if (CantAttack) return false;

            swordHeavyAttack.UpdateIndex();
            ApplySwordHeavyAttack();
            return true;
        }
        public bool DoFuckOffReact()
        {
            if (CantAttack) return false;
            ApplyFuckoffReact();
            return true;
        }

        public bool DoForlornHitDashStrikeSword()
        {
            ApplyFuckoffReact();
            return true;
        }

        // Health
        public override void DoDamage(int damage) { }
        public override void DoDeath()
        {
            
        }

        #endregion

        #region Apply

        // Attack
        private void ApplySwordAttack()
        {
            swordAttack.Attack(animator);
            ApplyAttack(10, 5, true);
        }
        private void ApplySwordHeavyAttack()
        {
            swordHeavyAttack.Attack(animator);
            ApplyAttack(10, 5, true);
        }
        private void ApplyFuckoffReact()
        {
            animator.SetTrigger(FUCK_OFF_REACT_ANIMATOR_TRIGGER);
            ApplyAttack(10, 5, true);
        }

        private void ApplyStand()
        {
            animator.SetTrigger(STAND_ANIMATOR_TRIGGER);
            IsSit = false;
        }
        private void ApplySit()
        {
            cc.SetAllowAction(false);
            animator.SetTrigger(SIT_ANIMATOR_TRIGGER);
            IsSit = true;
        }
        private void ApplyJump()
        {
            jumpModule.DoActivate();
            animator.SetTrigger(JUMP_ANIMATOR_TRIGGER);
        }
        public void ApplyDodge()
        {
            Dodging = true;
            dodgeModule.DoActivate();
            animator.SetTrigger(DODGE_ANIMATOR_TRIGGER);
            animator.ResetTrigger(DODGE_END_ANIMATOR_TRIGGER);
        }
        #endregion

        #region Events
        private void DodgeEnd()
        {
            Dodging = false;
            animator.SetTrigger(DODGE_END_ANIMATOR_TRIGGER);
        }
        private void OnFall()
        {
            animator.ResetTrigger(ON_LAND_JUMP_ANIMATOR_TRIGGER);
            animator.SetTrigger(FALL_ANIMATOR_TRIGGER);
        }
        private void OnGround()
        {
            animator.SetTrigger(ON_LAND_JUMP_ANIMATOR_TRIGGER);
        }

        // Call from Animation Event
        public void OnStand()
        {
            cc.SetAllowAction(true);
        }
        // On Dodge End | On Fall Land
        public void StandUp()
        {
            animator.SetTrigger(STAND_ANIMATOR_TRIGGER);
        }

        #endregion
    }
}