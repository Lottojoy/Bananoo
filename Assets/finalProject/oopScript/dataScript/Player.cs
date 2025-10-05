using System;
[System.Serializable]
public class Player
{
    public string playerName;
    public int characterIndex;
    public int streakDays;
    public string currentLessonID;
    public string lastPlayedDate;

    // Constructor
    public Player(string name, int charIndex)
    {
        playerName = name;
        characterIndex = charIndex;
        streakDays = 0;
        currentLessonID = "";
        lastPlayedDate = DateTime.Now.ToString("yyyy-MM-dd");
    }

    // Set current lesson
    public void SetLesson(string lessonID)
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
