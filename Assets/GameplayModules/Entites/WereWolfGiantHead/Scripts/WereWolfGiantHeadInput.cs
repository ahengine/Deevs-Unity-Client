using UnityEngine;

namespace Entities.WereWolf.HeadGiant.Inputs
{
    [RequireComponent(typeof(WereWolfGiantHead))]
    public class WereWolfGiantHeadInput : MonoBehaviour
    {
        private WereWolfGiantHead controller;

        private void Awake() => 
            controller = GetComponent<WereWolfGiantHead>();


        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Alpha9))
                controller.DoAttack();

            if (Input.GetKeyDown(KeyCode.Alpha8))
                controller.DoAttack();

            if (Input.GetKeyDown(KeyCode.Alpha7))
                controller.DoWaitingAttack();
        }
    }
}