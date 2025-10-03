using UnityEngine;

public enum LessonType { Character, Word }

[CreateAssetMenu(fileName = "NewLesson", menuName = "Lesson")]
public class Lesson : ScriptableObject
{
    public string lessonID;                 // รหัสประจำด่าน
    public LessonType type;                 // ประเภทบทเรียน

    [Header("Character Lesson")]
    public string[] characters;             // ตัวอักษรให้พิมพ์

    [Header("Word Lesson")]
    public string[] words;                  // คำให้พิมพ์

    public void StartLesson()
    {
        Debug.Log("Lesson Started: " + lessonID);
    }

    public void CompleteLesson()
    {
        Debug.Log("Lesson Completed: " + lessonID);
    }
}
