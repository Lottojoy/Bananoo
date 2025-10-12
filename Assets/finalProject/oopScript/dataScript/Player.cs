using System;
[System.Serializable]
public class Player
{
    public string playerName;
    public int characterIndex;
    public int streakDays;
    public int currentLessonID;
    public string lastPlayedDate;

    // Constructor
    public Player(string name, int charIndex)
    {
        playerName = name;
        characterIndex = charIndex;
        streakDays = 0;
        currentLessonID = 1;
        lastPlayedDate = DateTime.Now.ToString("yyyy-MM-dd");
    }

    // Set current lesson
    public void SetLesson(int lessonID)
    {
        currentLessonID = lessonID;
        lastPlayedDate = DateTime.Now.ToString("yyyy-MM-dd");
    }

    // เพิ่ม streak
    public void AddStreakDay()
    {
        streakDays++;
    }
}
