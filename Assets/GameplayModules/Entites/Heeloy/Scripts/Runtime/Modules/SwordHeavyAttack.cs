using UnityEngine;

namespace Entities.Heeloy.Moudles
{
    [System.Serializable]
    public class SwordHeavyAttack
    {
        private const string SWORD_ATTACK_ANIMATOR_TRIGGER = "SwordHeavyAttack";
        private const string SWORD_ATTACK_INDEX_ANIMATOR_INT = "SwordHeavyAttackIndex";

        public int Index { private set; get; }
        [SerializeField] private int actionCount = 2;
        [SerializeField] private Vector2Int starterRangeIndex = new Vector2Int(0, 3);
        [SerializeField] private float delayBetweenEachAction = 1;
        private float startTime;

        public void UpdateIndex()
        { 
                Index = startTime + delayBetweenEachAction > Time.time ? 
                    (Index < actionCount ? Index + 1 : 0) : // Continue Combo
                    Random.Range(starterRangeIndex.x, starterRangeIndex.y); // Reset

            startTime = Time.time;
        }

        public void Attack(Animator animator)
        {
            animator.SetTrigger(SWORD_ATTACK_ANIMATOR_TRIGGER);
            animator.SetInteger(SWORD_ATTACK_INDEX_ANIMATOR_INT, Index);
        }
    }
}