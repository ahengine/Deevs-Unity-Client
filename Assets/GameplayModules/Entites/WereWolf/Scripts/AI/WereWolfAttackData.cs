using UnityEngine;

namespace Entities.WereWolf.AI
{
    public enum AttackType { Fuckoff, Strike, DashStrike, Cries, JumpInOutAttack, GiantHeadAttack }
    public enum LocomotionStates { Idle, Walk, BackwardWalk }

    [System.Serializable]
    public class WereWolfAttackData
    {
        public Vector2 distance = new Vector2(.3f, 1);
        [Tooltip("Min 0 Max 10")]public Vector2Int chance = new Vector2Int(2, 5);
        public AttackType attack;
    }
}
