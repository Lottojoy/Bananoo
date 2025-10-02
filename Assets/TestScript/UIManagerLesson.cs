using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class UIManagerLesson : MonoBehaviour
{
    public TMP_Text wpmText;
    public TMP_Text accText;
    public TMP_Text timeText;

    void Update()
    {
        if (ScoreManager.Instance == null) return;

        wpmText.text = "WPM: " + ScoreManager.Instance.WPM.ToString("F1");
        accText.text = "ACC: " + ScoreManager.Instance.ACC.ToString("F1") + "%";
        timeText.text = "Time: " + ScoreManager.Instance.TimeUsed.ToString("F1") + "s";
    }
}
