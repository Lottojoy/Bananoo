using UnityEngine;

public class DebugManager : MonoBehaviour
{
   /* private int slot = 0;          // slot ปัจจุบันที่ใช้เทส
    private Player currentPlayer;

    void Start()
    {
        LoadPlayer(); // โหลดตอนเริ่มเกม
    }

    void Update()
    {
        // ดูข้อมูล Player
        if (Input.GetKeyDown(KeyCode.P))
        {
            ShowPlayerData();
        }

        // เพิ่มวันต่อเนื่อง
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentPlayer.streakDays++;
            Debug.Log($"[DEBUG] เพิ่ม streakDays = {currentPlayer.streakDays}");
        }

        // เพิ่มเลเวลบทเรียน
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentPlayer.currentLessonIndex++;
            Debug.Log($"[DEBUG] เพิ่ม currentLessonIndex = {currentPlayer.currentLessonIndex}");
        }

        // รีเซ็ตวันต่อเนื่อง
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            currentPlayer.streakDays = 0;
            Debug.Log("[DEBUG] รีเซ็ต streakDays = 0");
        }

        // เซฟข้อมูล
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SaveManager.SavePlayer(slot, currentPlayer);
            Debug.Log($"[DEBUG] 💾 บันทึก Slot {slot} แล้ว");
        }

        // โหลดข้อมูลใหม่
        if (Input.GetKeyDown(KeyCode.F9))
        {
            LoadPlayer();
            Debug.Log($"[DEBUG] 🔄 โหลดข้อมูล Slot {slot} แล้ว");
        }

        // Debug ค่า GameData
        if (Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log($"[DEBUG] GameData: LessonID={GameData.CurrentLessonID}, StageID={GameData.CurrentStageID}, CanAddStreak={GameData.CanAddStreak}, NextStreakResetTime={GameData.NextStreakResetTime}, NextCanAddTime={GameData.NextCanAddTime}");
        }
    }

    void LoadPlayer()
    {
        currentPlayer = SaveManager.LoadPlayer(slot);
        if (currentPlayer == null)
        {
            currentPlayer = new Player()
            {
                playerName = "TestPlayer",
                characterIndex = 0,
                streakDays = 0,
                currentLessonIndex = 0,
                lastPlayedDate = System.DateTime.Now.ToString("yyyy-MM-dd")
            };
            Debug.Log($"[DEBUG] ⚠️ ไม่มีข้อมูลใน Slot {slot} — สร้างใหม่");
        }
    }

    void ShowPlayerData()
    {
        Debug.Log(
            $"[DEBUG] Player Data (Slot {slot}):\n" +
            $"🧍 Name: {currentPlayer.playerName}\n" +
            $"🎭 CharIndex: {currentPlayer.characterIndex}\n" +
            $"🔥 Streak: {currentPlayer.streakDays}\n" +
            $"📘 Lesson: {currentPlayer.currentLessonIndex}\n" +
            $"📅 LastPlayed: {currentPlayer.lastPlayedDate}"
        );
    }*/
}
