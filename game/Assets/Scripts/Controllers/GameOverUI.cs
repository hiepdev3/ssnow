using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;


public class GameOverUI : MonoBehaviour
{
    public TextMeshProUGUI distanceText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    public static List<MarkData> markList = new List<MarkData>();

    private void Start()
    {
        StartCoroutine(AddMark(GameData.Coins, (int)GameData.Distance));
        StartCoroutine(GetMarkHighest());
        // Hiển thị ngay khi scene load hoặc object active
       // Setup(GameData.Distance, GameData.Coins, markList);
    }

    public void Setup(float distance, int coins, List<MarkData> markList)
    {
        gameObject.SetActive(true);

        distanceText.text = "Your Distance: " + Mathf.FloorToInt(distance) + " m";
        scoreText.text = "Your Coins: " + coins;

        for (int i = 0; i < markList.Count; i++)
        {
            Debug.Log($"markList[{i}]: id={markList[i].id}, numMart={markList[i].numMart}, distance={markList[i].distance}");
        }

        highScoreText.text = "HighScore: Coins: " + markList[0].numMart + ", Distance: " + markList[0].distance;
    }

    public void OnPlayAgain()
    {
        SceneManager.LoadScene("Scene0");
    }



    private IEnumerator GetMarkHighest()
    {
        string url = "http://localhost:5000/api/Mark/getMarkHighest";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            if (request.result != UnityWebRequest.Result.Success)
#else
            if (request.isNetworkError || request.isHttpError)
#endif
            {
                Debug.LogError("API Error: " + request.error);
            }
            else
            {
                // Unity's JsonUtility does not support top-level arrays, so we use a wrapper class
                MarkApiResponse response = JsonUtility.FromJson<MarkApiResponse>(FixJson(request.downloadHandler.text));
                if (response != null && response.success)
                {
                    markList = response.data;
                    Debug.Log("Loaded mark list, count: " + markList.Count);
                    Setup(GameData.Distance, GameData.Coins, markList);
                }
            }
        }
    }

    public IEnumerator AddMark(int numMark, int distance)
    {
        string url = "http://localhost:5000/api/Mark/addMark";
        MarkPostData postData = new MarkPostData { numMark = numMark, distance = distance };
        string jsonData = JsonUtility.ToJson(postData);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("accept", "*/*");

            yield return request.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            if (request.result != UnityWebRequest.Result.Success)
#else
        if (request.isNetworkError || request.isHttpError)
#endif
            {
                Debug.LogError("AddMark API Error: " + request.error);
            }
            else
            {
                Debug.Log("AddMark Success: " + request.downloadHandler.text);
            }
        }
    }

    // Fixes JSON for Unity's JsonUtility if needed
    private string FixJson(string value)
    {
        // If the JSON is not wrapped, wrap it (not needed here, but kept for safety)
        if (!value.TrimStart().StartsWith("{"))
            value = "{\"data\":" + value + "}";
        return value;
    }
}


[System.Serializable]
public class MarkData
{
    public int id;
    public int numMart;
    public int distance;
}


[System.Serializable]
public class MarkApiResponse
{
    public bool success;
    public List<MarkData> data;
}

[System.Serializable]
public class MarkPostData
{
    public int numMark;
    public int distance;
}