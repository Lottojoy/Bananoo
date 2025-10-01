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
        {
            targets = currentLesson.words;
        }
        else if (currentLesson.type == LessonType.Character && currentLesson.characters.Length > 0)
        {
            targets = currentLesson.characters;
        }

        // บังคับให้ target ต้องมีเว้นวรรคต่อท้าย
        for (int i = 0; i < targets.Length; i++)
        {
            if (!targets[i].EndsWith(" "))
                targets[i] += " ";
        }

        if (targets != null && targets.Length > 0)
        {
            UpdateTypingText();
        }
    }

    void Update()
    {
        if (targets == null || targets.Length == 0) return;

        // Backspace
        if (Input.GetKeyDown(KeyCode.Backspace) && currentInput.Length > 0)
        {
            int lastIndex = currentInput.Length - 1;
            correctedIndexes.Add(lastIndex); // mark ว่าเคยถูกแล้วแก้ไข
            currentInput = currentInput.Substring(0, lastIndex);
            UpdateTypingText();
        }

        // ตัวอักษรทั่วไป
        foreach (char c in Input.inputString)
        {
            if (c == '\b' || c == '\n' || c == '\r') continue;

            currentInput += c;
            UpdateTypingText();

            // ตรวจว่า target ถูกครบแล้ว (รวมเว้นวรรค)
            if (currentInput == targets[currentIndex])
            {
                Debug.Log("Completed: " + targets[currentIndex]);

                currentIndex++;
                currentInput = "";
                correctedIndexes.Clear();

                if (currentIndex >= targets.Length)
                {
                    Debug.Log("Lesson Completed!");
                    SceneLoader.LoadScene("ResultScene");
                }
                else
                {
                    UpdateTypingText();
                }
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
        string colorTag = "#AAAAAA"; // default เทา

        if (i < currentInput.Length)
        {
            if (currentInput[i] == target[i])
                colorTag = correctedIndexes.Contains(i) ? "yellow" : "green";
            else
                colorTag = "red";
        }

        // แทรก cursor | ที่ตำแหน่ง currentInput.Length
        if (i == currentInput.Length)
            display += $"<color={colorTag}>{target[i]}|</color>";
        else
            display += $"<color={colorTag}>{target[i]}</color>";
    }

    // กรณี cursor อยู่หลังตัวสุดท้าย
    if (currentInput.Length >= target.Length)
        display += "|";

    typingText.text = display;
    }

    // Property สำหรับ UI หรือ Script อื่น ๆ
    public string CurrentInput => currentInput;
    public string CurrentTarget => (targets != null && currentIndex < targets.Length) ? targets[currentIndex] : null;
    public bool IsCompleted => (targets != null && currentIndex >= targets.Length);
}
