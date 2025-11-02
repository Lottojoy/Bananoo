using System;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance { get; private set; }

    public string CurrentLessonID { get; set; }
    public int CurrentStageID { get; set; }
    public bool CanAddStreak { get; set; }

    public DateTime NextStreakResetTime { get; set; }
    public DateTime NextCanAddTime { get; set; }

    public ScoreData ScoreData { get; private set; } = new ScoreData();

    // ✅ เพิ่มตรงนี้
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetFromLesson(float wpm, float accPercent, float timeUsedSec, Lesson lesson)
    {
        ScoreData.WPM       = wpm;
        ScoreData.ACC       = accPercent;                // 0–100
        ScoreData.TimeUsed  = timeUsedSec;
        ScoreData.LessonID  = lesson ? lesson.LessonID : 0;
        ScoreData.LessonTitle = lesson ? lesson.name : "Unknown";
        
    }

    public void ResetAll()
    {
        ScoreData = new ScoreData();
    }
    public void SetScore(ScoreData d)
{
    ScoreData = d;
}
}
