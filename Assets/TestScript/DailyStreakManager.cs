using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DailyStreakManager : MonoBehaviour
{
    [Header("UI")]
    public RawImage streakLight;   // รูปไฟ
    public Texture lightOn;        // ไฟติด
    public Texture lightOff;       // ไฟดับ
    public TMP_Text streakText;    // ข้อความ streak
    public TMP_Text nextResetText; // ข้อความบอกเวลา reset
    public TMP_Text currentTimeText; // เวลาปัจจุบัน (เช่น 9:00 PM)

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

        CheckStreak();
        UpdateUI();
    }

    // เรียกเมื่อเล่นผ่าน 1 ด่าน
    public void AddStreak()
    {
        DateTime today = DateTime.Now.Date;
        DateTime lastPlayed = DateTime.Parse(playerData.lastPlayedDate).Date;

        if (lastPlayed == today)
        {
            Debug.Log("วันนี้นับไปแล้ว");
            return;
        }

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

        Debug.Log("Streak ปัจจุบัน: " + playerData.streakDays);
        UpdateUI();
    }

    void CheckStreak()
    {
        DateTime today = DateTime.Now.Date;

        if (string.IsNullOrEmpty(playerData.lastPlayedDate))
        {
            playerData.lastPlayedDate = today.ToString("yyyy-MM-dd");
            SaveManager.SavePlayer(slot, playerData);
            return;
        }

        DateTime lastPlayed = DateTime.Parse(playerData.lastPlayedDate).Date;
        int daysDiff = (today - lastPlayed).Days;

        if (daysDiff > 1)
        {
            playerData.streakDays = 0;
            SaveManager.SavePlayer(slot, playerData);
            Debug.Log("ขาดวัน streak reset");
        }
    }

    void UpdateUI()
    {
        // ไฟ
        streakLight.texture = playerData.streakDays > 0 ? lightOn : lightOff;

        // streak
        streakText.text = "Streak: " + playerData.streakDays + " วัน";

        // reset (เที่ยงคืน)
        DateTime tomorrow = DateTime.Now.Date.AddDays(1);
        TimeSpan timeLeft = tomorrow - DateTime.Now;
        nextResetText.text = $"รีเซ็ตใน {timeLeft.Hours:D2}:{timeLeft.Minutes:D2}:{timeLeft.Seconds:D2}";

        // เวลาปัจจุบัน
        currentTimeText.text = "เวลาประเทศไทย: "+DateTime.Now.ToString("h:mm tt"); 
        // เช่น 9:05 PM
    }

    void Update()
    {
        if (playerData != null)
        {
            UpdateUI();
        }
    }
}
