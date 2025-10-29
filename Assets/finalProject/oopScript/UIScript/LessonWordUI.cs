using UnityEngine;
using TMPro;
using System.Collections;

public class LessonWordUI : MonoBehaviour
{
    [Header("Texts")]
    public TMP_Text lessonText;
    public TMP_Text progressText;
    public TMP_Text wpmText;
    public TMP_Text accText;
    public TMP_Text timeText;

    public void SetLessonText(string text)
    {
        if (lessonText) lessonText.text = text ?? "";
    }

    public void SetLessonRichText(string rich)
    {
        if (lessonText) lessonText.text = rich ?? "";
    }

    public void UpdateTypingProgress(int index, int total)
    {
        if (!progressText) return;
        index = Mathf.Clamp(index, 0, Mathf.Max(total, 1));
        float pct = total > 0 ? (index * 100f / total) : 0f;
        progressText.text = $"Progress: {index}/{total} ({pct:F1}%)";
    }

    // ✅ อัปเดตสถิติระหว่างเล่น
    public void UpdateLiveStats(float wpm, float acc, float elapsed)
    {
        if (wpmText)  wpmText.text  = $"WPM: {wpm:F1}";
        if (accText)  accText.text  = $"ACC: {acc * 100f:F1}%";
        if (timeText) timeText.text = $"Time: {elapsed:F1}s";
    }

    // เอฟเฟกต์ผิด
    public void ShowErrorEffect()
    {
        if (!lessonText) return;
        StopAllCoroutines();
        StartCoroutine(FlashRed());
    }

    private IEnumerator FlashRed()
    {
        Color orig = lessonText.color;
        lessonText.color = new Color(1f, 0.3f, 0.3f);
        yield return new WaitForSeconds(0.08f);
        lessonText.color = orig;
    }

    // แสดงผลตอนจบ
    public void ShowResult(float wpm, float acc, float time)
    {
        if (wpmText)  wpmText.text  = $"WPM: {wpm:F1}";
        if (accText)  accText.text  = $"ACC: {acc * 100f:F1}%";
        if (timeText) timeText.text = $"Time: {time:F1}s";
    }
}
