using UnityEngine;
using TMPro;

public class LessonWordUI : MonoBehaviour
{
    public TMP_Text lessonText;
    public TMP_Text progressText;
    public TMP_Text wpmText;
    public TMP_Text accText;
    public TMP_Text timeText;

    public void SetLessonText(string text)
    {
        lessonText.text = text;
    }

    public void UpdateTypingProgress(int index)
    {
        progressText.text = $"Progress: {index}";
        // เพิ่ม cursor หรือ highlight ได้
    }

    public void ShowErrorEffect()
    {
        // กระพริบสีแดงหรือเสียงผิด
    }

    public void ShowResult(float wpm, float acc, float time)
    {
        wpmText.text = $"WPM: {wpm:F1}";
        accText.text = $"ACC: {acc * 100f:F1}%";
        timeText.text = $"Time: {time:F1}s";
    }
}
