using UnityEngine;

namespace Cam
{
    public class CameraModule : MonoBehaviour
    {
        protected CameraController controller;
        [SerializeField] protected CameraModule[] dependencies;
        public virtual bool Active => true;

        public virtual void Init(CameraController controller)
        {
            this.controller = controller;
        }

        public void Updates()
        {
            for (int i = 0; i < dependencies.Length; i++)
                if (dependencies[i].Active)
                    return;

            UpdateData();
        }

        protected virtual void UpdateData()
        {

        }

        public virtual void Apply()
        {

        }
    }
}