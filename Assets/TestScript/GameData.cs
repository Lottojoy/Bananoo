using System;
public static class GameData
{
    public static string CurrentLessonID;
    public static int CurrentStageID;
     public static bool CanAddStreak = false;

     // เก็บเวลา reset และเวลาเพิ่ม streak
    public static DateTime NextStreakResetTime;
    public static DateTime NextCanAddTime;
}
