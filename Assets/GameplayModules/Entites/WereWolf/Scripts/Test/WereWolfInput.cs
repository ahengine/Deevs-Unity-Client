using UnityEngine;

namespace Entities.WereWolf.Inputs
{
    [RequireComponent(typeof(WereWolf))]
    public class WereWolfInput : MonoBehaviour
    {
        private WereWolf owner;
        [SerializeField] private string horizontalMove = "Horizontal";
        [SerializeField] private KeyCode HeadInAttack = KeyCode.Alpha6;
        [SerializeField] private KeyCode Death = KeyCode.Alpha5;

        private void Awake() => owner = GetComponent<WereWolf>();

        private void Update()
        {
            owner.SetHorizontalSpeed(Input.GetAxis(horizontalMove));

            if (Input.GetKeyDown(HeadInAttack))
                owner.GiantHeadModule.DoAttack(2);

            if (Input.GetKeyDown(Death))
            {
                if (!owner.IsDead)
                    owner.DoDeath();
                else owner.DoFinisherDeath();
            }
        }


    }
}