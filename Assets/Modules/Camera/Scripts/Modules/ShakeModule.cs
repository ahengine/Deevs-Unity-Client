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
            origin = controller.Cam.transform.position;
            startTime = Time.time;
        }

        protected override void UpdateData()
        {
            base.UpdateData();

            if(Input.GetKeyDown(KeyCode.Alpha0))
                Apply();

            if (!active) return;

            controller.Cam.transform.position = origin + Random.onUnitSphere * intensity;
            
            if(startTime + duration < Time.time)
                active = false;
        }
    }
}
