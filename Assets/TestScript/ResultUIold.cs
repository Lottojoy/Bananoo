using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ResultUIold : MonoBehaviour
{
    /*public TMP_Text wpmText;
    public TMP_Text accText;
    public TMP_Text scoreText;
    public TMP_Text timeText;

    void Start()
    {
        var score = ScoreManager.Instance;
        if (score != null)
        {
            wpmText.text = "WPM: " + score.WPM.ToString("F1");
            accText.text = "ACC: " + score.ACC.ToString("F1") + "%";
            scoreText.text = "Score: " + score.Score.ToString("F1");
            timeText.text = "Time: " + score.TimeUsed.ToString("F1") + "s";
        }
    }

    public void BackToMenu()
    {
        // ลบ ScoreManager ออกจากเกมเมื่อลงไปที่เมนู
        if (ScoreManager.Instance != null)
        {
            Destroy(ScoreManager.Instance.gameObject);
            ScoreManager.Instance = null;
        }

        SceneManager.LoadScene("MainMenu"); // ใส่ชื่อ Scene ที่คุณจะกลับไป
    }*/
}