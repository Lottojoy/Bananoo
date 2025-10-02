using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TypingManager : MonoBehaviour
{
   public Lesson currentLesson;      // บทเรียน
    public TMP_Text typingText;       // TMP Text สำหรับแสดงโจทย์พร้อม Highlight

    private string[] targets;
    private int currentIndex = 0;     // Target ปัจจุบัน
    private string currentInput = "";
    private HashSet<int> correctedIndexes = new HashSet<int>(); // เก็บ index ที่เคยถูกแล้วแก้ไข (Backspace)

    void Start()
    {
        // โหลดโจทย์
        if (currentLesson.type == LessonType.Word && currentLesson.words.Length > 0)
            targets = currentLesson.words;
        else if (currentLesson.type == LessonType.Character && currentLesson.characters.Length > 0)
            targets = currentLesson.characters;

        // บังคับให้ target ต้องมีเว้นวรรคต่อท้าย
        for (int i = 0; i < targets.Length; i++)
            if (!targets[i].EndsWith(" "))
                targets[i] += " ";

        UpdateTypingText();
    }

    void Update()
    {
        if (targets == null || targets.Length == 0) return;

        // Backspace
        if (Input.GetKeyDown(KeyCode.Backspace) && currentInput.Length > 0)
        {
            int lastIndex = currentInput.Length - 1;
            correctedIndexes.Add(lastIndex);
            currentInput = currentInput.Substring(0, lastIndex);
            UpdateTypingText();
        }

        // ตัวอักษรทั่วไป
        foreach (char c in Input.inputString)
        {
            if (c == '\b' || c == '\n' || c == '\r') continue;

            currentInput += c;
            UpdateTypingText();

            // ตรวจว่า target ถูกครบ
            if (currentInput == targets[currentIndex])
            {
                Debug.Log("Completed: " + targets[currentIndex]);
                currentIndex++;
                currentInput = "";
                correctedIndexes.Clear();

                if (currentIndex >= targets.Length)
                    Debug.Log("Lesson Completed!");
                else
                    UpdateTypingText();
            }
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
                    markColor = correctedIndexes.Contains(i) ? "#FFFF0095" : "#00FF0095"; // เหลือง / เขียว
                else
                    markColor = "#FF000095"; // แดง
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
 
    // Property สำหรับ UI หรือ Script อื่น ๆ
    public string CurrentInput => currentInput;
    public string CurrentTarget => (targets != null && currentIndex < targets.Length) ? targets[currentIndex] : null;
    public bool IsCompleted => (targets != null && currentIndex >= targets.Length);
}
