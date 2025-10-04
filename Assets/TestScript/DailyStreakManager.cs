using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DailyStreakManager : MonoBehaviour
{
    [Header("UI")]
    public RawImage streakLight;        // รูปไฟ
    public Texture lightOn;             // ไฟติด
    public Texture lightOff;            // ไฟดับ
    public TMP_Text streakText;         // ข้อความ streak
    public TMP_Text nextResetText;      // ข้อความบอกเวลา reset
    public TMP_Text currentTimeText;    // เวลาปัจจุบัน
    public TMP_Text streakStatusText;   // ข้อความบอกว่าเล่นเพิ่ม streak ได้หรือยัง

    private int slot;
    private Player playerData;

    void Start()
    {
        slot = PlayerPrefs.GetInt("CurrentSlot", 0);
        playerData = SaveManager.LoadPlayer(slot);

        if (playerData == null)
        {
            Debug.LogWarning("ไม่พบข้อมูลผู้เล่น");
            return;
        }

        // ถ้ายังไม่มีค่าเวลาใน GameData ให้เซ็ตครั้งแรก
        if (GameData.NextStreakResetTime == default)
            GameData.NextStreakResetTime = DateTime.Now.AddMinutes(3); // รีเซ็ต streakDays ทุก 3 นาที
        if (GameData.NextCanAddTime == default)
            GameData.NextCanAddTime = DateTime.Now.AddMinutes(1);      // อัปเดต CanAddStreak ทุก 1 นาที

        UpdateCanAddStreak();
        UpdateUI();
    }

    void Update()
    {
        if (playerData == null) return;

        DateTime now = DateTime.Now;

        // รีเซ็ต streakDays ทุก 3 นาที
        if (now >= GameData.NextStreakResetTime)
        {
            playerData.streakDays = 0;
            SaveManager.SavePlayer(slot, playerData);
            GameData.NextStreakResetTime = now.AddMinutes(3);
            Debug.Log("รีเซ็ต streakDays ตามเวลา 3 นาที");
        }

        // อัปเดต CanAddStreak ทุก 1 นาที
        if (now >= GameData.NextCanAddTime)
        {
            UpdateCanAddStreak();
            GameData.NextCanAddTime = now.AddMinutes(1);
        }

        UpdateUI();
    }

    void UpdateCanAddStreak()
    {
        DateTime today = DateTime.Now.Date;
        DateTime lastPlayed = string.IsNullOrEmpty(playerData.lastPlayedDate) ? today.AddDays(-1) : DateTime.Parse(playerData.lastPlayedDate).Date;

        GameData.CanAddStreak = lastPlayed < today;
    }

    public void AddStreak()
    {
        if (!GameData.CanAddStreak)
        {
            Debug.Log("วันนี้นับ streak ไปแล้ว");
            return;
        }

        DateTime today = DateTime.Now.Date;
        DateTime lastPlayed = string.IsNullOrEmpty(playerData.lastPlayedDate) ? today.AddDays(-1) : DateTime.Parse(playerData.lastPlayedDate).Date;

        if (lastPlayed == today.AddDays(-1))
        {
            playerData.streakDays++;
        }
        else
        {
            playerData.streakDays = 1;
        }

        playerData.lastPlayedDate = today.ToString("yyyy-MM-dd");
        SaveManager.SavePlayer(slot, playerData);

        GameData.CanAddStreak = false; // เล่นแล้ววันนี้
        Debug.Log("เพิ่ม streak! ตอนนี้ streakDays = " + playerData.streakDays);

        UpdateUI();
    }

    void UpdateUI()
    {
        // ไฟ
        streakLight.texture = playerData.streakDays > 0 ? lightOn : lightOff;

        // ข้อความ streak
        streakText.text = "Streak: " + playerData.streakDays + " วัน";

        // เวลาที่จะรีเซ็ต streak
        TimeSpan resetTimeLeft = GameData.NextStreakResetTime - DateTime.Now;
        nextResetText.text = $"รีเซ็ต streak ใน {resetTimeLeft.Minutes:D2}:{resetTimeLeft.Seconds:D2} นาที";

        // เวลาปัจจุบัน
        currentTimeText.text = "เวลาปัจจุบัน: " + DateTime.Now.ToString("HH:mm:ss");

        // ข้อความ CanAddStreak
        if (GameData.CanAddStreak)
            streakStatusText.text = "สามารถเล่นเพื่อเพิ่ม streak ได้";
        else
        {
            TimeSpan timeLeft = GameData.NextCanAddTime - DateTime.Now;
            streakStatusText.text = $"เล่นวันนี้ไปแล้ว รออีก {timeLeft.Minutes:D2}:{timeLeft.Seconds:D2} ถึงนับอีกได้";
        }
    }
}
