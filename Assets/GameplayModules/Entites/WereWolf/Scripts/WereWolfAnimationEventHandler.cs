using UnityEngine;

namespace Entities.WereWolf
{
    public class WereWolfAnimationEventHandler : MonoBehaviour
    {
        private WereWolf owner;

        private void Awake()
        {
            owner = transform.parent.parent.GetComponent<WereWolf>();
        }

        public void AttackEnd() =>
            owner.AttackEnd();

        public void ApplyHeadOutAttack() =>
            owner.GiantHeadModule.Attack();

        public void ApplyJumpOutAttack() =>
            owner.JumpOutAttackModule.Attack();

        public void ApplyJumpOutAttackEnd() =>
                owner.JumpOutAttackModule.AttackEnd();

        public void DeathComplete() =>
            owner.ApplyFinisherDeathEnd();

        public void FuckOffDamage() =>
            owner.ApplyFuckOffDamage();

        public void DashStrikeDamage() =>
            owner.ApplyDashStrikeDamage();

        public void Strike01Damage() =>
            owner.ApplyStrike01Damage();
        
        public void Strike02Part1Damage() =>
            owner.ApplyStrike02Part1Damage();

        public void Strike02Part2Damage() =>
            owner.ApplyStrike02Part2Damage();

        public void DashStrikeJumpStart() =>
            owner.DashStriking(true);
        public void DashStrikeJumpLand() =>
            owner.DashStriking(false);

        public void DamageEnd() =>
                owner.DamageState(false);

        public void PushBack() =>
            owner.PushBack();

        public void PushBackStop() =>
            owner.PushBackStop();

        public void DoBloodyRain() =>
            owner.DoBloodyRain();
    }
}
