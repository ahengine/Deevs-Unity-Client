using UnityEngine;

namespace Entities.WereWolf.Inputs
{
    [RequireComponent(typeof(WereWolf))]
    public class WereWolfInput : MonoBehaviour
    {
        private WereWolf owner;
        [SerializeField] private string horizontalMove = "Horizontal";
        [SerializeField] private KeyCode strike = KeyCode.Alpha3;
        [SerializeField] private KeyCode dashStrike = KeyCode.Alpha4;
        [SerializeField] private KeyCode Death = KeyCode.Alpha5;
        [SerializeField] private KeyCode HeadInAttack = KeyCode.Alpha6;
        [SerializeField] private KeyCode JumpInOutAttack = KeyCode.Alpha7;
        [SerializeField] private KeyCode Cries = KeyCode.Alpha8;
        [SerializeField] private KeyCode FuckOff = KeyCode.Alpha9;

        private void Awake() => owner = GetComponent<WereWolf>();

        private void Update()
        {
            if (Input.GetKeyDown(Death))
            {
                if (!owner.IsDead)
                    owner.DoDeath();
                else owner.DoFinisherDeath();
            }

            if (owner.IsDead)
                return;

            owner.SetHorizontalSpeed(Input.GetAxis(horizontalMove));

            if (Input.GetKeyDown(HeadInAttack))
                owner.GiantHeadModule.DoAttack();

            if (Input.GetKeyDown(JumpInOutAttack))
                owner.JumpOutAttackModule.DoAttack();

            if (Input.GetKeyDown(Cries))
                owner.DoCries();

            if (Input.GetKeyDown(FuckOff))
                owner.DoFuckOff();

            if (Input.GetKeyDown(dashStrike))
                owner.DoDashStrike();

            if (Input.GetKeyDown(strike))
                owner.DoStrike();
        }


    }
}