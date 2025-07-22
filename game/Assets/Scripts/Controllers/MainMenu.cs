using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Scene0"); // Thay bằng tên scene game thật
    }

    public void ViewMark()
    {
        // Ví dụ: Load scene điểm số, hoặc mở bảng điểm (sẽ tùy bạn xử lý)
        Debug.Log("Xem điểm số");
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
