using UnityEngine;

namespace Entities.Heeloy
{
    public class HeeloyAnimationEventHandler : MonoBehaviour
    {
        private Heeloy heeloy;

        private void Awake()
        {
            heeloy = transform.parent.parent.GetComponent<Heeloy>();
        }

        public void OnStand() => heeloy.OnStand();
        public void StandUp() => heeloy.StandUp();
        public void OnAttackEnd() => heeloy.AttackEnd();
    }
}