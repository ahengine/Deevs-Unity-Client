using UnityEngine;

namespace Cam
{
    public class FollowerModule : CameraModule
    {
        [SerializeField] private float moveSpeed = 2;
        private int targetDirection = 1;
        private Vector2 lastTargetPosition;
        [SerializeField] private float distanceFollow = .25f;

        protected override void UpdateData()
        {
            base.UpdateData();
            CalculateTargetDirection();

            bool AllowRightFollow = targetDirection == 1 && controller.TargetEntity.position.x > controller.Tr.position.x - distanceFollow;
            bool AllowLeftFollow = targetDirection == -1 && controller.TargetEntity.position.x < controller.Tr.position.x + distanceFollow;

            if (AllowRightFollow || AllowLeftFollow)
                Follow();
        }

        private void Follow()
        {
            Vector3 targetPos = controller.TargetCam.position;
            targetPos.z = controller.Tr.position.z;
            controller.Tr.position = Vector3.Lerp(controller.Tr.position, targetPos, moveSpeed * Time.deltaTime);
        }

        private void CalculateTargetDirection()
        {
            if (lastTargetPosition.x == controller.TargetEntity.position.x) return;

            targetDirection = lastTargetPosition.x < controller.TargetEntity.position.x ? 1 : -1;
            lastTargetPosition = controller.TargetEntity.position;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Vector3 distance = new Vector3(this.distanceFollow, 0, 0);
            Vector3 cam = transform.position;
            cam.z = 0;
            Gizmos.DrawLine(cam + distance, cam - distance);
        }
    }
}
