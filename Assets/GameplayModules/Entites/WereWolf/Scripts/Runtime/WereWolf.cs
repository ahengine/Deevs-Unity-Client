using UnityEngine;
using CC2D;
using CC2D.Modules;
//using Entities.WereWolf.Moudles;

namespace Entities.WereWolf
{
    public class WereWolf : Entity2D
    {
        public override void SetHorizontalSpeed(float value)
        {
            base.SetHorizontalSpeed(value);
            spr.flipX = cc.FaceDirection == 1;
        }

        public override void DoDamage(int damage)
        {

        }
    }
}