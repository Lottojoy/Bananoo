using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TypingManager : MonoBehaviour
{
    private Lesson currentLesson;      // บทเรียน
    public TMP_Text typingText;       // TMP Text สำหรับแสดงโจทย์พร้อม Highlight

    private string[] targets;
    private int currentIndex = 0;     // Target ปัจจุบัน
    private string currentInput = "";
    private HashSet<int> correctedIndexes = new HashSet<int>();

    // ตัวนับสำหรับ ScoreManager
    private int totalSyllables = 0;
    private int correctHits = 0;
    private int wrongHits = 0;
    private int corrections = 0;

    public bool IsCompleted { get; private set; } = false;

    void Start()
    {
        currentLesson = LoadLessonByID(GameData.CurrentLessonID);

        if(currentLesson == null)
        {
            Debug.LogError("Lesson not found!");
            return;
        }
        // โหลดโจทย์
        if (currentLesson.type == LessonType.Word && currentLesson.words.Length > 0)
            targets = currentLesson.words;
        else if (currentLesson.type == LessonType.Character && currentLesson.characters.Length > 0)
            targets = currentLesson.characters;

        // บังคับให้ target ต้องมีเว้นวรรคต่อท้าย
        for (int i = 0; i < targets.Length; i++)
            if (!targets[i].EndsWith(" "))
                targets[i] += " ";

        // นับจำนวนตัวอักษรรวม
        foreach (var t in targets) totalSyllables += t.Length;

        UpdateTypingText();

        Debug.Log($"[Typing Debug] Lesson Loaded | Total Syllables: {totalSyllables} | Total Targets: {targets.Length}");
    }

    void Update()
    {
        if (targets == null || targets.Length == 0 || IsCompleted) return;

        // Backspace
        if (Input.GetKeyDown(KeyCode.Backspace) && currentInput.Length > 0)
        {
            int lastIndex = currentInput.Length - 1;
            correctedIndexes.Add(lastIndex);
            currentInput = currentInput.Substring(0, lastIndex);
            corrections++;
            UpdateTypingText();

            Debug.Log($"[Typing Debug] 🔙 Backspace | CurrentInput: \"{currentInput}\" | Progress: {currentIndex}/{targets.Length}");
        }

        // ตัวอักษรทั่วไป
        foreach (char c in Input.inputString)
        {
            if (c == '\b' || c == '\n' || c == '\r') continue;

            int i = currentInput.Length;
            if (i < targets[currentIndex].Length)
            {
                if (c == targets[currentIndex][i])
                    correctHits++;
                else
                    wrongHits++;
            }
            else
            {
                wrongHits++; // กรณีพิมพ์เกิน
            }

            currentInput += c;
            UpdateTypingText();

            Debug.Log($"[Typing Debug] Typing | Input: \"{currentInput}\" | Progress: {currentIndex}/{targets.Length}");

            // ✅ ตรวจว่า target ถูกครบแล้ว แม้พิมพ์ผิด
            if (currentInput.Length >= targets[currentIndex].Length)
            {
                currentIndex++;
                currentInput = "";
                correctedIndexes.Clear();

                Debug.Log($"[Typing Debug] ✅ Completed syllable {currentIndex}/{targets.Length}");
            }
        }

        // ✅ ตรวจว่าจบ lesson ทุกเฟรม
        if (currentIndex >= targets.Length && !IsCompleted)
        {
            IsCompleted = true;
            Debug.Log($"[Typing Debug] 🎉 Lesson Completed! Total: {totalSyllables}");
            StartCoroutine(LoadResultAfterDelay(2f));
        }

        // ✅ ส่งข้อมูลไป ScoreManager
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.SetData(totalSyllables, correctHits, wrongHits, corrections);
        }
    }

    void UpdateTypingText()
    {
        if (targets == null || currentIndex >= targets.Length) return;

        string target = targets[currentIndex];
        string display = "";

        for (int i = 0; i < target.Length; i++)
        {
            string markColor = "";

            if (i < currentInput.Length)
            {
                if (currentInput[i] == target[i])
                    markColor = correctedIndexes.Contains(i) ? "#FFFF0095" : "#00FF0095";
                else
                    markColor = "#FF000095";
            }

            if (i == target.Length - 1 && currentInput.Length >= target.Length)
                display += !string.IsNullOrEmpty(markColor) ? $"<mark={markColor}>{target[i]}</mark>" : $"|{target[i]}";
            else if (i == currentInput.Length)
                display += !string.IsNullOrEmpty(markColor) ? $"<mark={markColor}>{target[i]}|</mark>" : $"{target[i]}";
            else
                display += !string.IsNullOrEmpty(markColor) ? $"<mark={markColor}>{target[i]}</mark>" : target[i];
        }

        typingText.text = display;
    }

    IEnumerator LoadResultAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneLoader.LoadScene("ResultScene");
    }

    Lesson LoadLessonByID(string id)
    {
        // โหลดทุก Lesson asset ใน Resources/Lessons
        Lesson[] allLessons = Resources.LoadAll<Lesson>("Lessons");
        foreach(var l in allLessons)
        {
            if(l.lessonID == id)
                return l;
        }
        return null;
    }
    // Property สำหรับ Script อื่น ๆ
    public string CurrentInput => currentInput;
    public string CurrentTarget => (targets != null && currentIndex < targets.Length) ? targets[currentIndex] : null;
}
