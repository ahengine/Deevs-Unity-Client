using UnityEngine;

namespace Cam
{
    public class ShakeModule : CameraModule
    {
        [SerializeField] private float duration = .3f;
        [SerializeField] private float intensity = .2f;

        private float startTime;
        private bool active;
        private Vector3 origin;
        public override bool Active => active;

        public override void Apply()
        {
            base.Apply();
            active = true;
            startTime = Time.time;
            origin = controller.Cam.transform.localPosition;
        }

        protected override void UpdateData()
        {
            base.UpdateData();

            //if(Input.GetKeyDown(KeyCode.Alpha1))
            //    Apply();

            if (!active) return;

            controller.Cam.transform.localPosition = origin + Random.onUnitSphere * intensity * Time.deltaTime;

            if (startTime + duration < Time.time)
            {
                controller.Cam.transform.localPosition = origin;
                active = false;
            }
        }
    }
}
