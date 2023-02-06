using UnityEngine;

namespace Cam
{
    public class OrthoSizeRelativeModule : CameraModule
    {
        private const float onTenthOrthoSize = 1.78f;
        [SerializeField] private Transform YSyncTr;
        [SerializeField] private Transform relative;
        [SerializeField] private Vector2 yRange = new Vector2(-2, 4);
        [SerializeField] private Vector2 orthoSize = new Vector2(8, 13);
        private Vector3 position;
        public float OrthSizeFillAmount => (controller.Cam.orthographicSize - orthoSize.x) / orthoSize.y;
        public float distanceCameraCamTargetOrthoSize;

        public override void Init(CameraController controller)
        {
            base.Init(controller);
            position = YSyncTr.localPosition;

            distanceCameraCamTargetOrthoSize =
                DistanceFloat(controller.TargetCam.position.x, controller.TargetEntity.position.x) / onTenthOrthoSize;
        }

        public void SetRelativeTo(Transform relativeTo)
        {
            relative = relativeTo;
        }

        protected override void UpdateData()
        {
            base.UpdateData();

            if (!controller.TargetEntity) return;

            float ortho = distanceCameraCamTargetOrthoSize + DistanceFloat(relative.position.x, 
                controller.TargetEntity.position.x) / onTenthOrthoSize;
            controller.Cam.orthographicSize = Mathf.Clamp(ortho, orthoSize.x, orthoSize.y);
            position.y = yRange.x + (yRange.y - yRange.x) * OrthSizeFillAmount;
            YSyncTr.localPosition = position;
        }

        private static float DistanceFloat(float a, float b) =>
            Mathf.Abs(Mathf.Abs(a) - Mathf.Abs(b));
    }
}