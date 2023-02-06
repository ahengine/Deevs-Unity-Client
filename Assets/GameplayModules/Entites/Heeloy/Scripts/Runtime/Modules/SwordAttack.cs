using UnityEngine;

namespace Entities.Heeloy.Moudles
{
    [System.Serializable]
    public class SwordAttack
    {
        private const string SWORD_ABILITY_ATTACK_ANIMATOR_TRIGGER = "SwordAbilityAttack";
        private const string SWORD_ATTACK_ANIMATOR_TRIGGER = "SwordAttack";
        private const string SWORD_ATTACK_INDEX_ANIMATOR_INT = "SwordAttackIndex";

        public int Index { private set; get; }
        [SerializeField] private int actionCount = 5;
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

        public void SwordAbilityAttack(Animator animator)
        {
            animator.SetTrigger(SWORD_ABILITY_ATTACK_ANIMATOR_TRIGGER);
        }
    }
}