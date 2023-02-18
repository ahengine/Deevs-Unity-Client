using UnityEngine;
using Entities.WereWolf.Moudles;
using Entities.Heeloy.Moudles;

namespace Entities.WereWolf
{
    public class WereWolf : Entity2D
    {
        private const string FUCKOFF_TRIGGER_ANIMATOR = "FuckOff";
        private const string CRIES_TRIGGER_ANIMATOR = "Cries";
        private const string DASH_STRIKE_TRIGGER_ANIMATOR = "DashStrike";
        private const string STRIKE_TRIGGER_ANIMATOR = "Strike";
        private const string STRIKE_FIRST_TRIGGER_ANIMATOR = "StrikeFirst";
        private const string FINISHER_DEATH_ANIMATOR_TRIGGER = "FinisherDeath";

        [field:SerializeField] public Transform Target { private set; get; }
        public float DistanceToTarget => Vector2.Distance(tr.position, new Vector2(Target.position.x, tr.position.y));
        public bool TargetFrontOnMe => (!spr.flipX && tr.position.x > Target.position.x) ||
                                       (spr.flipX && tr.position.x < Target.position.x);

        [field:SerializeField] public WereWolfGiantHeadModule GiantHeadModule { private set; get; }
        [field:SerializeField] public WereWolfJumpOutAttackModule JumpOutAttackModule { private set; get; }
        [field:SerializeField] private bool firstStrike;

        public bool FinisherIsStarted { private set; get; }
        public bool DeathIsCompleted { private set; get; }
        [SerializeField] private Transform finisherDeathDropTargetPoint;
        [SerializeField, Space(2)] private GameObject groundDamagedPrefab;
        [SerializeField] private float groundDamagedY;
        [SerializeField] private BloodyRain bloodyRain;
        [SerializeField,Range(0,1)] private float backwardWalkSpeedPrecentOfRun = .4f;
        [SerializeField,Space(2)] private Vector2Int fuckOffDamage;
        [SerializeField,Space(2)] private Vector2Int dashStrikeDamage;
        [SerializeField,Space(2)] private Vector2Int strike1Damage;
        [SerializeField,Space(2)] private Vector2Int strike2Part1Damage;
        [SerializeField,Space(2)] private Vector2Int strike2Part2Damage;
        [SerializeField] private Vector2 fuckOffDistanceAttack = new Vector2(.25f, .125f);
        [SerializeField] private Vector2 DashStrikeDistanceAttack = new Vector2(.25f, .175f);
        [SerializeField] private Vector2 strike1DistanceAttack = new Vector2(.25f, .125f);
        [SerializeField] private Vector2 strike2Part1DistanceAttack = new Vector2(.25f, .125f);
        [SerializeField] private Vector2 strike2Part2DistanceAttack = new Vector2(.8f, .1f);
        [SerializeField] private AudioSource runFootstep;
        [SerializeField, Space(2)] private AudioSource backWardWalkFootstep;
        [SerializeField] private float pushBackDamage = .1f;
        public bool IsBackwardMove { private set; get; }
        public bool IsDashStriking { private set; get; }
        private int attackDirection;
        [SerializeField] private float dashStrikeSpeed = .45f;
        protected override void Awake()
        {
            base.Awake();
            GiantHeadModule.Init(this,animator);
            JumpOutAttackModule.Init(this,animator);
            Health.OnDeath += ApplyDeath;
        }

        protected override void Update()
        {
            base.Update();

            if (IsDashStriking)
                SetHorizontalSpeed(dashStrikeSpeed * attackDirection, true);
        }

        public override void SetHorizontalSpeed(float value, bool notUserInput = false, bool reverseDirection = false, bool WalkAnimate = true)
        {
            if (reverseDirection)
                value *= backwardWalkSpeedPrecentOfRun;
            base.SetHorizontalSpeed(value, notUserInput, reverseDirection);
        }
        public override void SetFaceDirection(bool right, bool reverseDirection = false)
        {
            base.SetFaceDirection(!right, reverseDirection);
        }
        protected override void FootstepSFXHandling(float value, bool reverseDirection)
        {
            if (!reverseDirection)
            {
                if (backWardWalkFootstep.isPlaying) backWardWalkFootstep.Pause();

                if (value != 0)
                {
                    if (!runFootstep.isPlaying) runFootstep.Play();
                }
                else
                    if (runFootstep.isPlaying) runFootstep.Pause();
            }
            else
            {
                if (backWardWalkFootstep.isPlaying) runFootstep.Pause();

                if (value != 0)
                {
                    if (!backWardWalkFootstep.isPlaying) backWardWalkFootstep.Play();
                }
                else
                    if (backWardWalkFootstep.isPlaying) backWardWalkFootstep.Pause();
            }

        }

        // Do
        public override void DoDamage(int damage)
        {
            if (IsAttacking || IsDead) return;

            base.DoDamage(damage);

            ResetAllStates();
            tr.position += new Vector3(pushBackDamage * (tr.position.x < Target.position.x ? -1 : 1),0);
        }
        protected override void ResetAllStates()
        {
            IsDamaging = false;
        }
        public void DoFinisherDeath()
        {
            if (FinisherIsStarted) return;

            ApplyFinisherDeath();
        }
        public void DoCries(bool force = false)
        {
            if (IsAttacking && !force) return;
            SetDamagable(false);
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
        public bool DoFuckOff()
        {
            if(IsAttacking) return false;

            ApplyFuckOff();
            return true;
        }

        // Apply
        public override void AttackEnd()
        {
            base.AttackEnd();
            SetDamagable(true);
        }
        private void ApplyFuckOff()
        {
            ApplyAttack();
            animator.SetTrigger(FUCKOFF_TRIGGER_ANIMATOR);
        }
        private void ApplyFinisherDeath()
        {
            FinisherIsStarted = true;
            Target.gameObject.SetActive(false);
            animator.SetTrigger(FINISHER_DEATH_ANIMATOR_TRIGGER);
        }
        private void ApplyCries()
        {
            ApplyAttack();
            animator.SetTrigger(CRIES_TRIGGER_ANIMATOR);
        }

        public void DoBloodyRain() => bloodyRain.Do();

        private void ApplyDashStrike()
        {
            ApplyAttack();
            SetDamagable(false);
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
            DeathIsCompleted = true;
        }

        public void SetTarget(Transform target) =>
            Target = target;
        public void CreateGroundDamaged() =>
            Instantiate(groundDamagedPrefab, new Vector3(tr.position.x, groundDamagedY),Quaternion.identity);
        public void SetBackwardMove(bool value) =>
            IsBackwardMove = value;
        public void ApplyFuckOffDamage()
        {
            if (!TargetFrontOnMe || CheckCantAttackDistance(fuckOffDistanceAttack)) return;
            
            var reactable = Target.GetComponent<ForlornWereWolfInteraction>();
            if (reactable)
                reactable.ApplyHitFuckOff(spr.flipX, Random.Range(fuckOffDamage.x, fuckOffDamage.y));
        }
        public void ApplyDashStrikeDamage()
        {
            if (!TargetFrontOnMe || CheckCantAttackDistance(DashStrikeDistanceAttack)) return;

            var reactable = Target.GetComponent<ForlornWereWolfInteraction>();
            if (reactable)
                reactable.ApplyHitDashStrikeSword(spr.flipX, Random.Range(dashStrikeDamage.x, dashStrikeDamage.y));
        }
        public void ApplyStrike01Damage()
        {
            if (!TargetFrontOnMe || CheckCantAttackDistance(strike1DistanceAttack)) return;

            var reactable = Target.GetComponent<ForlornWereWolfInteraction>();
            if (reactable)
                reactable.ApplyHitStrikeSword01(Random.Range(strike1Damage.x, strike1Damage.y));
        }
        public void ApplyStrike02Part1Damage()
        {
            if (!TargetFrontOnMe || CheckCantAttackDistance(strike2Part1DistanceAttack)) 
                return; 

            var reactable = Target.GetComponent<ForlornWereWolfInteraction>();
            if (reactable)
                reactable.ApplyHitStrikeSword02Part1(Random.Range(strike2Part1Damage.x, strike2Part1Damage.y));
        }

        public void ApplyStrike02Part2Damage()
        {
            if (!TargetFrontOnMe || DistanceToTarget > strike2Part2DistanceAttack.x || Target.position.y > tr.position.y + strike2Part2DistanceAttack.y) return;

            var reactable = Target.GetComponent<ForlornWereWolfInteraction>();
            if (reactable)
                reactable.ApplyHitStrikeSword02Part2(spr.flipX, Random.Range(strike2Part2Damage.x, strike2Part2Damage.y));
        }
        public void ApplyAttackDamage(float distance,int damage)
        {
            Vector2 attackRange = new Vector2(tr.position.x, 0);
            attackRange.y = cc.FaceDirection * distance;
            if (FloatHelper.InBetween(Target.position.x, attackRange.x, attackRange.y))
                Target.GetComponent<IDamagable>().DoDamage(damage);
        }

        private bool CheckCantAttackDistance(Vector2 distance) =>
            DistanceToTarget > distance.x || Target.position.y > tr.position.y + distance.y;

        public void DashStriking(bool state)
        {
            attackDirection = cc.FaceDirection;
            IsDashStriking = state;
            AllowMove = !state;
        }
    }
}