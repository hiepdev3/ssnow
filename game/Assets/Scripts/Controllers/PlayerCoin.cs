using System;
using UnityEngine;

public class PlayerCoin : MonoBehaviour
{
    public int coinCount = 0;

    public void AddCoin(int amount)
    {
        coinCount += amount;
        Debug.Log("Collected Coin! Total: " + coinCount);
        Console.WriteLine("Collected Coin! Total: " + coinCount);
        FindObjectOfType<HUDTextController>().AddCoin(coinCount);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
