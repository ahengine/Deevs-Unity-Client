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
        private const string DODGE_ANIMATOR_STATE = "Base Layer.Dodge.DodgeStart";
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

        protected override bool BusyForNewAction => base.BusyForNewAction || Dodging;

        [SerializeField] private AudioSource footStepSFX;
        [SerializeField] private Vector2Int landFromSkyDamage = new Vector2Int(5, 8);
        [SerializeField] private float hillOnSitRate = .1f;
        [SerializeField] private int hillOnSit = 2;
        private float lastHillOnSitTime;

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

        private void OnEnable()
        {
            if (fallingSky)
                FallingInSky();

            SetHorizontalSpeed(0, true);
        }

        protected override void Update()
        {
            base.Update();
            footStepSFX.mute = cc.VelocityY != 0 || Dodging || IsAttacking;

            if (IsSit)
                HillingOnSit();
        }

        public override void SetHorizontalSpeed(float value, bool notUserInput = false, bool reverseDirection = false, bool WalkAnimate = true)
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
            Dodging = false;
            DoUnsit();
        }

        private void HillingOnSit()
        {
            if (lastHillOnSitTime + hillOnSitRate < Time.time)
            {
                Health.Add(hillOnSit);
                lastHillOnSitTime = Time.time;
            }
        }

        #region Do

        // Base
        public bool DoSit()
        {
            if (dodgeModule.IsActive || cc.Velocity.y != 0) return false;
            ApplySit();
            return true;
        }

        private void DoUnsit()
        {
            if (!IsSit) return;

            IsSit = false;
            cc.SetAllowAction(true);
        }

        public bool DoStand()
        {
            if (!IsSit) return false;

            ApplyStand();
            return true;
        }
        public bool DoJump()
        {
            if (!jumpModule.AllowActivate() || BusyForNewAction || !AllowHorizontalInput) return false;

            ApplyJump();
            return true;
        }
        public bool DoDodge()
        {
            if (Dodging || cc.Velocity.y != 0) return false;

            if (IsAttacking)
                AttackEnd();

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
            DoUnsit();
        }
        public override void DoDamage(int damage)
        {
            base.DoDamage(damage);
            DoUnsit();
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

            if (dodgeModule.IsActive)
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
            SetHorizontalSpeed(0, true);
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
            gameObject.layer = LayerMask.NameToLayer(PLAYER_GHOST_LAYER);
            dodgeModule.DoActivate();
            animator.Play(DODGE_ANIMATOR_STATE);
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
            if (fallingSky || IsDamaging)
                return;
            animator.SetTrigger(FALL_ANIMATOR_TRIGGER);
        }
        public void FallingInSky()
        {
            animator.SetTrigger(FALLING_SKY_ANIMATOR_TRIGGER);
            jumpModule.OnGround += OnLandFromSky;
            IsDamaging = true;
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

        #endregion

        public override void DamageState(bool value, bool changeLayer = true)
        {
            DoUnsit();
            if (IsAttacking)
                AttackEnd();
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

        public override bool DoAttackByDistance(float distance, int damage,bool faceDirectionBase = true)
        {
            base.DoAttackByDistance(distance, damage, faceDirectionBase);

            if (!target)
                return false;

            Vector2 pos = tr.position;

            // Change to Ray Attack or register enemies to enemy system
            if ((faceDirectionBase && FloatHelper.InBetween(target.position.x, pos.x, pos.x + (cc.FaceDirection * distance))) ||
                FloatHelper.Distance(target.position.x,pos.x) < distance)
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
        public void DoSwordAbilityAttackByDistance(float distance, int damage, int index)
        {
            if (DoAttackByDistance(distance, damage))
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

        public void DoJumpAttackByDistance(float distance, int damage)
        {
            if (DoAttackByDistance(distance, damage,false))
            {
                var attack = target.GetComponent<SwordAbilityInteraction>();
                if (attack) attack.JumpAttack();
            }
        }
    }
}