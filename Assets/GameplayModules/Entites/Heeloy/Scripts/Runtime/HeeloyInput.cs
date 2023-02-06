using UnityEngine;

namespace Entities.Heeloy.Inputs
{
    [RequireComponent(typeof(Heeloy))]
    public class HeeloyInput : MonoBehaviour
    {
        private Heeloy heeloy;
        [SerializeField] private string horizontalMove = "Horizontal";
        [SerializeField] private KeyCode[] jump = { KeyCode.W,KeyCode.UpArrow };
        [SerializeField] private KeyCode[] sit = { KeyCode.LeftControl , KeyCode.RightControl };
        [SerializeField] private KeyCode[] dodge = { KeyCode.LeftShift , KeyCode.RightShift };
        [SerializeField] private KeyCode swordAttack = KeyCode.Mouse0;
        [SerializeField] private KeyCode swordHeavyAttack = KeyCode.Mouse1;
        [SerializeField] private KeyCode swordAbilityAttack = KeyCode.Q;

        private void Awake() => heeloy = GetComponent<Heeloy>();

        private void Update()
        {
            heeloy.SetHorizontalSpeed(Input.GetAxis(horizontalMove));

            for (int i = 0; i < jump.Length; i++)
                if (Input.GetKeyDown(jump[i]))
                {
                    if(heeloy.IsSit)
                        heeloy.DoStand();
                    else
                        heeloy.DoJump();
                }


            for (int i = 0; i < sit.Length; i++)
                if (Input.GetKeyDown(sit[i]))
                    heeloy.DoSit();

            for (int i = 0; i < dodge.Length; i++)
                if (Input.GetKeyDown(dodge[i]))
                    heeloy.DoDodge();

            if (Input.GetKeyDown(swordAttack))
                heeloy.DoAttack();

            if (Input.GetKeyDown(swordHeavyAttack))
                heeloy.DoSwordHeavyAttack();

            if (Input.GetKeyDown(swordAbilityAttack))
                heeloy.DoSwordAbilityAttack();
        }
    }
}