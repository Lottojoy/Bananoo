using System;                      // เผื่อใช้ในอนาคต
using System.Collections.Generic;  // ✅ เพื่อใช้ List<>
using UnityEngine;

[System.Serializable]
public class ScoreData
{
    // (ถ้ามีการใช้อยู่แล้วก็เก็บไว้ได้)
    public int totalSyllables;
    public int correctHits;
    public int wrongHits;
    public int corrections;

    public int LessonID;
    public string LessonTitle;   // optional เอาไว้โชว์ชื่อด่าน
    public float WPM;            // คำ/นาที
    public float ACC;            // % ความแม่นยำ (0-100)
    public float TimeUsed;       // วินาที
    public float FinalScore;     // คะแนนรวมที่คิดเอง (optional)

    public int PlayedCharCount; // จำนวนตัวอักษรที่ใช้วัด (ตัดช่องว่างแล้ว)
    public int PlayedWordCount; // จำนวนคำ (กรณีโหมด Word/Audio)
    // ✅ เก็บสถิติอักขระที่พิมพ์ผิด (serialize ง่ายกว่า Dictionary)
    public List<char> WrongChars = new List<char>();
    public List<int>  WrongCounts = new List<int>();

    public void Reset()
    {
        totalSyllables = 0;
        correctHits = 0;
        wrongHits = 0;
        corrections = 0;

        LessonID = 0;
        LessonTitle = string.Empty;
        WPM = 0f;
        ACC = 0f;
        TimeUsed = 0f;
        FinalScore = 0f;
        PlayedCharCount = 0;
        PlayedWordCount = 0;
        WrongChars.Clear();
        WrongCounts.Clear();
    }
 
}
