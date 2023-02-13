using UnityEngine;

namespace Entities.WereWolf.HeadGiant
{
    public class WereWolfGiantHeadAnimationEvent : MonoBehaviour
    {
        private WereWolfGiantHead controller;

        private void Awake()
        {
            controller = transform.parent.GetComponent<WereWolfGiantHead>();
        }

        public void Attack() => controller.Attack();

        public void PickUpTarget() => controller.PickUpTarget();

        public void LeftTarget() => controller.LeftTarget();

        public void EndAttack() => controller.EndAttack();
    }
}