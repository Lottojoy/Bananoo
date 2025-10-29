// LessonContext.cs
using UnityEngine;

public static class LessonContext
{
    // เก็บบทเรียนที่เลือกไว้ข้ามซีน
    public static Lesson SelectedLesson = null;

    // สำรอง: เผื่ออยากใช้หาใหม่ผ่าน DataManager
    public static int SelectedLessonID = 0;

    // สะดวกไว้เคลียร์ context ตอนออกจากฉาก
    public static void Clear()
    {
        SelectedLesson = null;
        SelectedLessonID = 0;
    }
}
