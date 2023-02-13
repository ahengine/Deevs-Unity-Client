using IEnumerator = System.Collections.IEnumerator;
using UnityEngine;


namespace Entities.WereWolf.Moudles
{
    public class WereWolfJumpOutAttackModule
    {
        private WereWolf controller;
        private Animator animator;

        public void Init(WereWolf controller, Animator animator)
        {
            this.controller = controller;
            this.animator = animator;
        }
    }
}