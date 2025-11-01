using UnityEngine;
using TMPro;

public class StarScorer : MonoBehaviour
{
    [Header("Refs")]
    public Animator anim;          // มี trigger: t1, t2, t3
    public TMP_Text infoText1;     // แสดงเกณฑ์ 1★ (ดิบ)
    public TMP_Text infoText2;     // แสดงเกณฑ์ 2★ (ดิบ)
    public TMP_Text infoText3;     // แสดงเกณฑ์ 3★ (ดิบ)
    public TMP_Text scoreText;     // คะแนนดิบที่ผู้เล่นทำได้

    [Header("Weights (sum ≈ 1.0) for % score")]
    [Range(0f,1f)] public float weightACC  = 0.50f;
    [Range(0f,1f)] public float weightWPM  = 0.35f;
    [Range(0f,1f)] public float weightTime = 0.15f;

    [Header("Targets for % components")]
    public float targetWPM     = 40f;  // WPM ที่ถือว่า 100%
    public float targetTimeSec = 60f;  // เวลาที่ถือว่า 100% (เร็วกว่านี้ก็เพดาน 100%)

    [Header("Star Thresholds (percent, but will convert to raw)")]
    public float oneStarPct    = 20f;
    public float twoStarsPct   = 50f;
    public float threeStarsPct = 80f;

    [Header("Scaling by Lesson Length")]
    public bool   scaleByChars    = true;  // true = ใช้จำนวนตัวอักษร, false = ใช้จำนวนคำ
    public float  pointsPerChar   = 1f;    // นน. ต่อหนึ่ง “ตัวอักษร”
    public float  pointsPerWord   = 5f;    // นน. ต่อหนึ่ง “คำ”

    void Start()
    {
        // 0) ปรับ normalize น้ำหนัก
        float sum = Mathf.Max(0.0001f, weightACC + weightWPM + weightTime);
        weightACC  /= sum;  weightWPM  /= sum;  weightTime /= sum;

        // 1) ดึงผลสอบล่าสุด
        var gdm = GameDataManager.Instance ?? FindObjectOfType<GameDataManager>(true);
        if (gdm == null || gdm.ScoreData == null)
        {
            Debug.LogWarning("[StarScorer] No GameDataManager/ScoreData — using zero.");
            Apply(0f, 0f, 0f, null);
            return;
        }

        var data   = gdm.ScoreData;                  // ACC (0..100), WPM, TimeUsed
        var lesson = GetLessonByID(data.LessonID);   // ใช้คำนวณความยาวบท

        Apply(data.ACC, data.WPM, data.TimeUsed, lesson);
    }

    // ===== CORE =====
    private void Apply(float accPercent, float wpm, float timeSec, Lesson lesson)
    {
        // 2) หาความยาวบทเรียน
        int lenChars = 0, lenWords = 0;
        CalcLessonLength(lesson, out lenChars, out lenWords);

        // 3) คิด “คะแนนดิบสูงสุด” ของด่านนี้
        float maxRaw = scaleByChars ? (lenChars * pointsPerChar)
                                    : (lenWords * pointsPerWord);
        // กันกรณีว่างเปล่า
        if (maxRaw <= 0f) maxRaw = scaleByChars ? 1f * pointsPerChar : 1f * pointsPerWord;

        // 4) คิด “คะแนน % รวม” เหมือนเดิม (0..100) จากผลจริง
        float accPct  = Mathf.Clamp(accPercent, 0f, 100f);
        float wpmPct  = Mathf.Clamp01(wpm / Mathf.Max(1f, targetWPM)) * 100f;
        float timePct = (timeSec <= 0f) ? 100f : Mathf.Clamp01(targetTimeSec / timeSec) * 100f;

        float totalPct = (accPct  * weightACC) +
                         (wpmPct  * weightWPM) +
                         (timePct * weightTime);

        totalPct = Mathf.Clamp(totalPct, 0f, 100f);

        // 5) แปลงคะแนน % → “คะแนนดิบที่ทำได้” ตามความยาวด่าน
        float totalRaw = Mathf.Round(totalPct / 100f * maxRaw);

        // 6) แปลงเกณฑ์ดาว % → “เกณฑ์ดิบ” ของด่านนี้
        float t1 = Mathf.Ceil(oneStarPct    / 100f * maxRaw);
        float t2 = Mathf.Ceil(twoStarsPct   / 100f * maxRaw);
        float t3 = Mathf.Ceil(threeStarsPct / 100f * maxRaw);

        // 7) ยิงอนิเมชันดาวตามคะแนนดิบจริง
        if (totalRaw >= t3)      anim?.SetTrigger("t3");
        else if (totalRaw >= t2) anim?.SetTrigger("t2");
        else if (totalRaw >= t1) anim?.SetTrigger("t1");
        // ตํ่ากว่า t1 = ไม่ได้ดาว

        // 8) อัปเดตข้อความ (ดิบล้วน)
        if (scoreText) scoreText.text = $"Score: {totalRaw:F0} / {maxRaw:F0}";

        if (infoText1) infoText1.text = BuildLine("1★", t1, totalRaw);
        if (infoText2) infoText2.text = BuildLine("2★", t2, totalRaw);
        if (infoText3) infoText3.text = BuildLine("3★", t3, totalRaw);
        Debug.Log($"lenChars={lenChars}, lenWords={lenWords}, maxRaw={maxRaw}");
Debug.Log($"t1={t1}, t2={t2}, t3={t3}, totalPct={totalPct}, totalRaw={totalRaw}");
    }

    private string BuildLine(string label, float thresholdRaw, float totalRaw)
    {
        // ข้อความแบบ “คะแนนนี้ ≥ X จะได้” + ถ้ายังไม่ถึง แสดงว่ายังขาดเท่าไหร่ (ดิบ)
        float delta = Mathf.Max(0f, thresholdRaw - totalRaw);
        return delta <= 0f
            ? $"{thresholdRaw:F0}"
            : $"{thresholdRaw:F0}";
    }

    // ===== Length helpers =====
    private void CalcLessonLength(Lesson lesson, out int charLen, out int wordLen)
    {
        // ถ้าไม่มี lesson ให้ใช้ ScoreData.LessonTitle หรือ GameData ตามที่มี
        string text = (lesson != null) ? lesson.GetText() : "";
        text = text ?? "";

        // นับตัวอักษร (ไม่เอาช่องว่าง)
        charLen = text.Replace(" ", "").Length;

        // นับคำแบบหยาบ ๆ (แยกด้วย space)
        wordLen = 0;
        if (!string.IsNullOrWhiteSpace(text))
        {
            var parts = text.Split(' ');
            foreach (var p in parts) if (!string.IsNullOrWhiteSpace(p)) wordLen++;
        }
    }

    private Lesson GetLessonByID(int lessonID)
    {
        var dm = DataManager.Instance ?? FindObjectOfType<DataManager>(true);
        return (dm != null && lessonID > 0) ? dm.GetLessonByID(lessonID) : null;
    }
}
