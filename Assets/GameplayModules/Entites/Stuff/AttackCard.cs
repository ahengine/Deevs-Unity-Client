using UnityEngine;

namespace Entities
{
    [CreateAssetMenu(fileName = "Attack Card", menuName = "Cards/Attack Card", order = 1)]
    public class AttackCard : ScriptableObject
    {
        public float distance = .2f;
        public Vector2Int damageRange = new Vector2Int(3, 4);
        public int Damage => Random.Range(damageRange.x, damageRange.y);
    }
}