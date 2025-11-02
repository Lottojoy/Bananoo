using UnityEngine;
using TMPro;

public class StarScorer : MonoBehaviour
{
    [Header("Refs")]
    public Animator anim;
    public TMP_Text infoText1, infoText2, infoText3, scoreText;

    [Header("Weights (sum ≈ 1.0)")]
    [Range(0f,1f)] public float weightACC  = 0.50f;
    [Range(0f,1f)] public float weightWPM  = 0.35f;
    [Range(0f,1f)] public float weightTime = 0.15f;

    [Header("Targets")]
    public float targetWPM = 40f;
    public float targetTimeSec = 60f;

    [Header("Star thresholds (%)")]
    public float oneStarPct = 20f, twoStarsPct = 50f, threeStarsPct = 80f;

    [Header("Scaling")]
    public bool  scaleByChars  = true; // true=ใช้ตัวอักษร, false=ใช้จำนวนคำ
    public float pointsPerChar = 1f;
    public float pointsPerWord = 5f;

    void Start()
    {
        // normalize น้ำหนัก
        float sum = Mathf.Max(0.0001f, weightACC + weightWPM + weightTime);
        weightACC /= sum; weightWPM /= sum; weightTime /= sum;

        var gdm = GameDataManager.Instance ?? FindObjectOfType<GameDataManager>(true);
        if (gdm == null || gdm.ScoreData == null)
        {
            Debug.LogWarning("[StarScorer] No ScoreData.");
            Apply(new ScoreData(), null);
            return;
        }

        var data   = gdm.ScoreData;
        var lesson = GetLessonByID(data.LessonID); // อาจเป็น null ได้ถ้าไม่มี DataManager ใน ResultScene
        Apply(data, lesson);
    }

    private void Apply(ScoreData data, Lesson lesson)
    {
        // 1) อ่าน length จาก ScoreData ก่อน (เชื่อถือได้สุด)
        int usedChars = Mathf.Max(0, data.PlayedCharCount);
        int usedWords = Mathf.Max(0, data.PlayedWordCount);

        // 2) ถ้ายังเป็น 0 ให้ fallback ไปคำนวณจาก Lesson
        if ((usedChars == 0 && scaleByChars) || (usedWords == 0 && !scaleByChars))
        {
            CalcLessonLength(lesson, out int lenChars, out int lenWords);
            if (scaleByChars && usedChars == 0) usedChars = lenChars;
            if (!scaleByChars && usedWords == 0) usedWords = lenWords;
        }

        // 3) ถ้ายัง 0 อยู่ ให้กันตกเป็น 1 (แต่ log เตือนให้รู้ว่าผิด flow)
        if (scaleByChars && usedChars <= 0)
        {
            Debug.LogWarning("[StarScorer] usedChars is 0. Fallback to 1. Make sure managers filled PlayedCharCount.");
            usedChars = 1;
        }
        if (!scaleByChars && usedWords <= 0)
        {
            Debug.LogWarning("[StarScorer] usedWords is 0. Fallback to 1. Make sure managers filled PlayedWordCount.");
            usedWords = 1;
        }

        // 4) สร้าง maxRaw ตามโหมด
        float maxRaw = scaleByChars ? usedChars * pointsPerChar
                                    : usedWords * pointsPerWord;

        // 5) รวมคะแนนเปอร์เซ็นต์
        float accPct  = Mathf.Clamp(data.ACC, 0f, 100f);
        float wpmPct  = Mathf.Clamp01(data.WPM / Mathf.Max(1f, targetWPM)) * 100f;
        float timePct = (data.TimeUsed <= 0f) ? 100f
                     : Mathf.Clamp01(targetTimeSec / data.TimeUsed) * 100f;

        float totalPct = Mathf.Clamp(accPct * weightACC +
                                     wpmPct * weightWPM +
                                     timePct * weightTime, 0f, 100f);

        // 6) แปลงเป็นคะแนนดิบ + เกณฑ์ดาวดิบ
        float totalRaw = Mathf.Round(totalPct / 100f * maxRaw);
        float t1 = Mathf.Ceil(oneStarPct    / 100f * maxRaw);
        float t2 = Mathf.Ceil(twoStarsPct   / 100f * maxRaw);
        float t3 = Mathf.Ceil(threeStarsPct / 100f * maxRaw);

        // 7) แสดงผล + ยิงอนิเมชัน
        if (scoreText) scoreText.text = $"Score: {totalRaw:F0} / {maxRaw:F0}";
        if (infoText1) infoText1.text = $"{t1:F0}";
        if (infoText2) infoText2.text = $"{t2:F0}";
        if (infoText3) infoText3.text = $"{t3:F0}";

        if (totalRaw >= t3)      anim?.SetTrigger("t3");
        else if (totalRaw >= t2) anim?.SetTrigger("t2");
        else if (totalRaw >= t1) anim?.SetTrigger("t1");

        // debug ละเอียดยิบ
        Debug.Log($"[StarScorer] usedChars={usedChars}, usedWords={usedWords}, maxRaw={maxRaw}");
        Debug.Log($"[StarScorer] acc%={accPct:F2}, wpm%={wpmPct:F2}, time%={timePct:F2}, total%={totalPct:F2}, raw={totalRaw:F0}, t1={t1}, t2={t2}, t3={t3}");
    }

    private void CalcLessonLength(Lesson lesson, out int charLen, out int wordLen)
    {
        string text = (lesson != null) ? (lesson.GetText() ?? "") : "";
        charLen = text.Replace(" ", "").Length;

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
