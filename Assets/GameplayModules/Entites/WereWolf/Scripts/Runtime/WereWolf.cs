using UnityEngine;
using CC2D;
using CC2D.Modules;
//using Entities.WereWolf.Moudles;

namespace Entities.WereWolf
{
    [RequireComponent(typeof(CharacterController2D))]
    public class WereWolf : MonoBehaviour
    {
        private const string WALK_ANIMATOR_INT = "Walk";

        private Transform tr;
        private CharacterController2D cc;
        private Animator animator;
        private SpriteRenderer spr;

        private void Awake()
        {
            tr = transform;
            animator = GetComponentInChildren<Animator>();
            spr = animator.GetComponent<SpriteRenderer>();
            cc = GetComponent<CharacterController2D>();
        }

        private void Update()
        {
            animator.SetInteger(WALK_ANIMATOR_INT, cc.Velocity.x > 0 ? 1 : cc.Velocity.x < 0 ? -1 : 0);
        }

        public void SetHorizontalSpeed(float value)
        {
            cc.SetHorizontal(value);
            spr.flipX = cc.FaceDirection == 1;
        }

    }
}