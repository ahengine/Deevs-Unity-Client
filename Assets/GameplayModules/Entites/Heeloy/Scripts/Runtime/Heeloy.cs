using UnityEngine;
using CC2D;
using CC2D.Modules;
using Entities.Heeloy.Moudles;

namespace Entities.Heeloy
{
    [RequireComponent(typeof(CharacterController2D))]
    public class Heeloy : MonoBehaviour
    {
        private const string SIT_ANIMATOR_TRIGGER = "Sit";
        private const string STAND_ANIMATOR_TRIGGER = "Stand";
        private const string WALK_ANIMATOR_INT = "Walk";
        private const string DODGE_ANIMATOR_TRIGGER = "Dodge";
        private const string DODGE_END_ANIMATOR_TRIGGER = "DodgeEnd";
        private const string JUMP_ANIMATOR_TRIGGER = "Jump";
        private const string FALL_ANIMATOR_TRIGGER = "Fall";
        private const string ON_LAND_JUMP_ANIMATOR_TRIGGER = "OnLand";
        private const string ATTACK_ANIMATOR_BOOL = "Attack";

        private Transform tr;
        private CharacterController2D cc;
        private JumpModule jumpModule;
        private DodgeModule dodgeModule;
        private Animator animator;
        private SpriteRenderer spr;
        private bool dodging;
        [SerializeField] private SwordAttack swordAttack;
        public bool IsSit { private set; get; }
        public bool IsAttacking { private set; get; }
        [SerializeField] private Transform camTarget;
        private Vector2 camPosition;

        private bool CantAttack => IsAttacking || dodging || cc.Velocity.y != 0;

        private void Awake()
        {
            tr = transform;
            animator = GetComponentInChildren<Animator>();
            spr = animator.GetComponent<SpriteRenderer>();
            cc = GetComponent<CharacterController2D>();
            dodgeModule = cc.GetModule<DodgeModule>();
            jumpModule = cc.GetModule<JumpModule>();
            jumpModule.OnFall += OnFall;
            jumpModule.OnGround += OnGround;
            camPosition = camTarget.localPosition;
        }

        private void Update()
        {
            animator.SetInteger(WALK_ANIMATOR_INT, cc.Velocity.x > 0 ? 1 : cc.Velocity.x < 0 ? -1 : 0);

            if(dodging && !dodgeModule.IsActive)
                DodgeEnd();
        }

        public void SetHorizontalSpeed(float value)
        {
            cc.SetHorizontal(value);

            spr.flipX = cc.FaceDirection == -1;

            // Flip Cam
            camPosition.x = cc.FaceDirection * Mathf.Abs(camPosition.x);
            camTarget.localPosition = camPosition;
        }

        #region Do

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

        public bool DoSwordAttack()
        {
            if(CantAttack) return false;

            swordAttack.UpdateIndex();

            ApplySwordAttack();
            return true;
        }

        #endregion

        #region Apply
        private void ApplySwordAttack()
        {
            IsAttacking = true;
            swordAttack.Attack(animator);
            animator.SetBool(ATTACK_ANIMATOR_BOOL,true);
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
            dodging = true;
            dodgeModule.DoActivate();
            animator.SetTrigger(DODGE_ANIMATOR_TRIGGER);
            animator.ResetTrigger(DODGE_END_ANIMATOR_TRIGGER);
        }
        #endregion

        #region Events
        private void DodgeEnd()
        {
            dodging = false;
            animator.SetTrigger(DODGE_END_ANIMATOR_TRIGGER);
        }
        private void OnFall()
        {
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
        public void AttackEnd()
        {
            IsAttacking = false;
            animator.SetBool(ATTACK_ANIMATOR_BOOL, false);
        }
        #endregion
    }
}