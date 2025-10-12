using UnityEngine;

public class DebugUI : MonoBehaviour
{
    public static DebugUI Instance { get; private set; }

    private string debugText = "";

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

    private void Update()
    {
        // กด "=" เพื่อโชว์ Debug snapshot ล่าสุด
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            UpdateDebugData();
            ShowDebug();
        }
    }

    /// <summary>
    /// เรียกเมื่อ Player, Map หรือ GameData เปลี่ยน
    /// </summary>
    public void UpdateDebugData()
    {
        // Player info
        var player = GameManager.Instance.CurrentPlayer;
        string playerInfo = player != null
            ? $"[Player] Name: {player.playerName}, CharIndex: {player.characterIndex}, Streak: {player.streakDays}, Lesson: {player.currentLessonID}"
            : "[Player] ไม่มีผู้เล่น";

        // GameDataManager info
        var gameData = GameDataManager.Instance;
        string gameDataInfo = gameData != null
            ? $"[GameData] CurrentLessonID: {gameData.CurrentLessonID}, CurrentStageID: {gameData.CurrentStageID}, CanAddStreak: {gameData.CanAddStreak}\n" +
              $"NextStreakReset: {gameData.NextStreakResetTime}, NextCanAddTime: {gameData.NextCanAddTime}"
            : "[GameData] ไม่มีข้อมูล";

        // Map info
        string mapInfo = $"[Map] CurrentMapID: {GameManager.Instance.CurrentMapID}";

        // รวมข้อความทั้งหมด
        debugText = $"{playerInfo}\n{gameDataInfo}\n{mapInfo}";
    }

    private void ShowDebug()
    {
        Debug.Log(debugText);
    }
}
