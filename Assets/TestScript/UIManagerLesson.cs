using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class UIManagerLesson : MonoBehaviour
{
     [Header("TMP UI Elements")]
    
    public TMP_Text statusText;   // สถานะ เช่น "ถูกต้อง!" หรือ "เสร็จสิ้น"

    private TypingManager typingManager;

    void Start()
    {
        // หา TypingManager ใน Scene
        typingManager = FindObjectOfType<TypingManager>();
    }

    void Update()
    {
        if (typingManager == null) return;

       

        if (typingManager.IsCompleted)
        {
            statusText.text = "✅ Lesson Completed!";
        }
        else
        {
            statusText.text = "";
        }
    }
}
