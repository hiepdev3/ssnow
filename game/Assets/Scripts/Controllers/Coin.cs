using UnityEngine;

public class Coin : MonoBehaviour
{
    public int coinValue = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Gọi đến Player để cộng điểm
            PlayerCoin player = collision.GetComponent<PlayerCoin>();
            if (player != null)
            {
                player.AddCoin(coinValue);
            }

            // Hủy coin
            Destroy(gameObject);
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
