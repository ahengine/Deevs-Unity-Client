using UnityEngine;

namespace CC2D.Modules
{
    public class DodgeModule : CC2DModule
    {
        [SerializeField] private float speed = 1.5f;
        [SerializeField] private float duration = .5f;

        public override bool AllowHorizontalMove => false;

        protected override bool AllowActivateModule => 
            cc.AllowAction && !IsActive && cc.Velocity.y == 0;
        protected override void ApplyActivate() { }

        public override void Process()
        {
            base.Process();
            
            cc.VelocityX = cc.FaceDirection * speed;

            if (duration + startTime > Time.time) return;

            DoDeactivate();
        }

        protected override void ApplyDeactivate() => cc.VelocityX = 0;
    }
}