using UnityEngine;

namespace Cam
{
    // Add to Camera Controller for last module
    public class LimitMoveModule : CameraModule
    {
        [SerializeField] private bool horizontal;
        [SerializeField] private Vector2 horizontalRange;

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
            float x = controller.Tr.position.x;
            x = Mathf.Clamp(x,
                horizontalRange.x, horizontalRange.y);
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
            cam.y += .1f;
            Gizmos.color = Color.green;
            if (horizontal)
                Gizmos.DrawLine(new Vector3(horizontalRange.x, cam.y), new Vector3(horizontalRange.y, cam.y));
        }
    }
}
