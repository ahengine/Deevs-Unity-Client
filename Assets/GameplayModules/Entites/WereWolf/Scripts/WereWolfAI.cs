using UnityEngine;

namespace Entities.WereWolf
{
    [RequireComponent(typeof(WereWolf))]
    public class WereWolfAI : MonoBehaviour
    {
        private WereWolf owner;
        [SerializeField] private Transform target;
        [SerializeField] private float distanceAttack;


        private void Awake()
        {
            owner = GetComponent<WereWolf>();
        }

        private void Update()
        {
           if(!owner.IsAttacking) Follow();
        }
        
        private void Follow()
        {
            if (Vector2.Distance(transform.position, new Vector2(target.position.x, transform.position.y)) > distanceAttack)
                owner.GiantHeadModule.DoAttack();
           
        }
    }
}
