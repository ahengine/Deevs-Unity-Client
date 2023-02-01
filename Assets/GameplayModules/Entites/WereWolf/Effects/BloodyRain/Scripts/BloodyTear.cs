using System.Threading.Tasks;
using UnityEngine;

namespace BlodyTears
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class BloodyTear : MonoBehaviour
    {
        private const string EXPLODE_BOOL_ANIMATOR = "Explode";

        private Rigidbody2D rb;
        private Animator animator;
        [SerializeField] private float moveSpeed = 5;


        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponentInChildren<Animator>();
        }

        private void OnEnable()
        {
            rb.velocity = new Vector2(0, -moveSpeed);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            animator.SetBool(EXPLODE_BOOL_ANIMATOR, true);
            _ = DeactiveAsync();
        }

        private async Task DeactiveAsync()
        {
            await Task.Delay(1000);
            gameObject.SetActive(false);
        }
    }
}