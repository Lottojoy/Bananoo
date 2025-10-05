using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManagerold : MonoBehaviour
{
   /* public static ScoreManager Instance;

    private int totalSyllables = 0;
    private int correctHits = 0;
    private int wrongHits = 0;
    private int corrections = 0;

    private float startTime = -1f;
    private float endTime = 0f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // อยู่รอดข้าม Scene
        }
        else
        {
            Destroy(gameObject); // กันซ้ำ
        }
    }

    void Update()
    {
        // เริ่มจับเวลาเมื่อมีการกดตัวแรก
        if (startTime < 0f && TypingManagerHasInput())
            startTime = Time.time;

        // ถ้า TypingManager บอกว่าเสร็จ → เก็บเวลา
        if (TypingManagerCompleted() && endTime == 0f)
            endTime = Time.time;

        // DEBUG: กดปุ่ม D เพื่อดูค่า
        if (Input.GetKeyDown(KeyCode.P))
        {
            DebugStats();
        }
    }

    // ตัวอย่าง getter สำหรับข้อมูล (ใช้ TypingManager ส่งเข้ามาจริงๆ จะดีกว่า)
    public void SetData(int total, int correct, int wrong, int fix)
    {
        totalSyllables = total;
        correctHits = correct;
        wrongHits = wrong;
        corrections = fix;
    }

    private bool TypingManagerHasInput()
    {
        return (FindObjectOfType<TypingManager>()?.CurrentInput.Length ?? 0) > 0;
    }

    private bool TypingManagerCompleted()
    {
        return (FindObjectOfType<TypingManager>()?.IsCompleted ?? false);
    }

    public float TimeUsed => (startTime < 0f) ? 0f : ((endTime > 0f ? endTime : Time.time) - startTime);

    public float WPM
    {
        get
        {
            float minutes = TimeUsed / 60f;
            if (minutes <= 0f) return 0f;
            return (correctHits / 5f) / minutes; // 1 word = 5 ตัวอักษร
        }
    }

    public float ACC
    {
        get
        {
            int totalHits = correctHits + wrongHits + corrections;
            if (totalHits == 0) return 0f;
            return (float)correctHits / totalHits * 100f;
        }
    }

    public float Score => WPM * (ACC / 100f);

    public void ResetScore()
    {
        totalSyllables = 0;
        correctHits = 0;
        wrongHits = 0;
        corrections = 0;
        startTime = -1f;
        endTime = 0f;
    }

    // ✅ ฟังก์ชัน Debug
    public void DebugStats()
    {
        Debug.Log($"ScoreManager Stats: Total Syllables:{totalSyllables} ,Correct Hits:{correctHits},Wrong Hits:{wrongHits},Corrections:{corrections},Time Used:{TimeUsed:F2}");
        Debug.Log($"WPM:{WPM:F2},ACC:{ACC:F2},Score:{Score:F2} ");
        
    }*/
}
