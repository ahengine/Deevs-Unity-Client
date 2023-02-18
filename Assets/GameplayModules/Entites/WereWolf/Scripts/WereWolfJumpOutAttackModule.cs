using UnityEngine;
using IEnumerator = System.Collections.IEnumerator;

namespace Entities.WereWolf.Moudles
{
    [System.Serializable]
    public class WereWolfJumpOutAttackModule
    {
        private const string SEEK_TRIGGER_ANIMATION = "Seek";
        private const string JUMPIN_STATE_ANIMATION = "Base Layer.Attack.JumpInOutAttack.JumpIn";
        private const string JUMPOUT_TRIGGER_ANIMATION = "JumpOutAttack";
        private const string JUMPOOUT_ATTACK_SUCCESS_TRIGGER_ANIMATION = "JumpOutAttackSuccess";

        private WereWolf owner;
        private Animator animator;
        [SerializeField] private float damagDistance = .3f;
        [SerializeField] private Vector2Int damageRange = new Vector2Int(10,20);
        [SerializeField] private Animator seekingAnimator;
        [SerializeField] private Transform dropTargetPoint;
        [SerializeField] private float seekDelay = 1.2f;
        [SerializeField] private float jumpOutDelay = 1.2f;
        private int defaultOrder;

        public void Init(WereWolf controller, Animator animator)
        {
            this.owner = controller;
            this.animator = animator;
        }

        public void DoAttack() => JumpIn();

        private void JumpIn()
        {
            owner.SetDamagable(false);
            owner.SetState(false);
            owner.ApplyAttack();
            animator.Play(JUMPIN_STATE_ANIMATION);
            owner.StartCoroutine(JumpOutDelay());
            defaultOrder = owner.OrderLayer;
            owner.SetOrderLayer(100);
        }

        private IEnumerator JumpOutDelay()
        {
            yield return new WaitForSeconds(seekDelay);
            seekingAnimator.SetTrigger(SEEK_TRIGGER_ANIMATION);

            float timer = 0;

            while(timer < jumpOutDelay)
            {
                owner.transform.position = new Vector3(owner.Target.position.x, owner.transform.position.y);
                timer += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            JumpOut();
        }
        public void JumpOut()
        {
            owner.transform.position = new Vector3(owner.Target.position.x, owner.transform.position.y);
            owner.CreateGroundDamaged();
            animator.SetTrigger(JUMPOUT_TRIGGER_ANIMATION);
        }

        public void Attack()
        {
            bool successAttack =
                FloatHelper.Distance(owner.Target.position.x, owner.transform.position.x) < damagDistance;

            if(successAttack)
                animator.SetTrigger(JUMPOOUT_ATTACK_SUCCESS_TRIGGER_ANIMATION);

            if (successAttack)
            {
                owner.Target.gameObject.SetActive(false);
                owner.Target.GetComponent<IDamagable>().
                        DoDamageOnSky(Random.Range(damageRange.x, damageRange.y));
            }
        }
        public void AttackEnd()
        {
            owner.AttackEnd();
            owner.SetState(true);
            owner.SetOrderLayer(defaultOrder);
            if (!owner.Target.gameObject.activeSelf)
            {
                owner.Target.position = dropTargetPoint.position;
                owner.Target.gameObject.SetActive(true);
            }
        }
    }
}