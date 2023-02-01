using Patterns;
using UnityEngine;

public class BoxSpawner : MonoBehaviour
{
    [SerializeField] private Pool<Component> itemPrefab;
    [SerializeField] private float rate = .05f;
    [SerializeField] private Vector2 size = new Vector2(1,1);
    private Vector2 HalfSize => size / 2;
    private float lastSpawnTime;
    private Transform tr;
    [field:SerializeField] public bool AllowSpawn { private set; get; }

    public void SetAllowSpawn(bool allowSpawn) => AllowSpawn = allowSpawn;

    private void Awake()
    {
        tr = transform;
        lastSpawnTime = Time.time;
    }


    private void Update()
    {
        if (lastSpawnTime + rate > Time.time) return;

        lastSpawnTime = Time.time;

        var item = itemPrefab.Get;
        item.transform.position = tr.position +
            new Vector3(Random.Range(-HalfSize.x, HalfSize.x),
                        Random.Range(-HalfSize.y, HalfSize.y));
        item.gameObject.SetActive(true);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector2 basePos = transform.position;
        // Horizontal Line
        Gizmos.DrawLine(basePos + new Vector2(-HalfSize.x, -HalfSize.y), basePos + new Vector2(HalfSize.x, -HalfSize.y));
        Gizmos.DrawLine(basePos + new Vector2(-HalfSize.x, HalfSize.y), basePos + new Vector2(HalfSize.x, HalfSize.y));
        // Vertical Line
        Gizmos.DrawLine(basePos + new Vector2(-HalfSize.x, -HalfSize.y), basePos + new Vector2(-HalfSize.x, HalfSize.y));
        Gizmos.DrawLine(basePos + new Vector2(HalfSize.x, -HalfSize.y), basePos + new Vector2(HalfSize.x, HalfSize.y));
    }
}