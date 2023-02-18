using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Entities.WereWolf.AI
{
    [RequireComponent(typeof(WereWolf))]
    public class WereWolfAI : MonoBehaviour
    {
        public WereWolf Owner { private set; get; }
        private Action locomotionAction;

        [Header("Locomotion")]
        [SerializeField] private float minDistanceLocomotion = .05f;
        [SerializeField] private Vector2 walkRangeDistance = new Vector2(1.5f,5);
        [SerializeField] private float backwardDistance = .4f;
        [SerializeField] private int backwardComboDamageCount = 3;
        [Header("Melee Attack")]
        [SerializeField] private WereWolfAttackData fuckOff;
        [SerializeField] private WereWolfAttackData strike;
        [SerializeField] private WereWolfAttackData dashStrike;
        [SerializeField] private float meleeAttackRate = 1;
        
        [Header("Distance Attack")]
        [SerializeField] private WereWolfAttackData cries;
        [SerializeField] private WereWolfAttackData jumpInOut;
        [SerializeField] private WereWolfAttackData giantHead;
        [SerializeField] private float distanceAttackRate = 6;
        [SerializeField] private float JumpInOutAttackPlayOnHealthPrecent = .7f;
        [SerializeField] private float giantHeadAttackPlayOnHealthPrecent = .2f;

        private float attackLastTime;
        [SerializeField] private float targetDistance;

        [SerializeField] private LocomotionStates locomotionStates;
        [SerializeField] private KeyCode finisherKey = KeyCode.E;
        [SerializeField] private float finisherDistance = .3f;
        private bool finisherIsStart;
        [SerializeField] private UnityEvent OnPhaseTwo;
        [SerializeField] private UnityEvent OnDeathWaitForFinisher;

        private int comboDamageCounter;
        [SerializeField] private float comboDamageRate = .1f;
        private float lastComboDamage;

        private float SpeedToTarget =>
            Owner.Target.position.x > transform.position.x ? 1 : -1;

        private IDamagable targetDamagable;

        private void Awake()
        {
            Owner = GetComponent<WereWolf>();
        }

        private void Start()
        {
            Owner.Health.OnDamage += Phase2;
            Owner.Health.OnDamage += JumpInOutAttackOnDamaged;
            Owner.Health.OnDamage += GiantHeadOnDamaged;
            Owner.Health.OnDamage += damage => OnComboDamage();
            Owner.Health.OnDeath += ()=> OnDeathWaitForFinisher.Invoke();
            attackLastTime = Time.time;
            targetDamagable = Owner.Target.GetComponent<IDamagable>();
            GoToIdle();
        }

        private void Update()
        {
            targetDistance = Owner.DistanceToTarget;

            if (Owner.IsDead)
            {
                if (!finisherIsStart && !Owner.FinisherIsStarted)
                    if (targetDistance < finisherDistance)
                        if (Input.GetKeyDown(finisherKey))
                        {
                            finisherIsStart = true;
                            Owner.DoFinisherDeath();
                        }
                return;
            }

            if (!Owner.IsAttacking && !Owner.IsDamaging && !targetDamagable.IsDamaging)
            {
                if (targetDistance > minDistanceLocomotion)
                    locomotionAction();
                else
                    Owner.SetHorizontalSpeed(0, true);
                Attack();
            }
            else
                attackLastTime = Time.time;
        }

        #region Locomotion States
        private void GoToIdle()
        {
            locomotionStates = LocomotionStates.Idle;
            Owner.SetHorizontalSpeed(0);
            locomotionAction = Idle;
        }
        private void Idle()
        {   
           Owner.SetFaceDirection(Owner.Target.position.x > transform.position.x);

            if (targetDistance > walkRangeDistance.x)
            {
                GoToWalk();
                return;
            }
        }

        private void GoToWalk()
        {
            locomotionStates = LocomotionStates.Walk;
            locomotionAction = Walk;
        }
        private void Walk()
        {
            Owner.SetHorizontalSpeed(SpeedToTarget);

            if (!FloatHelper.InBetween(targetDistance, walkRangeDistance))
                GoToIdle();
        }

        private void GoToBackwardWalk()
        {
            locomotionStates = LocomotionStates.BackwardWalk;
            locomotionAction = BackwardWalk;
        }
        private void BackwardWalk()
        {
            Owner.SetHorizontalSpeed(-SpeedToTarget, false,true);

            if (targetDistance > backwardDistance)
                GoToIdle();
        }
        #endregion

        private void Attack()
        {
            if (targetDistance < dashStrike.distance.y)
            {
                if (attackLastTime + meleeAttackRate > Time.time)
                    return;

                attackLastTime = Time.time;

                AttackRandom(new WereWolfAttackData[] { strike, fuckOff, dashStrike });
            }
            else
            {
                if (attackLastTime + distanceAttackRate > Time.time)
                    return;

                attackLastTime = Time.time;

                if (Owner.Health.Current > Owner.Health.Max / 2)
                    AttackRandom(new WereWolfAttackData[] { jumpInOut });
                else
                    AttackRandom(new WereWolfAttackData[] { giantHead,jumpInOut,cries });
            }
        }
        private void AttackRandom(WereWolfAttackData[] attacks)
        {
            int randomChance = (int)(UnityEngine.Random.value * 10);
            //print("AttackRandom: "+randomChance);
            List<WereWolfAttackData> attacksSelected = new List<WereWolfAttackData>();

            for (int i = 0; i < attacks.Length; i++)
                if (FloatHelper.InBetween(targetDistance,attacks[i].distance))
                    attacksSelected.Add(attacks[i]);

            for (int i = 0; i < attacksSelected.Count; i++)
                if (!FloatHelper.InBetween(randomChance,attacksSelected[i].chance))
                {
                    attacksSelected.RemoveAt(i);
                    i--;
                }

            if (attacksSelected.Count > 0)
                DoAttack(attacksSelected[UnityEngine.Random.Range(0, attacksSelected.Count)].attack);
        }
        public void DoAttack(AttackType type) 
        {
            switch (type)
            {
                case AttackType.Fuckoff:
                    Owner.DoFuckOff();
                    break;
                case AttackType.Strike:
                    Owner.DoStrike();
                    break;
                case AttackType.DashStrike:
                    Owner.DoDashStrike();
                    break;
                case AttackType.Cries:
                    Owner.DoCries(true);
                    break;
                case AttackType.JumpInOutAttack:
                    Owner.JumpOutAttackModule.DoAttack();
                    break;
                case AttackType.GiantHeadAttack:
                    Owner.GiantHeadModule.DoAttack();
                    break;
            }
        }

        private void Phase2(int currentHealth)
        {
            if(Owner.Health.Current <= Owner.Health.Max/2)
            {
                Owner.Health.OnDamage -= Phase2;
                Owner.DoCries(true);
                OnPhaseTwo.Invoke();
            }
        }

        private void OnComboDamage()
        {
            if(lastComboDamage + comboDamageRate < Time.time)
                comboDamageCounter = 0;

            lastComboDamage = Time.time;

            comboDamageCounter++;

            if(comboDamageCounter == backwardComboDamageCount && !Owner.IsAttacking && targetDistance < backwardDistance)
                GoToBackwardWalk();         
        }

        private void JumpInOutAttackOnDamaged(int health)
        {
            if (Owner.Health.CurrentFillAmount <= JumpInOutAttackPlayOnHealthPrecent)
            {
                Owner.Health.OnDamage -= JumpInOutAttackOnDamaged;
                DoAttack(AttackType.JumpInOutAttack);
            }
        }
        private void GiantHeadOnDamaged(int health)
        {
            if (Owner.Health.CurrentFillAmount <= giantHeadAttackPlayOnHealthPrecent)
            {
                Owner.Health.OnDamage -= GiantHeadOnDamaged;
                DoAttack(AttackType.GiantHeadAttack);
            }
        }
    }
}
