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

    [Header("Pop Effect")]
    [SerializeField] float popUpScale = 1.12f;   // ใหญ่แค่ไหนตอนเด้ง
    [SerializeField] float upTime     = 0.06f;   // เวลาเด้งขึ้น
    [SerializeField] float downTime   = 0.09f;   // เวลาเด้งกลับ
    [SerializeField] float maxClamp   = 1.2f;    // กันบวมเกิน

    Vector3 baseScale = Vector3.one;
    Coroutine popCo;
    Color baseColor;

    void Awake() {
        if (lessonText)
        {
            baseScale = lessonText.rectTransform.localScale;
            baseColor = lessonText.color;
        }
    }

    public void SetLessonText(string text) {
        if (lessonText) lessonText.text = text ?? "";
    }
    public void SetLessonRichText(string rich) {
        if (lessonText) lessonText.text = rich ?? "";
    }

    public void UpdateTypingProgress(int index, int total) {
        if (!progressText) return;
        index = Mathf.Clamp(index, 0, Mathf.Max(total, 1));
        float pct = total > 0 ? (index * 100f / total) : 0f;
        progressText.text = $"Progress: {index}/{total} ({pct:F1}%)";
    }
    public void UpdateLiveStats(float wpm, float acc, float elapsed) {
        if (wpmText)  wpmText.text  = $"WPM: {wpm:F1}";
        if (accText)  accText.text  = $"ACC: {acc * 100f:F1}%";
        if (timeText) timeText.text = $"Time: {elapsed:F1}s";
    }

    // =============== FIX POP BLOAT ===============
    public void ShowErrorEffect()
    {
        if (!lessonText) return;

        // หยุดของเก่าก่อน แล้วรีเซ็ตฐานทุกครั้ง
        if (popCo != null) StopCoroutine(popCo);
        lessonText.rectTransform.localScale = baseScale;
        lessonText.color = baseColor;

        popCo = StartCoroutine(PopAndFlash());
    }

    IEnumerator PopAndFlash()
    {
        var rt = lessonText.rectTransform;

        // flash สีแดงสั้นๆ (ไม่สะสม)
        var orig = baseColor;
        lessonText.color = new Color(1f, 0.35f, 0.35f);

        // เด้งขึ้น
        float t = 0f;
        while (t < upTime)
        {
            t += Time.unscaledDeltaTime; // ใช้เวลาแบบไม่โดน timeScale
            float k = Mathf.SmoothStep(0f, 1f, t / upTime);
            float s = Mathf.Lerp(1f, popUpScale, k);
            s = Mathf.Min(s, maxClamp);
            rt.localScale = baseScale * s;
            yield return null;
        }

        // เด้งกลับ
        t = 0f;
        while (t < downTime)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.SmoothStep(0f, 1f, t / downTime);
            float s = Mathf.Lerp(popUpScale, 1f, k);
            rt.localScale = baseScale * s;
            yield return null;
        }

        // รีเซ็ตกลับฐานทุกครั้งกันบวม
        rt.localScale = baseScale;
        lessonText.color = orig;
        popCo = null;
    }

    public void ShowResult(float wpm, float acc, float time)
    {
        if (wpmText)  wpmText.text  = $"WPM: {wpm:F1}";
        if (accText)  accText.text  = $"ACC: {acc * 100f:F1}%";
        if (timeText) timeText.text = $"Time: {time:F1}s";
    }

    // สำหรับ Key Hint (เดิม)
    [Header("Key Hint")]
    public UnityEngine.UI.RawImage keyHintImage;
    public void ShowKeyHint(Texture2D tex) { if (keyHintImage) { keyHintImage.texture = tex; keyHintImage.enabled = (tex != null); } }
    public void HideKeyHint() { if (keyHintImage) keyHintImage.enabled = false; }
}
