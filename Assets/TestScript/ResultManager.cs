using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class ResultManager : MonoBehaviour
{
   /* public Button backButton;
    private int slot;

    void Start()
    {
        slot = PlayerPrefs.GetInt("CurrentSlot", 0);

        if (backButton != null)
            backButton.onClick.AddListener(OnBackToMain);
    }

    void OnBackToMain()
    {
        Player player = SaveManager.LoadPlayer(slot);
        if (player == null)
        {
            Debug.LogWarning("ไม่มีข้อมูล Player ใน slot นี้");
            SceneManager.LoadScene("MainScene");
            return;
        }

        Debug.Log($"[Result] Player {player.playerName} เล่นถึงด่าน {GameData.CurrentStageID} (ปัจจุบัน={player.currentLessonIndex})");

        // อัปเดต currentLessonIndex ถ้าผ่านด่านใหม่
        if (GameData.CurrentStageID >= player.currentLessonIndex)
        {
            player.currentLessonIndex = GameData.CurrentStageID + 1;
            Debug.Log($"🎉 อัปเดตบทเรียนใหม่ = {player.currentLessonIndex}");
        }

        // เพิ่ม streak ถ้า GameData.CanAddStreak เป็น true
        if (GameData.CanAddStreak)
        {
            DateTime today = DateTime.Now.Date;
            DateTime lastPlayed = string.IsNullOrEmpty(player.lastPlayedDate)
                                    ? today.AddDays(-2) // ถ้าไม่มีค่าเก่า ให้ถือว่าไม่ได้เล่นเมื่อวาน
                                    : DateTime.Parse(player.lastPlayedDate).Date;

            if (lastPlayed == today.AddDays(-1))
            {
                player.streakDays++; // เล่นต่อเนื่อง
            }
            else
            {
                player.streakDays = 1; // เริ่ม streak ใหม่
            }

            player.lastPlayedDate = today.ToString("yyyy-MM-dd");
            GameData.CanAddStreak = false; // เล่นแล้ววันนี้
            Debug.Log($"✅ เพิ่ม streak! ตอนนี้ streakDays = {player.streakDays}");
        }

        // บันทึก player หลังจากอัปเดตทุกอย่าง
        SaveManager.SavePlayer(slot, player);

        // รีเซ็ตแค่ GameData ที่เกี่ยวกับบทเรียน/ด่าน ไม่กระทบเวลา streak
        ResetGameData();

        // กลับไปหน้า MainScene
        SceneManager.LoadScene("MainScene");
    }

    void ResetGameData()
    {
        GameData.CurrentLessonID = null;
        GameData.CurrentStageID = 0;
        // GameData.CanAddStreak จะคงอยู่เพราะเราต้องใช้ต่อ
        Debug.Log("[Result] รีเซ็ต GameData บทเรียนและด่านเรียบร้อย");
    }*/
}
