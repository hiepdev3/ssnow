using UnityEngine;
using UnityEngine.U2D; // <- Cần để truy cập SpriteShape
using System.Collections;
using UnityEngine;
public class RespawnRock : MonoBehaviour
{
    public GameObject rockPrefab;       // Snow-Rocker2 Prefab
    public GameObject groundObject;     // Ground (SpriteShape)
    public int rockCount = 10;          // Số lượng cục đá muốn spawn

    private EdgeCollider2D edgeCollider;

    void Start()
    {
        edgeCollider = groundObject.GetComponent<EdgeCollider2D>();

        if (edgeCollider == null)
        {
            Debug.LogError("Ground object must have an EdgeCollider2D.");
            return;
        }

        SpawnRocksOnGround();
    }

    void SpawnRocksOnGround()
    {
        for (int i = 0; i < rockCount; i++)
        {
            // Chọn một đoạn ngẫu nhiên giữa 2 điểm trên collider
            int index = Random.Range(0, edgeCollider.pointCount - 1);
            Vector2 pointA = edgeCollider.points[index];
            Vector2 pointB = edgeCollider.points[index + 1];

            // Lấy điểm nằm giữa đoạn (hoặc gần điểm A để tạo hiệu ứng rải đều)
            float t = Random.Range(0f, 1f);
            Vector2 spawnPointLocal = Vector2.Lerp(pointA, pointB, t);

            // Chuyển từ local sang world
            Vector2 spawnPointWorld = (Vector2)groundObject.transform.position + spawnPointLocal;

            // Offset lên một chút để không bị lọt xuống đất
            Vector3 finalSpawnPos = new Vector3(spawnPointWorld.x, spawnPointWorld.y + 0.5f, 0f);
            Instantiate(rockPrefab, finalSpawnPos, Quaternion.identity);
        }
    }
}
