using System;
[System.Serializable]
public class Player
{
    public string playerName; 
    public int characterIndex; //อ้างอิงอวตาร์ ตัวละคร
    public int currentLessonID;
    
    // --- Streak ---
    public int streakDays;          // วันต่อเนื่องสะสม
    public long streakLastClearUtc; // เวลาที่ผ่านด่านครั้งล่าสุด (UTC)
    public long streakNextEligibleUtc; // เวลาที่ "พร้อมบวกครั้งถัดไป" (คูลดาวน์) (UTC)
    public long streakResetAtUtc;      // เดดไลน์ที่ต้องผ่านอีกครั้งก่อนรีเซ็ต (UTC)

    // Constructor
    public Player(string name, int charIndex)
    {
        playerName = name;
        characterIndex = charIndex;

        streakDays = 0;
        streakLastClearUtc = 0;
        long now = UtcNow();
        streakNextEligibleUtc = now; // สร้างใหม่ให้พร้อมนับได้ทันที
        streakResetAtUtc = 0;

        currentLessonID = 1;
        
    }

    // Set current lesson
    public void SetLesson(int lessonID)
    {
        currentLessonID = lessonID;
        
    }

    // เพิ่ม streak
    public void AddStreakDay()
    {
        streakDays++;
    }
    static long UtcNow() =>
        (long)(DateTime.UtcNow - new DateTime(1970,1,1,0,0,0, DateTimeKind.Utc)).TotalSeconds;
}
