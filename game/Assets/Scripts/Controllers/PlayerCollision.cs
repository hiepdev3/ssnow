using UnityEngine;
using UnityEngine.SceneManagement; // để load lại hoặc chuyển màn

public class PlayerCollision : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("Chạm phải vật cản! Kết thúc game.");
            // Load lại màn chơi hoặc chuyển sang màn endgame
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            HUDTextController hud = FindObjectOfType<HUDTextController>();
            if (hud != null)
            {
                GameData.Distance = hud.GetDistance();
                GameData.Coins = hud.GetCoins();
            }
            SceneManager.LoadScene("GameOverScene");
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
