using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Player CurrentPlayer { get; private set; }
    public Lesson CurrentLesson { get; private set; }

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

    public void StartGame(Player player)
    {
        CurrentPlayer = player;
        Debug.Log($"Game started with player: {player.playerName}");
    }

    public void SelectLesson(Lesson lesson)
    {
        CurrentLesson = lesson;
        if (CurrentPlayer != null)
            CurrentPlayer.SetLesson(lesson.LessonID);

        Debug.Log($"Selected lesson: {lesson.LessonID}");
    }

    public void FinishLesson(string lessonID, float accuracy)
    {
        Debug.Log($"Finished lesson {lessonID} with accuracy {accuracy * 100f:F1}%");

        // ตัวอย่าง: เพิ่ม streak ถ้า accuracy >= 80%
        if (accuracy >= 0.8f)
            CurrentPlayer.AddStreakDay();
    }
}
