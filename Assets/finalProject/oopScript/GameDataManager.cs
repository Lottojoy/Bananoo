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

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ResetAll()
    {
        ScoreData.Reset();
        CurrentLessonID = "";
        CurrentStageID = 0;
        CanAddStreak = false;
    }
}
