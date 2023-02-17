using UnityEngine;

namespace Cam
{
    public class CameraController : MonoBehaviour
    {
        public Camera Cam { private set; get; }
        public Transform Tr { private set; get; }
        [field:SerializeField] public Transform TargetEntity { private set; get; }
        [field:SerializeField] public Transform TargetCam { private set; get; }

        public void SetTarget(Transform target) => TargetEntity = target;
        [SerializeField] private CameraModule[] modules;


        private void Awake()
        {
            Tr = transform;
            Cam = GetComponentInChildren<Camera>();

            for (int i = 0; i < modules.Length; i++)
                modules[i].Init(this);
        }

        private void LateUpdate()
        {
            for (int i = 0; i < modules.Length; i++)
                modules[i].Updates();
        }
    }
}
