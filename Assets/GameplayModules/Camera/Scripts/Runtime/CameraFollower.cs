using UnityEngine;

namespace Cam
{
    public class CameraFollower : MonoBehaviour
    {
        [field: SerializeField] public Transform Target;
        [SerializeField] private float moveSpeed = 2;
        private Transform tr;
        private Vector3 target;
        private void Awake()
        {
            tr = transform;
        }

        private void LateUpdate()
        {
            if (!Target) return;
            target = Target.position;
            target.z = tr.position.z;
            tr.position = Vector3.Lerp(tr.position, target, moveSpeed * Time.deltaTime);
        }

        public void DoShake()
        {

        }

        private void ApplyShake()
        {

        }
    }
}
