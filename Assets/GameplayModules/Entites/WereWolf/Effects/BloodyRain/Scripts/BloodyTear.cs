using Entities;
using System.Threading.Tasks;
using UnityEngine;

namespace BlodyTears
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class BloodyTear : MonoBehaviour
    {
        private const string EXPLODE_BOOL_ANIMATOR = "Explode";

        private Collider2D col;
        private Rigidbody2D rb;
        private Animator animator;
        [SerializeField] private float moveSpeed = 5;
        [SerializeField] private Vector2Int damageRange = new Vector2Int(10,20);


        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            col = GetComponent<Collider2D>();
            animator = GetComponentInChildren<Animator>();
        }

        private void OnEnable()
        {
            col.enabled = rb.simulated = true;
            rb.velocity = new Vector2(0, -moveSpeed);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            animator.SetBool(EXPLODE_BOOL_ANIMATOR, true);
            col.enabled = rb.simulated = false;
            var damagable =  collision.collider.GetComponent<IDamagable>();
            if (damagable != null) damagable.DoDamage(Random.Range(damageRange.x, damageRange.y));
            _ = DeactiveAsync();
        }

        private async Task DeactiveAsync()
        {
            await Task.Delay(1000);
            gameObject.SetActive(false);
        }
    }
}