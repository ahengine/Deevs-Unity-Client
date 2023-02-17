using System;
using UnityEngine;

namespace CC2D.Modules
{
    public class DodgeModule : CC2DModule
    {
        [SerializeField] private float speed = 1.5f;
        [SerializeField] private float startDelay = .1f;
        [SerializeField] private float duration = .5f;
        [SerializeField] private float endDelay = .1f;

        public override bool AllowHorizontalMove => false;

        public event Action onDodgingEnd;

        public bool IsDodging;

        protected override bool AllowActivateModule => 
            cc.AllowAction && !IsActive && cc.Velocity.y == 0;
        protected override void ApplyActivate() => IsDodging = true;

        public override void Process()
        {
            base.Process();

            // START
            if (startDelay + startTime > Time.time)
            {
                cc.VelocityX = 0;
                return;
            }

            // DURATION
            if (startDelay + duration + startTime > Time.time)
            {
                cc.VelocityX = cc.FaceDirection * speed;
                return;
            }

            // END
            if (startDelay + duration + endDelay + startTime > Time.time)
            {

                if (IsDodging)
                {
                    onDodgingEnd.Invoke();
                    IsDodging = false;
                }
                cc.VelocityX = 0;
                return;
            }

            DoDeactivate();
        }

        protected override void ApplyDeactivate() => cc.VelocityX = 0;
    }
}