using UnityEngine;

namespace Cam
{
    public class OrthoSizeRelativeModule : CameraModule
    {
        [SerializeField] private Transform relative;
        [SerializeField] private float maxDistanceSize = 1.5f;
        [SerializeField] private Vector2 yRange = new Vector2(-2, 4);
        [SerializeField] private Vector2 orthoSize = new Vector2(8, 13);
        private Vector3 position;


        public override void Init(CameraController controller)
        {
            base.Init(controller);
            position = controller.Tr.localPosition;
        }

        public void SetRelativeTo(Transform relativeTo)
        {
            relative = relativeTo;
        }

        protected override void UpdateData()
        {
            base.UpdateData();

            if(!controller.TargetEntity) return;

            float distance = Mathf.Clamp(Vector2.Distance(controller.TargetEntity.position, relative.position) / maxDistanceSize, 0, 1);
            position.y = yRange.x + (yRange.y - yRange.x) * distance;
            controller.Cam.transform.localPosition = position;
            controller.Cam.orthographicSize = orthoSize.x + (orthoSize.y - orthoSize.x) * distance;
        }
    }
}