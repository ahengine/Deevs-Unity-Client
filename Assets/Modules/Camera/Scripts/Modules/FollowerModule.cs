using UnityEngine;

namespace Cam
{
    public class FollowerModule : CameraModule
    {
        [SerializeField] private float moveSpeed = 2;
        private int targetDirection = 1;
        private Vector2 lastTargetPosition;
        [SerializeField] private float distanceFollow = .25f;
        [SerializeField] bool followVertical;
        [SerializeField] private Transform secondTarget;

        protected override void UpdateData()
        {
            base.UpdateData();
            CalculateTargetDirection();

            bool AllowRightFollow = targetDirection == 1 && controller.TargetEntity.position.x > controller.Tr.position.x - distanceFollow;
            bool AllowLeftFollow = targetDirection == -1 && controller.TargetEntity.position.x < controller.Tr.position.x + distanceFollow;

            Vector3 targetPos = controller.TargetCam.position;

            if (secondTarget)
                targetPos.x += FloatHelper.Distance(controller.TargetCam.position.x,secondTarget.position.x) / 2 * (secondTarget.position.x > controller.Tr.position.x?1:-1);

            if (!followVertical) targetPos.y = controller.Tr.position.y;
            targetPos.z = controller.Tr.position.z;

            if (!AllowRightFollow && !AllowLeftFollow)
                targetPos.x = controller.Tr.position.x;

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
