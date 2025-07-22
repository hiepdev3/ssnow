using UnityEngine;
using TMPro;

public class HUDTextController : MonoBehaviour
{
    public TextMeshProUGUI distanceText;
    public TextMeshProUGUI coinsText;

    public float distance;
    public int coins;

    void Update()
    {
        // Giả sử distance tăng theo thời gian chạy
        distance += Time.deltaTime * 5f; // ví dụ mỗi giây tăng 5m

        distanceText.text = "Distance: " + Mathf.FloorToInt(distance) + " m";
        coinsText.text = "Coins: " + coins;
    }

    public void AddCoin(int amount)
    {
        coins = amount;
    }
    public float GetDistance() => distance;
    public int GetCoins() => coins;
}

public static class GameData
{
    public static float Distance;
    public static int Coins;
}