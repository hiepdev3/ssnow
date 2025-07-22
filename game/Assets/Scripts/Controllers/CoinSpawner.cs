using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    public GameObject Coin;           // Prefab của coin
    public Transform groundTransform; // Để xác định gốc tọa độ
    public float offsetY = 1.0f;      // Khoảng cách ban đầu từ ground
    public float coinSpacingX = 1.0f; // Khoảng cách giữa các coin theo X
    public float groupSpacingYMin = 2f; // Khoảng cách tối thiểu giữa các nhóm
    public float groupSpacingYMax = 4f; // Khoảng cách tối đa giữa các nhóm
    public int numberOfGroups = 5;    // Số lượng nhóm coin cần spawn

    public GameObject coinPrefab;       // Prefab của coin
    public GameObject ground;           // Object ground để lấy kích thước
    public int numberOfCoins = 3;       // Số coin spawn mỗi lần


    void Start()
    {
        Vector3 currentPos = new Vector3(
             groundTransform.position.x,
             groundTransform.position.y + offsetY,
             0f
         );

        for (int i = 0; i < numberOfGroups; i++)
        {
            SpawnCoinGroup(currentPos);

            // Random khoảng cách theo trục Y
            float randomY = Random.Range(groupSpacingYMin, groupSpacingYMax);
            currentPos.y += randomY;
        }
        SpawnCoins();
    }
   
    void SpawnCoinGroup(Vector3 startPos)
    {
        float spacing = 10f; // Khoảng cách giữa các coin
        int numberOfCoins = 3;
        for (int i = 0; i < numberOfCoins; i++)
        {
            // Tính vị trí theo chiều ngang (trục X)
            //Vector3 spawnPos = groundTransform.position + new Vector3(i * spacing, 0, 0);
            Vector3 spawnPos = startPos + new Vector3(i * spacing, 0, 0);
            Instantiate(Coin, spawnPos, Quaternion.identity);
        }
      

    }
    void SpawnCoins()
    {
        if (ground == null || coinPrefab == null) return;

        Bounds groundBounds = ground.GetComponent<Renderer>().bounds;

        for (int i = 0; i < numberOfCoins; i++)
        {
            float randomX = Random.Range(groundBounds.min.x, groundBounds.max.x);
            float randomZ = Random.Range(groundBounds.min.z, groundBounds.max.z);
            float y = groundBounds.max.y + 1f; // spawn cao hơn mặt đất 1 chút

            Vector3 spawnPosition = new Vector3(randomX, y, randomZ);
            Instantiate(coinPrefab, spawnPosition, Quaternion.identity);
        }
    }
}
