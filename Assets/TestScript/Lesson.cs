using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LessonType { Character, Word }

[System.Serializable]
public class Lesson : MonoBehaviour
{
    public string id;                 // รหัสประจำด่าน
    public LessonType type;           // ประเภทบทเรียน
    [Header("=====output only one =====")]
    

    [Header("Character Lesson")]
    public string[] characters; // รายการตัวอักษรที่ให้พิมพ์

    [Header("Word Lesson")]
    public string[] words;// รายการคำที่ให้พิมพ์
    
    public void StartLesson()
    {
        Debug.Log("Lesson Started: " + id);
    }

    public void CompleteLesson()
    {
        Debug.Log("Lesson Completed: " + id);
    }
}
