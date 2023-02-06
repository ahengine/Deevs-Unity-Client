using UnityEngine;

namespace Cam
{
    // Add to Camera Controller for last module
    public class LimitMoveModule : CameraModule
    {
        [SerializeField] private OrthoSizeRelativeModule orthoModule;
        [SerializeField] private bool horizontal;
        [SerializeField] private Vector2 horizontalMinRange;
        [SerializeField] private Vector2 horizontalMaxRange;
        [SerializeField] private Vector2 horizontalRange =>
            !orthoModule ? new Vector2(horizontalMinRange.x, horizontalMinRange.y) :
            new Vector2(
            Mathf.Lerp(horizontalMinRange.x, horizontalMaxRange.x, orthoModule.OrthSizeFillAmount),
            Mathf.Lerp(horizontalMinRange.y, horizontalMaxRange.y, orthoModule.OrthSizeFillAmount));

        [SerializeField] private bool vertical;
        [SerializeField] private Vector2 verticalRange;

        protected override void UpdateData()
        {
            base.UpdateData();

            if (horizontal)
                Horizontal();

            if (vertical)
                Vertical();
        }

        private void Horizontal()
        {
            float x = Mathf.Clamp(controller.Tr.position.x, horizontalRange.x, horizontalRange.y);
            controller.Tr.position = new Vector3(x, controller.Tr.position.y, controller.Tr.position.z);
        }

        private void Vertical()
        {
            float y = controller.Tr.position.y;
            y = Mathf.Clamp(y,
                verticalRange.x, verticalRange.y);
            controller.Tr.position = new Vector3(controller.Tr.position.x, y, controller.Tr.position.z);
        }

        private void OnDrawGizmos()
        {
            Vector3 cam = transform.position;
            cam.z = 0;

            if (horizontal)
            {
                cam.y += .1f;
                Gizmos.color = Color.green;
                Gizmos.DrawLine(new Vector3(horizontalMinRange.x, cam.y), new Vector3(horizontalMinRange.y, cam.y));
                cam.y -= .2f;
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(new Vector3(horizontalMaxRange.x, cam.y), new Vector3(horizontalMaxRange.y, cam.y));
            }
        }
    }
}
