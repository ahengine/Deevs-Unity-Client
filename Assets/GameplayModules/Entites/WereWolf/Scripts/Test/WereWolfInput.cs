using UnityEngine;

namespace Entities.WereWolf.Inputs
{
    [RequireComponent(typeof(WereWolf))]
    public class WereWolfInput : MonoBehaviour
    {
        private WereWolf owner;
        [SerializeField] private string horizontalMove = "Horizontal";

        private void Awake() => owner = GetComponent<WereWolf>();

        private void Update()
        {
            owner.SetHorizontalSpeed(Input.GetAxis(horizontalMove));
        }
    }
}