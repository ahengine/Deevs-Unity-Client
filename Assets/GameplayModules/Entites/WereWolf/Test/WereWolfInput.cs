using UnityEngine;

namespace Entities.WereWolf.Inputs
{
    [RequireComponent(typeof(WereWolf))]
    public class WereWolfInput : MonoBehaviour
    {
        private WereWolf owner;
        [SerializeField] private string horizontalMove = "Horizontal";
        [SerializeField] private KeyCode Death = KeyCode.Alpha5;
        [SerializeField] private KeyCode HeadInAttack = KeyCode.Alpha6;
        [SerializeField] private KeyCode JumpInOutAttack = KeyCode.Alpha7;

        private void Awake() => owner = GetComponent<WereWolf>();

        private void Update()
        {
            owner.SetHorizontalSpeed(Input.GetAxis(horizontalMove));

            if (Input.GetKeyDown(Death))
            {
                if (!owner.IsDead)
                    owner.DoDeath();
                else owner.DoFinisherDeath();
            }

            if (Input.GetKeyDown(HeadInAttack))
                owner.GiantHeadModule.DoAttack();

            if (Input.GetKeyDown(JumpInOutAttack))
                owner.JumpOutAttackModule.DoAttack();
        }


    }
}