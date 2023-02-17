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
        public bool TargetFrontOnMe => (cc.FaceDirection == -1 && tr.position.x > Target.position.x) ||
                                       (cc.FaceDirection == 1 && tr.position.x < Target.position.x);

        [field:SerializeField] public WereWolfGiantHeadModule GiantHeadModule { private set; get; }
        [field:SerializeField] public WereWolfJumpOutAttackModule JumpOutAttackModule { private set; get; }
        [field:SerializeField] private bool firstStrike;

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
        [SerializeField] private float fuckOffDistanceAttack;
        [SerializeField] private float DashStrikeDistanceAttack = .25f;
        [SerializeField] private float strike1DistanceAttack = .25f;
        [SerializeField] private float strike2Part1DistanceAttack = .25f;
        [SerializeField] private float strike2Part2DistanceAttack = .75f;
        [SerializeField] private AudioSource runFootstep;
        [SerializeField, Space(2)] private AudioSource backWardWalkFootstep;
        public bool IsBackwardMove { private set; get; }
        public bool IsDashStriking { private set; get; }
        private int attackDirection;
        [SerializeField] private float dashStrikeSpeed = .45f;
        protected override void Awake()
        {
            base.Awake();
            GiantHeadModule.Init(this,animator);
            JumpOutAttackModule.Init(this,animator);
        }

        private void Update()
        {
            if (IsDashStriking)
                SetHorizontalSpeed(dashStrikeSpeed * attackDirection, true);
        }

        public override void SetHorizontalSpeed(float value, bool notUserInput = false, bool reverseDirection = false)
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
            base.DoDamage(damage);
        }
        public void DoFinisherDeath()
        {
            if (DeathIsCompleted) return;

            ApplyFinisherDeath();
        }
        public void DoCries(bool force = false)
        {
            if (IsAttacking && !force) return;

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
        private void ApplyFuckOff()
        {
            ApplyAttack();
            animator.SetTrigger(FUCKOFF_TRIGGER_ANIMATOR);
        }
        private void ApplyFinisherDeath()
        {
            DeathIsCompleted = true;
            Target.gameObject.SetActive(false);
            animator.SetTrigger(FINISHER_DEATH_ANIMATOR_TRIGGER);
        }
        private void ApplyCries()
        {
            ApplyAttack();
            bloodyRain.Do();
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
        public void CreateGroundDamaged() =>
            Instantiate(groundDamagedPrefab, new Vector3(tr.position.x, groundDamagedY),Quaternion.identity);
        public void SetBackwardMove(bool value) =>
            IsBackwardMove = value;
        public void ApplyFuckOffDamage()
        {
            if (!TargetFrontOnMe || DistanceToTarget > fuckOffDistanceAttack) return;

            var reactable = Target.GetComponent<ForlornWereWolfInteraction>();
            if (reactable)
                reactable.ApplyHitFuckOff(spr.flipX, Random.Range(fuckOffDamage.x, fuckOffDamage.y));
        }
        public void ApplyDashStrikeDamage()
        {
            if (!TargetFrontOnMe || DistanceToTarget > DashStrikeDistanceAttack) return;

            var reactable = Target.GetComponent<ForlornWereWolfInteraction>();
            if (reactable)
                reactable.ApplyHitDashStrikeSword(spr.flipX, Random.Range(dashStrikeDamage.x, dashStrikeDamage.y));
        }
        public void ApplyStrike01Damage()
        {
            if (!TargetFrontOnMe || DistanceToTarget > strike1DistanceAttack) return;

            var reactable = Target.GetComponent<ForlornWereWolfInteraction>();
            if (reactable)
                reactable.ApplyHitStrikeSword01(Random.Range(strike1Damage.x, strike1Damage.y));
        }
        public void ApplyStrike02Part1Damage()
        {
            if (!TargetFrontOnMe || DistanceToTarget > strike2Part1DistanceAttack) return;

            var reactable = Target.GetComponent<ForlornWereWolfInteraction>();
            if (reactable)
                reactable.ApplyHitStrikeSword02Part1(Random.Range(strike2Part1Damage.x, strike2Part1Damage.y));
        }

        public void ApplyStrike02Part2Damage()
        {
            if (!TargetFrontOnMe || DistanceToTarget > strike2Part2DistanceAttack) return;

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

        public void DashStriking(bool state)
        {
            attackDirection = cc.FaceDirection;
            IsDashStriking = state;
            AllowMove = !state;
        }
    }
}