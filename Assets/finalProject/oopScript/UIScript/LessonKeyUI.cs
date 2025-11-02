using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LessonKeyUI : MonoBehaviour
{
    [Header("Texts")]
    public TMP_Text lessonText;
    public TMP_Text progressText;
    public TMP_Text wpmText;
    public TMP_Text accText;
    public TMP_Text timeText;

    [Header("Key Hint (Image)")]
    public RawImage keyHintImage;   // ตั้งใน Inspector

    [Header("Error Pop Effect")]
    [SerializeField] float popUpScale = 1.12f;   // ขนาดตอนเด้งขึ้น
    [SerializeField] float upTime     = 0.06f;   // เวลาเด้งขึ้น
    [SerializeField] float downTime   = 0.09f;   // เวลาเด้งกลับ
    [SerializeField] float maxClamp   = 1.2f;    // กันบวมสูงสุด

    Vector3 baseScale = Vector3.one;
    Color baseColor   = Color.white;
    Coroutine popCo;

    void Awake()
    {
        if (lessonText)
        {
            baseScale = lessonText.rectTransform.localScale;
            baseColor = lessonText.color;
        }
    }

    public void SetLessonText(string t)        { if (lessonText) lessonText.text = t ?? ""; }
    public void SetLessonRichText(string rich) { if (lessonText) lessonText.text = rich ?? ""; }

    public void UpdateTypingProgress(int index, int total)
    {
        if (!progressText) return;
        index = Mathf.Clamp(index, 0, Mathf.Max(total, 1));
        float pct = total > 0 ? index * 100f / total : 0f;
        progressText.text = $"Progress: {index}/{total} ({pct:F1}%)";
    }

    public void UpdateLiveStats(float wpm, float acc01, float elapsed)
    {
        if (wpmText)  wpmText.text  = $"WPM: {wpm:F1}";
        if (accText)  accText.text  = $"ACC: {acc01 * 100f:F1}%";
        if (timeText) timeText.text = $"Time: {elapsed:F1}s";
    }

    // ===== FIX: ป้องกันเด้งบวม =====
    public void ShowErrorEffect()
    {
        if (!lessonText) return;

        // หยุดของเก่าก่อน และรีเซ็ตฐานทุกครั้ง
        if (popCo != null) StopCoroutine(popCo);
        var rt = lessonText.rectTransform;
        rt.localScale = baseScale;
        lessonText.color = baseColor;

        popCo = StartCoroutine(PopAndFlash());
    }

    IEnumerator PopAndFlash()
    {
        var rt = lessonText.rectTransform;

        // flash สีแดงสั้น ๆ
        var orig = baseColor;
        lessonText.color = new Color(1f, 0.35f, 0.35f);

        // เด้งขึ้น
        float t = 0f;
        while (t < upTime)
        {
            t += Time.unscaledDeltaTime;
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

        // รีเซ็ตค่าฐาน กันสะสม
        rt.localScale = baseScale;
        lessonText.color = orig;
        popCo = null;
    }

    // ---------- KEY HINT (IMAGE) ----------
    public void ShowKeyHint(Texture2D tex)
{
    if (!keyHintImage) return;
    if (tex == null) { HideKeyHint(); return; }

    keyHintImage.texture = tex;
    keyHintImage.enabled = true;

    // อย่าบังคับขนาดจากไฟล์ภาพ
    // keyHintImage.SetNativeSize();  // ← ลบ/คอมเมนต์บรรทัดนี้

    // ถ้าอยากคงสัดส่วนโดยไม่ล็อกไซส์ ให้เปิด preserveAspect ก็พอ
    
}


    public void HideKeyHint()
    {
        if (!keyHintImage) return;
        keyHintImage.enabled = false;
        keyHintImage.texture = null;
    }

    public void ShowResult(float wpm, float acc01, float time)
    {
        if (wpmText)  wpmText.text  = $"WPM: {wpm:F1}";
        if (accText)  accText.text  = $"ACC: {acc01 * 100f:F1}%";
        if (timeText) timeText.text = $"Time: {time:F1}s";
    }
}
