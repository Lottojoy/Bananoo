using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Player CurrentPlayer { get; private set; }
    public Lesson CurrentLesson { get; private set; }
    public int CurrentMapID { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        

        // กันกรณี deserialize แล้วเป็น 0
        if (CurrentPlayer.currentLessonID < 1)
            CurrentPlayer.currentLessonID = 1;


        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log($"[GM] Loaded player {CurrentPlayer.playerName}, progress={CurrentPlayer.currentLessonID}");
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

   public void FinishLesson(int lessonID, float accuracy)
{
    Debug.Log($"Finished lesson {lessonID} with accuracy {accuracy * 100f:F1}%");

    if (accuracy >= 0.8f)
        CurrentPlayer.AddStreakDay();
}
    public void SetCurrentMap(int mapID)
{
    CurrentMapID = mapID;
    Debug.Log($"GameManager: ตั้งค่า CurrentMap เป็น {mapID}");
}

}
