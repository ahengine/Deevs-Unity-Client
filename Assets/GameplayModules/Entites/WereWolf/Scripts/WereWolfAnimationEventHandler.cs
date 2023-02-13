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
            owner.ApplyHeadOutAttack();
    }
}
