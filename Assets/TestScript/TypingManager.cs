using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypingManager : MonoBehaviour
{
   public Lesson currentLesson;  // อ้างถึง Lesson ใน Scene

    private string[] targets;     // target list
    private int currentIndex = 0; // กำลังพิมพ์ตัวไหนอยู่
    private string currentInput = "";

    void Start()
    {
        // โหลด target ตาม type
        if (currentLesson.type == LessonType.Word && currentLesson.words.Length > 0)
        {
            targets = currentLesson.words;
        }
        else if (currentLesson.type == LessonType.Character && currentLesson.characters.Length > 0)
        {
            targets = currentLesson.characters;
        }

        if (targets != null && targets.Length > 0)
        {
            Debug.Log("First Target: " + targets[0]);
        }
    }

    void Update()
    {
        if (targets == null || targets.Length == 0) return;

        foreach (char c in Input.inputString)
        {
            currentInput += c;

            if (currentInput == targets[currentIndex])
            {
                Debug.Log("Completed: " + targets[currentIndex]);

                currentIndex++;
                currentInput = "";

                if (currentIndex >= targets.Length)
                {
                    Debug.Log("Lesson Completed!");
                    SceneLoader.LoadScene("ResultScene");
                }
                else
                {
                    Debug.Log("Next Target: " + targets[currentIndex]);
                }
            }
        }
    }
}
