using UnityEngine;
using TMPro;

public class ResultUI : MonoBehaviour
{
    [SerializeField] private TMP_Text wpmText;
    [SerializeField] private TMP_Text accText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text timeText;

    void Start()
    {
        var data = GameDataManager.Instance.ScoreData;
        wpmText.text = $"WPM: {data.WPM:F1}";
        accText.text = $"ACC: {data.ACC:F1}%";
        scoreText.text = $"Score: {data.FinalScore:F1}";
        timeText.text = $"Time: {data.TimeUsed:F1}s";
    }

    public void BackToMenu()
    {
        GameDataManager.Instance.ResetAll();
        GameFlowManager.Instance.LoadMainMenu();
    }
}
