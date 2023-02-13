using Patterns;
using UnityEngine;
using UnityEngine.AI;

namespace Entities.WereWolf
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(AudioSource))]
    public class WereWolfAI : Entity2D, IDamagable
    {
        public bool hard;
        public int hardMinHealthPercent = 25;

        public enum States { Idle, Patrol, Death, Hunt }
        private const string RunAnimationBool = "Run", CrawlAnimationBool = "Crawl", DeathAnimationTrigger = "Death", DeathIndexAnimationInt = "DeathIndex",
            AttackAnimationTrigger = "Attack", AttackIndexAnimationInt = "AttackIndex",
            HitAnimationTrigger = "Hit";

        private NavMeshAgent agent;
        private Transform tr;
        private Animator animator;
        private Collider col;
        [SerializeField] private States currentState;
        private System.Action currentStateUpdate = null;
        [SerializeField] private float restPointTime = 3f;
        private float restPointStartTime;
        [SerializeField] private Transform[] wayPoints;
        private bool wayPointChangeIndexByOrder = true;
        private int wayPointIndex;
        public Health health;
        [SerializeField] private float attackRate = 4;
        private float lastAttackedTime;
        [SerializeField] private int attackDamage = 15;
        [SerializeField] private Transform attackPoint;
        private const float attackSphereTriggerSize = .2f;
        private bool isEnemySeen;
        private bool isEnemyHunting;
        [SerializeField] private float seenDistance = 10;
        [SerializeField] private float huntDistanceWithoutSeen = 1.5f;
        [SerializeField] private float sureSeeEnemyTime = 3;
        private float lastTimeEnemySeen;
        private Transform target;
        private Vector3 lastPointEnemySeen;
        private float deathAnimationTime = 6;
        [SerializeField] private LayerMask enemyLY;
        private AudioSource audioSource;
        [SerializeField] private AudioClip[] idleClips;
        [SerializeField] private AudioClip[] attackClips;
        [SerializeField] private AudioClip[] dieClips;
        [SerializeField] private int score = 10;
        [SerializeField] private Pool<ParticleSystem> damageEffectPool;
        private void Awake()
        {
            tr = transform;
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
            col = GetComponent<Collider>();
            GoToIdle();
            health.Init();
            health.OnDeath +=GoToDeath;
        }

        private void OnEnable()
        {
            col.enabled = true;
            AgentMoveState(false);
        }

        public void SetPatrolPath(Transform[] wayPoints) => this.wayPoints = wayPoints;

        private void AgentMoveState(bool allowMove)
        {   
            animator.SetBool(RunAnimationBool, allowMove);
            agent.isStopped = !allowMove;
        }

        private void Update()
        {
            if (currentState == States.Death) return;

            if (currentStateUpdate != null)
                currentStateUpdate();

            agent.destination = target ? lastPointEnemySeen : wayPoints[wayPointIndex].position;

            Watching();
        }

        private void Watching()
        {
            RaycastHit hit;
            if (isEnemySeen = Physics.CapsuleCast(tr.position + new Vector3(0, .5f), tr.position - new Vector3(0, .5f), .2f, tr.forward, out hit, seenDistance, enemyLY))
            {
                isEnemyHunting = true;
                target = hit.transform;
                lastPointEnemySeen = hit.point;
                lastTimeEnemySeen = Time.time;
            }

            if (!isEnemySeen && isEnemyHunting)
            {
                if (target)
                {
                    lastPointEnemySeen = target.position;

                    if (Vector3.Distance(tr.position, target.position) < huntDistanceWithoutSeen)
                        lastTimeEnemySeen = Time.time;
                }

                if (!target || lastTimeEnemySeen + sureSeeEnemyTime < Time.time)
                {
                    isEnemyHunting = false;
                    target = null;
                }
            }

            if (isEnemySeen && currentState != States.Hunt)
                GoToHunt();
        }

        private void MoveToNextWayPoint()
        {
            if (wayPointChangeIndexByOrder) wayPointIndex = wayPointIndex < wayPoints.Length - 1 ? wayPointIndex + 1 : 0;
            else
            {
                int previousPoint = wayPointIndex;
                while (previousPoint == wayPointIndex)
                    wayPointIndex = Random.Range(0, wayPoints.Length);
            }

            AgentMoveState(true);
            GoToPatrol();
        }

        private void GoToIdle()
        {
            AgentMoveState(false);
            currentState = States.Idle;
            restPointStartTime = Time.time;
            currentStateUpdate = Idle;
            PlayAudio(idleClips[Random.Range(0, idleClips.Length)], false);
        }

        private void Idle()
        {
            if (isEnemySeen) return;
            if (restPointStartTime + restPointTime < Time.time)
                MoveToNextWayPoint();
        }

        private void GoToPatrol()
        {
            AgentMoveState(true);
            currentState = States.Patrol;
            currentStateUpdate = Patrol;
        }

        private void Patrol()
        {
            var targetPoint = wayPoints[wayPointIndex].position;
            targetPoint.y = tr.position.y;

            if (Vector3.Distance(tr.position, targetPoint) < agent.stoppingDistance)
                GoToIdle();
        }

        private void GoToHunt()
        {
            AgentMoveState(true);
            currentState = States.Hunt;
            currentStateUpdate = Hunting;
        }

        private void Hunting()
        {
            if (!isEnemyHunting)
            {
                GoToIdle();
                return;
            }

            if (Vector3.Distance(tr.position, lastPointEnemySeen) < agent.stoppingDistance)
            {
                if (isEnemySeen) DoAttack();
                AgentMoveState(false);
                var target = Quaternion.LookRotation(new Vector3(lastPointEnemySeen.x, tr.position.y, lastPointEnemySeen.z) - tr.position);
                tr.rotation = Quaternion.Lerp(tr.rotation, target,agent.angularSpeed/100 * Time.deltaTime);
            }
            else
                AgentMoveState(true);
        }

        private void GoToDeath()
        {
            AgentMoveState(false);
            animator.SetTrigger(DeathAnimationTrigger);
            animator.SetInteger(DeathIndexAnimationInt,Random.value > .5f ? 1 : 0);
            currentState = States.Death;
            col.enabled = false;
            StartCoroutine(Death());

            PlayAudio(dieClips[Random.Range(0, dieClips.Length)], false);
        }

        private System.Collections.IEnumerator Death() 
        {
            yield return new WaitForSeconds(deathAnimationTime);
            gameObject.SetActive(false);
        }

        public void ApplyDamage(int damage, Vector3 damagePoint)
        {
            if (currentState == States.Death) return;

            health.ApplyDamage(damage);
            var effect = damageEffectPool.Get;
            effect.transform.position = damagePoint;
            effect.transform.rotation = tr.rotation;
            effect.gameObject.SetActive(true);
            effect.Play();
            animator.SetTrigger(HitAnimationTrigger);

            if(hard && (100 * health.Current/health.Max) <= hardMinHealthPercent)
                animator.SetBool(CrawlAnimationBool, true);
        }

        private void DoAttack()
        {
            if (lastAttackedTime + attackRate > Time.time)
                return;

            PlayAudio(attackClips[Random.Range(0, attackClips.Length)], false);

            lastAttackedTime = Time.time;
            animator.SetInteger(AttackIndexAnimationInt, Random.value > 0.5f ? 0 : 1);
            animator.SetTrigger(AttackAnimationTrigger);
        }

        public void Attack()
        {
            RaycastHit[] hits = Physics.SphereCastAll(attackPoint.position,attackSphereTriggerSize, attackPoint.forward, 1, enemyLY);
            foreach(var hit in hits)
            {
                hit.collider.GetComponent<IDamagable>().DoDamage(attackDamage);
                Debug.DrawLine(attackPoint.position, attackPoint.position + attackPoint.forward, Color.green, 1);
            }    
        }

        private void PlayAudio(AudioClip clip, bool loop)
        {
            audioSource.Stop();
            audioSource.loop = loop;
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
}

