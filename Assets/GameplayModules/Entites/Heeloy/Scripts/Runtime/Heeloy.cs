using CC2D.Modules;
using Entities.Heeloy.Moudles;
using System;
using UnityEngine;

namespace Entities.Heeloy
{
    public class Heeloy : Entity2D
    {
        private const string PLAYER_LAYER = "Player";
        private const string PLAYER_GHOST_LAYER = "PlayerUncolidedEnemies";

        private const string SIT_ANIMATOR_TRIGGER = "Sit";
        private const string STAND_ANIMATOR_TRIGGER = "Stand";
        private const string DODGE_ANIMATOR_TRIGGER = "Dodge";
        private const string DODGE_END_ANIMATOR_TRIGGER = "DodgeEnd";
        private const string JUMP_ANIMATOR_TRIGGER = "Jump";
        private const string FALL_ANIMATOR_TRIGGER = "Fall";
        private const string ON_LAND_JUMP_ANIMATOR_TRIGGER = "OnLand";
        protected const string DEATH_SKY_ANIMATOR_TRIGGER = "DeathSky";
        protected const string FALLING_SKY_ANIMATOR_TRIGGER = "FallingSky";
        [SerializeField] private bool fallingSky;

        private JumpModule jumpModule;
        private DodgeModule dodgeModule;
        private Action<IDamagable> doAttackSpecial = null;

        public bool Dodging { private set; get; }
        [SerializeField] private SwordAttack swordAttack;
        [SerializeField] private SwordHeavyAttack swordHeavyAttack;
        public bool IsSit { private set; get; }

        [SerializeField] private Transform camTarget;
        private Vector2 camTargetPosition;
        protected override bool CantAttack => base.CantAttack || Dodging;

        protected override bool BusyForNewAction => base.BusyForNewAction && Dodging;

        [SerializeField] private AudioSource footStepSFX;
        private float pushBackHorizontalSpeed;
        private bool pushBackHorizontal;
        [SerializeField] private Vector2Int landFromSkyDamage = new Vector2Int(5, 8);

        protected override void Awake()
        {
            base.Awake();

            dodgeModule = cc.GetModule<DodgeModule>();
            jumpModule = cc.GetModule<JumpModule>();
            jumpModule.OnFall += OnFall;
            jumpModule.OnGround += OnGround;
            camTargetPosition = camTarget.localPosition;
            Health.OnDeath += ApplyDeath;

            dodgeModule.onDodgingEnd += DodgeEnd;
            dodgeModule.OnComplete += DodgeComplete;
        }

        private void Update()
        {
            footStepSFX.mute = cc.VelocityY != 0 || Dodging || IsAttacking;

            if (pushBackHorizontal)
                SetHorizontalSpeed(pushBackHorizontalSpeed, true, true);
        }

        public override void SetHorizontalSpeed(float value, bool notUserInput = false, bool reverseDirection = false)
        {
            base.SetHorizontalSpeed(value, notUserInput, reverseDirection);

            // Flip Cam
            camTargetPosition.x = cc.FaceDirection * Mathf.Abs(camTargetPosition.x);
            camTarget.localPosition = camTargetPosition;
        }

        protected override void FootstepSFXHandling(float value, bool reverseDirection)
        {
            if (value != 0)
            {
                if (!footStepSFX.isPlaying) footStepSFX.Play();
            }
            else
                if (footStepSFX.isPlaying) footStepSFX.Pause();
        }

        protected override void ResetAllStates()
        {
            base.ResetAllStates();
            Dodging = IsSit = false;
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
            if (!jumpModule.AllowActivate() || BusyForNewAction) return false;

            ApplyJump();
            return true;
        }
        public bool DoDodge()
        {
            if (!dodgeModule.AllowActivate() || BusyForNewAction) return false;
            ApplyDodge();
            return true;
        }

        // Attack
        public override bool DoAttack() => Dodging ? DoDodgeAttack() : cc.Velocity.y != 0 ? DoJumpAttack() : DoSwordAttack();
        public bool DoDodgeAttack()
        {
            if (!Dodging || IsAttacking) return false;
            IsAttacking = true;
            dodgeModule.OnComplete += ApplyAttack;
            return true;
        }
        public bool DoJumpAttack()
        {
            if (Dodging || cc.Velocity.y == 0) return false;

            ApplyAttack();
            return true;
        }
        public bool DoSwordAttack()
        {
            if (CantAttack) return false;

            swordAttack.UpdateIndex();
            ApplySwordAttack();
            return true;
        }

        public bool DoSwordAbilityAttack()
        {
            if (CantAttack) return false;

            ApplySwordAbilityAttack();
            return true;
        }

        public bool DoSwordHeavyAttack()
        {
            if (CantAttack) return false;

            swordHeavyAttack.UpdateIndex();
            ApplySwordHeavyAttack();
            return true;
        }

        // Health

        // Must be change this algorithm after demo [Falling SKY]
        public override void DoDamageOnSky(int damage)
        {
            base.DoDamageOnSky(damage);
            fallingSky = true;
        }

        public override void DoDeath()
        {
            
        }

        #endregion

        #region Apply

        private void OnLandFromSky()
        {
            int damage = UnityEngine.Random.Range(landFromSkyDamage.x, landFromSkyDamage.y);

            if (Health.Current <= damage)
                IsDead = true;

            DoDamage(damage);

            if (IsDead)
                animator.SetTrigger(DEATH_ANIMATOR_TRIGGER);

            jumpModule.OnGround -= OnLandFromSky;
        }

        public override void ApplyAttack()
        {
            base.ApplyAttack();

            if(dodgeModule.IsActive)
                dodgeModule.OnComplete -= ApplyAttack;
        }

        // Attack
        private void ApplySwordAttack()
        {
            swordAttack.Attack(animator);
            ApplyAttack();
        }
        private void ApplySwordAbilityAttack()
        {
            swordAttack.SwordAbilityAttack(animator);
            ApplyAttack();
        }
        private void ApplySwordHeavyAttack()
        {
            swordHeavyAttack.Attack(animator);
            ApplyAttack();
        }

        private void ApplyStand()
        {
            animator.SetTrigger(STAND_ANIMATOR_TRIGGER);
            IsSit = false;
        }
        private void ApplySit()
        {
            cc.SetAllowAction(false);
            SetHorizontalSpeed(0, true);
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
            gameObject.layer = LayerMask.NameToLayer(PLAYER_GHOST_LAYER);
            dodgeModule.DoActivate();
            animator.SetTrigger(DODGE_ANIMATOR_TRIGGER);
            animator.ResetTrigger(DODGE_END_ANIMATOR_TRIGGER);
            
        }
        #endregion

        #region Events
        private void DodgeEnd()
        {
            Dodging = false;
            gameObject.layer = LayerMask.NameToLayer(PLAYER_LAYER);
            animator.SetTrigger(DODGE_END_ANIMATOR_TRIGGER);
        }

        private void DodgeComplete() =>
            Dodging = false;

        private void OnFall()
        {
            animator.ResetTrigger(ON_LAND_JUMP_ANIMATOR_TRIGGER);
            if (fallingSky)
            {
                print("FALLING SKY -------------------------------------");
                FallingInSky();
                return;
            }
            animator.SetTrigger(FALL_ANIMATOR_TRIGGER);
        }
        public void FallingInSky()
        {
            animator.SetTrigger(FALLING_SKY_ANIMATOR_TRIGGER);
            jumpModule.OnGround += OnLandFromSky;
        }
        public void FallingSky(bool falling) => fallingSky = falling;
        private void OnGround()
        {
            fallingSky = false;
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

        public void PushBack() =>
            pushBackHorizontal = true;

        public void PushBackSpeed(float speed) =>
            pushBackHorizontalSpeed = speed;

        public void PushBackStop() =>
            pushBackHorizontal = false;

        #endregion

        public override void DamageState(bool value, bool changeLayer = true)
        {
            base.DamageState(value, changeLayer);

            if (value)
            {
                if (changeLayer)
                    gameObject.layer = LayerMask.NameToLayer(PLAYER_GHOST_LAYER);
            }
            else
                gameObject.layer = LayerMask.NameToLayer(PLAYER_LAYER);
        }

        // This Part Just for demo
        [SerializeField] private Transform target;

        public override bool DoAttackByDistance(float distance, int damage)
        {
            base.DoAttackByDistance(distance, damage);

            if (!target)
                return false;

            Vector2 pos = tr.position;

            // Change to Ray Attack or register enemies to enemy system
            if (FloatHelper.InBetween(target.position.x, pos.x, pos.x + (cc.FaceDirection * distance)))
            {
                var damagable = target.GetComponent<IDamagable>();

                if (damagable != null)
                {
                    damagable.DoDamage(damage);
                    doAttackSpecial?.Invoke(damagable);
                    return true;
                }
            }

            return false;
        }
        public void DoSwordAbilityAttackByDistance(float distance,int damage,int index)
        {
            if(DoAttackByDistance(distance,damage))
            {
                var attack = target.GetComponent<SwordAbilityInteraction>();
                if (!attack) return;
                switch (index)
                {
                    case 0:
                        attack.SwordAbility01();
                        break;

                    case 1:
                        attack.SwordAbility02();
                        break;

                    case 2:
                        attack.SwordAbility03();
                        break;
                }
            }
        }
    }
}