using UnityEngine;

public class RockSpawner : MonoBehaviour
{
    public GameObject rockPrefab;            // Prefab đá
    public Transform playerTransform;        // Transform của người chơi
    public float minX = -10f;                // Ranh giới trái của ground
    public float maxX = 10f;                 // Ranh giới phải của ground
    public float groundY = 0f;               // Độ cao mặt đất
    public float offsetY = 0.5f;             // Cho đá nổi lên khỏi mặt đất
    public float minDistanceFromPlayer = 3f; // Khoảng cách tối thiểu giữa đá và player

    void Start()
    {
        SpawnRockOnce();
    }


    void SpawnRockOnce()
    {
        float randomX;
        int attempts = 0;
        int maxAttempts = 50;

        do
        {
            randomX = Random.Range(minX, maxX);
            attempts++;
        }
        while (Mathf.Abs(randomX - playerTransform.position.x) < minDistanceFromPlayer && attempts < maxAttempts);

        Vector2 spawnPos = new Vector2(randomX, groundY + offsetY);
        Instantiate(rockPrefab, spawnPos, Quaternion.identity);
    }
}
