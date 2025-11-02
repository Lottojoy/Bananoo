using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class StreakUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private GameObject fireImageOn;
    [SerializeField] private GameObject fireImageOff;

    [Header("Admin (optional)")]
    [SerializeField] private Button adminSkipButton;
    [SerializeField] private Button hardResetButton;

    // cache state เพื่อลดการเขียน UI ซ้ำ ๆ
    string _lastPrimaryLabel = "";
    TimeSpan _lastPrimaryTime = TimeSpan.MinValue;
    int _lastStreak = -1;

    void Start()
    {
        if (adminSkipButton)
            adminSkipButton.onClick.AddListener(() =>
            {
                if (StreakManager.Instance) StreakManager.Instance.AdminSkipCooldown();
                ForceRefreshAll();
            });

        if (hardResetButton)
            hardResetButton.onClick.AddListener(() =>
            {
                if (StreakManager.Instance) StreakManager.Instance.HardReset();
                ForceRefreshAll();
            });

        // อัปเดตแบบเป็นมิตรกับ CPU ทุก 0.2 วินาที
        StartCoroutine(Tick());
    }

    IEnumerator Tick()
    {
        var wait = new WaitForSeconds(0.2f);
        while (true)
        {
            var sm = StreakManager.Instance;
            if (sm == null)
            {
                WriteStatus("--");
                WriteTime("--:--:--");
                SetFire(false);
                yield return wait;
                continue;
            }

            // ให้ Manager เช็คหมดเดดไลน์แล้ว reset streak เอง
            sm.SoftRefresh();

            // อัปเดตจำนวนสตรีค
            int streak = sm.GetStreakDays();
            if (streak != _lastStreak)
            {
                _lastStreak = streak;
                SetFire(streak > 0);
                WriteStatus($"เล่นต่อเนื่อง: {streak} ครั้ง");
            }

            // อัปเดต "บรรทัดหลัก" (คูลดาวน์ หรือ เดดไลน์)
            var primary = sm.GetPrimaryTimer(); // (label, time)
            if (primary.label != _lastPrimaryLabel || primary.time != _lastPrimaryTime)
            {
                _lastPrimaryLabel = primary.label;
                _lastPrimaryTime  = primary.time;
                WriteTime($"{primary.label}: {FormatHHMMSS(primary.time)}");
            }

            yield return wait;
        }
    }

    // ===== UI helpers =====
    void WriteStatus(string s)
    {
        if (statusText && statusText.text != s) statusText.text = s;
    }

    void WriteTime(string s)
    {
        if (timeText && timeText.text != s) timeText.text = s;
    }

    void SetFire(bool on)
    {
        if (fireImageOn)  fireImageOn.SetActive(on);
        if (fireImageOff) fireImageOff.SetActive(!on);
    }

    string FormatHHMMSS(TimeSpan t)
    {
        int totalHours = (int)t.TotalHours;
        return $"{totalHours:00}:{t.Minutes:00}:{t.Seconds:00}";
    }

    // เรียกเมื่ออยากรีเฟรชทันที (ปุ่มแอดมิน)
    void ForceRefreshAll()
    {
        _lastStreak = -1;
        _lastPrimaryLabel = "";
        _lastPrimaryTime  = TimeSpan.MinValue;
    }
}
