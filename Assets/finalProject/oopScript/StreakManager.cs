using UnityEngine;
using System;

public class StreakManager : MonoBehaviour
{
    public static StreakManager Instance { get; private set; }

    [Header("Windows (seconds)")]
    [Tooltip("ต้องรอกี่วินาทีก่อนนับสตรีคเพิ่มได้อีกครั้ง")]
    public int cooldownSeconds = 600;    // 10 นาที เทสต์
    [Tooltip("ต้องผ่านอีกครั้งภายในกี่วินาที ไม่งั้นสตรีคจะรีเซ็ต")]
    public int deadlineSeconds = 600;    // 10 นาที เทสต์

    [Header("When hit during cooldown")]
    [Tooltip("กดผ่านด่านระหว่างคูลดาวน์ จะต่ออายุเดดไลน์ไหม")]
    public bool extendDeadlineWhenCooldownHit = true;

    // --- Shortcuts ---
    private Player P => PlayerManager.Instance ? PlayerManager.Instance.CurrentPlayer : null;
    private int CurrentSlot => PlayerPrefs.GetInt("CurrentSlot", 0);

    void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        // ถ้าถึงเวลา reset แล้ว ให้ล้างสตรีค
        if (P != null && P.streakDays > 0 && UtcNowUnix() > P.streakResetAtUtc)
        {
            HardReset();
        }
    }

    /// <summary>
    /// เรียกเมื่อ "ผ่านด่านสำเร็จ" (ด่านใดก็ได้)
    /// return: true = มีการเพิ่มสตรีค, false = ไม่เพิ่ม
    /// </summary>
    public bool OnLevelCleared()
    {
        if (P == null) return false;

        long now = UtcNowUnix();

        // ยังติดคูลดาวน์: ไม่บวกสตรีค
        if (now < P.streakNextEligibleUtc)
        {
            if (extendDeadlineWhenCooldownHit)
            {
                // ขยับเดดไลน์ออกไปใหม่ (ตาม deadlineSeconds)
                P.streakResetAtUtc = now + Math.Max(1L, (long)deadlineSeconds);
                SavePlayer();
            }
            return false;
        }

        // ไม่ติดคูลดาวน์: บวกสตรีค 1
        P.streakDays = Mathf.Max(0, P.streakDays) + 1;
        P.streakLastClearUtc    = now;

        // ตั้งคูลดาวน์รอบถัดไป + เดดไลน์รอบใหม่ (แยกค่ากัน)
        P.streakNextEligibleUtc = now + Math.Max(1L, (long)cooldownSeconds);
        P.streakResetAtUtc      = now + Math.Max(1L, (long)deadlineSeconds);

        SavePlayer();
        return true;
    }

    /// <summary>เช็คแล้วรีเซ็ตสตรีคถ้าหมดเดดไลน์</summary>
    public void SoftRefresh()
    {
        if (P != null && P.streakDays > 0 && UtcNowUnix() > P.streakResetAtUtc)
            HardReset();
    }

    /// <summary>รีเซ็ตสตรีคทันที</summary>
    public void HardReset()
    {
        if (P == null) return;
        P.streakDays = 0;
        P.streakLastClearUtc    = 0;
        P.streakNextEligibleUtc = UtcNowUnix(); // พร้อมเริ่มใหม่ทันที
        P.streakResetAtUtc      = 0;
        SavePlayer();
    }

    // ===== Admin / Tuning =====
    public void AdminSkipCooldown()
    {
        if (P == null) return;
        P.streakNextEligibleUtc = UtcNowUnix(); // ปลดคูลดาวน์
        SavePlayer();
        Debug.Log("[Streak] Admin skipped cooldown");
    }

    public void SetWindows(int cooldownSec, int deadlineSec, bool rebaseDeadlineNow = false)
    {
        cooldownSeconds = Mathf.Max(1, cooldownSec);
        deadlineSeconds = Mathf.Max(1, deadlineSec);
        if (rebaseDeadlineNow && P != null && P.streakDays > 0)
        {
            P.streakResetAtUtc = UtcNowUnix() + Math.Max(1L, (long)deadlineSeconds);
            SavePlayer();
        }
    }

    // ===== Expose to UI =====
    public int GetStreakDays() => (P != null) ? P.streakDays : 0;

    /// <summary>
    /// สำหรับจอที่ต้องการ "ข้อความเดียว":
    /// - ถ้ายังติดคูลดาวน์: label="คูลดาวน์", time=คูลดาวน์คงเหลือ
    /// - ถ้าคูลดาวน์หมดแล้ว: label="เดดไลน์", time=เดดไลน์คงเหลือ (0 = หมด/หลุด)
    /// </summary>
    public (string label, TimeSpan time) GetPrimaryTimer()
    {
        if (P == null) return ("พร้อมเริ่ม", TimeSpan.Zero);
        long now = UtcNowUnix();

        long cd = Math.Max(0L, P.streakNextEligibleUtc - now);
        if (cd > 0L) return ("คูลดาวน์", TimeSpan.FromSeconds((double)cd));

        long dl = Math.Max(0L, P.streakResetAtUtc - now);
        return ("เดดไลน์", TimeSpan.FromSeconds((double)dl));
    }

    /// <summary>ให้ UI ที่อยากโชว์ทั้ง 2 ค่า ดึงคู่เวลาไปแสดง</summary>
    public (TimeSpan untilNext, TimeSpan untilReset, bool hasStreak) GetTimers()
    {
        if (P == null) return (TimeSpan.Zero, TimeSpan.Zero, false);
        long now = UtcNowUnix();
        var next  = TimeSpan.FromSeconds((double)Math.Max(0L, P.streakNextEligibleUtc - now));
        var reset = TimeSpan.FromSeconds((double)Math.Max(0L, P.streakResetAtUtc     - now));
        return (next, reset, P.streakDays > 0);
    }

    // ===== helpers =====
    private long UtcNowUnix() =>
        (long)(DateTime.UtcNow - new DateTime(1970,1,1,0,0,0,DateTimeKind.Utc)).TotalSeconds;

    private void SavePlayer()
    {
        if (PlayerManager.Instance == null || P == null) return;
        PlayerManager.Instance.SavePlayer(CurrentSlot, P); // ใช้พร็อพเพอร์ตี้ ไม่ใช่ CurrentSlot()
    }
}
