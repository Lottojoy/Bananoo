using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text;
using System.Collections; // ← ต้องมี เพราะใช้ IEnumerator/WaitForSeconds

public class TypingManager : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private LessonWordUI ui;  // UI เดิม
    [SerializeField] private bool useWordsAsSegmentsIfTypeWord = true;

    // ---------- SFX (NEW) ----------
    [Header("SFX")]
    [SerializeField] private AudioClip errorClip;   // ใส่เสียง “พิมพ์ผิด” ใน Inspector
    [Range(0f,1f)] [SerializeField] private float errorVolume = 0.8f;
    [Tooltip("สุ่ม pitch ±ค่านี้เพื่อให้เสียงไม่ซ้ำจำเจ")]
    [Range(0f,0.2f)] [SerializeField] private float errorPitchJitter = 0.05f;
    [Tooltip("คูลดาวน์กันเสียงรัวเมื่อกดคีย์เร็วๆ")]
    [SerializeField] private float errorCooldown = 0.05f;

    private AudioSource sfxSource; // ใช้เล่น SFX สั้นๆ
    private float lastErrorTime = -999f;
    // -------------------------------

    private Lesson lesson;
    private string[] segments;
    private int segIdx = 0;
    private string current;
    private int charIdx = 0;

    private int correctTotal = 0;   // นับ “ถูก” ทั้งบท
    private int totalChars = 0;     // จำนวนตัวทั้งหมด (ตัด space แล้ว)

    private float startTime = 0f;
    private bool started = false;
    private bool finished = false;
    private bool segmentFinished = false;

    // --- สถานะระบายสี + ธงแก้ไข ---
    private enum S { Untyped, Correct, Wrong, Corrected }
    private S[] states;           // ต่ออักขระใน segment
    private bool[] edited;        // เคยโดน backspace/แก้ไขไหม (ให้ถูกครั้งถัดไป = เหลือง)

    // นับว่า “ถูก” เมื่อเป็นสองสถานะนี้
    private bool IsCountedCorrect(S st) => (st == S.Correct || st == S.Corrected);

    private const string GREEN  = "#2ECC71";
    private const string RED    = "#E74C3C";
    private const string YELLOW = "#F1C40F";
    private const string CURSOR = "#FFFFFF";

    void Awake()
    {
        if (!ui) ui = FindObjectOfType<LessonWordUI>(true);

        // ---------- เตรียม AudioSource สำหรับ SFX ----------
        // ถ้าบน GameObject นี้ยังไม่มี AudioSource แยกสำหรับ SFX ให้สร้างเพิ่ม (ไม่ยุ่งกับระบบ BGM)
        sfxSource = GetComponent<AudioSource>();
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }
        sfxSource.playOnAwake = false;
        sfxSource.loop = false;
        sfxSource.spatialBlend = 0f; // 2D
    }

    void Start()
    {
        lesson = LessonContext.SelectedLesson;
        if (lesson == null && LessonContext.SelectedLessonID > 0)
            lesson = DataManager.Instance.GetLessonByID(LessonContext.SelectedLessonID);

        if (lesson == null || ui == null)
        {
            Debug.LogError("[TypingManager] Missing refs / lesson.");
            ui?.SetLessonText("No lesson loaded.");
            enabled = false; 
            return;
        }

        // 1) แตกเป็นบล็อค
        if (lesson.Type == LessonType.Character)
        {
            segments = (string[])(lesson.GetType()
                        .GetField("characters", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        .GetValue(lesson));
        }
        else if (useWordsAsSegmentsIfTypeWord)
        {
            segments = (string[])(lesson.GetType()
                        .GetField("words", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        .GetValue(lesson));
        }
        else
        {
            segments = new string[] { lesson.GetText() ?? "" };
        }

        if (segments == null || segments.Length == 0)
            segments = new string[] { lesson.GetText() ?? "" };

        // 2) นับตัวรวม (ตัด space)
        totalChars = 0;
        foreach (var s in segments) totalChars += CountCharsNoSpace(s);

        // 3) โหลดบล็อคแรก
        LoadSegment(0);

        started = false; 
        finished = false;
        ui.UpdateTypingProgress(0, totalChars);
        ui.UpdateLiveStats(0f, 0f, 0f);
    }

    void Update()
    {
        if (finished || current == null) return;

        // Space → ข้ามเฉพาะเมื่อพิมพ์ครบแล้ว
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (segmentFinished) GoNextSegment();
            return;
        }

        // Backspace
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (!started) { started = true; startTime = Time.time; }
            HandleBackspace();
            return;
        }

        // ตัวอักษรจริง (ไม่รับ space)
        if (Input.anyKeyDown && Input.inputString.Length > 0)
        {
            foreach (char c in Input.inputString)
            {
                if (char.IsControl(c) || c == ' ') continue;
                if (!started) { started = true; startTime = Time.time; }
                HandleChar(c);
            }
        }

        // สถิติ live
        if (started && !segmentFinished && !finished)
        {
            float t = Mathf.Max(Time.time - startTime, 0f);
            float wpm = (t > 0f) ? (correctTotal / 5f) / (t / 60f) : 0f;
            float acc = totalChars > 0 ? (float)correctTotal / totalChars : 0f;
            ui.UpdateLiveStats(wpm, acc, t);
        }
    }

    // -------- Core --------
    void LoadSegment(int index)
    {
        if (index >= segments.Length) { FinishAll(); return; }

        current = (segments[index] ?? "").Replace(" ", ""); // ไม่ให้พิมพ์ space
        charIdx = 0;
        segmentFinished = false;

        states = new S[current.Length];
        edited = new bool[current.Length];   // reset ธงแก้ไข

        RenderSegment();
        ui.UpdateTypingProgress(CalcTypedGlobalCount(), totalChars);
    }

    void HandleChar(char c)
    {
        if (segmentFinished) return;
        if (charIdx >= current.Length) { segmentFinished = true; return; }

        char expected = current[charIdx];
        S prev = states[charIdx];

        if (c == expected)
        {
            // ถูกครั้งแรก = เขียว / ถูกหลังเคยแก้ = เหลือง
            S now = edited[charIdx] ? S.Corrected : S.Correct;
            if (!IsCountedCorrect(prev)) correctTotal++; // จากไม่ถูก -> ถูก (+1)
            states[charIdx] = now;
        }
        else
        {
            // ผิด = แดง; ถ้าก่อนหน้านับถูกอยู่ ให้ -1
            if (IsCountedCorrect(prev)) correctTotal--;
            states[charIdx] = S.Wrong;

            // 🔊 เล่นเสียงผิด
            PlayErrorSfx();

            ui.ShowErrorEffect();
        }

        charIdx++;
        if (charIdx >= current.Length) segmentFinished = true;

        RenderSegment();
        ui.UpdateTypingProgress(CalcTypedGlobalCount(), totalChars);
    }

    void HandleBackspace()
    {
        if (charIdx <= 0) return;
        int i = charIdx - 1;

        // ถ้าเคยนับถูก (เขียว/เหลือง) อยู่ ให้ -1 ออกก่อน
        if (IsCountedCorrect(states[i])) correctTotal--;

        edited[i] = true;      // มาร์คว่า “ตำแหน่งนี้เคยแก้”
        states[i] = S.Untyped; // ล้างสี (ไม่มีสี)

        charIdx = i;
        segmentFinished = false;
        RenderSegment();
        ui.UpdateTypingProgress(CalcTypedGlobalCount(), totalChars);
    }

    void GoNextSegment()
    {
        segIdx++;
        if (segIdx >= segments.Length) FinishAll();
        else                           LoadSegment(segIdx);
    }

    void FinishAll()
    {
        if (finished) return;
        finished = true;

        float used  = (started ? Mathf.Max(Time.time - startTime, 0.0001f) : 0f);
        float acc01 = totalChars > 0 ? (float)correctTotal / totalChars : 0f;
        float wpm   = (used > 0f) ? (correctTotal / 5f) / (used / 60f) : 0f;

        ui.ShowResult(wpm, acc01, used);

        // ✨ คำนวณจำนวนตัวอักษร/จำนวนคำของบทเรียนนี้ (ใช้ out)
        int playedChars, playedWords;
        CalcLessonCounts_Out(
            lesson, segments,
            /*stripSpaces:*/ true,
            /*usingWordsSegments:*/ useWordsAsSegmentsIfTypeWord,
            out playedChars, out playedWords
        );

        var pack = new ScoreData
        {
            LessonID    = lesson ? lesson.LessonID : 0,
            LessonTitle = lesson ? lesson.name     : "Unknown",
            WPM         = wpm,
            ACC         = acc01 * 100f,
            TimeUsed    = used,

            PlayedCharCount = playedChars,
            PlayedWordCount = playedWords
        };
        var gdm = GetGDM();
        if (gdm != null) gdm.SetScore(pack);

        var pm = GetPM();
        if (pm != null && lesson != null) pm.OnLessonCleared(lesson.LessonID);

        StartCoroutine(GoResultAfter(1.5f));
    }

    // -------- SFX helper (NEW) --------
    private void PlayErrorSfx()
    {
        if (!errorClip || !sfxSource) return;

        // กันเสียงถี่เกินเวลาพิมพ์เร็วๆ
        if (Time.time - lastErrorTime < errorCooldown) return;
        lastErrorTime = Time.time;

        float basePitch = 1f;
        float jitter = Random.Range(-errorPitchJitter, errorPitchJitter);
        sfxSource.pitch = basePitch + jitter;
        sfxSource.PlayOneShot(errorClip, Mathf.Clamp01(errorVolume));
    }
    // -----------------------------------

    // -------- Render --------
    void RenderSegment()
    {
        var sb = new StringBuilder(current.Length * 20);
        for (int i = 0; i < current.Length; i++)
        {
            if (i == charIdx && !segmentFinished) sb.Append($"<color={CURSOR}>|</color>");

            char ch = current[i];
            switch (states[i])
            {
                case S.Correct:   sb.Append($"<color={GREEN}>{ch}</color>");  break;
                case S.Wrong:     sb.Append($"<color={RED}>{ch}</color>");    break;
                case S.Corrected: sb.Append($"<color={YELLOW}>{ch}</color>"); break;
                default:          sb.Append(ch);                               break;
            }
        }
        if (segmentFinished) sb.Append($"  <size=70%><color=#999999>(กดSpacebar เพื่อไปต่อ)</color></size>");
        ui.SetLessonRichText(sb.ToString());
    }

    // -------- Utils --------
    int CountCharsNoSpace(string s)
        => string.IsNullOrEmpty(s) ? 0 : s.Replace(" ", "").Length;

    int CalcTypedGlobalCount()
    {
        int sum = 0;
        for (int i = 0; i < segIdx; i++) sum += CountCharsNoSpace(segments[i]);
        sum += Mathf.Min(charIdx, current.Length);
        return sum;
    }

    IEnumerator GoResultAfter(float sec)
    {
        yield return new WaitForSeconds(sec);
        SceneLoader.FadeToScene("ResultScene");
    }

    private GameDataManager GetGDM()
    {
        var gdm = GameDataManager.Instance ?? FindObjectOfType<GameDataManager>(true);
        if (gdm == null)
        {
            var go = new GameObject("GameDataManager(Auto)");
            gdm = go.AddComponent<GameDataManager>();
            Debug.LogWarning("[TypingManager] Auto-created GameDataManager.");
        }
        return gdm;
    }

    private PlayerManager GetPM()
    {
        var pm = PlayerManager.Instance ?? FindObjectOfType<PlayerManager>(true);
        if (pm == null) Debug.LogWarning("[TypingManager] PlayerManager not found.");
        return pm;
    }

    // ===== helper: ใช้ out เพื่อกัน C# เก่า/ไม่รองรับ tuple =====
    private static void CalcLessonCounts_Out(
        Lesson lesson,
        string[] segments,
        bool stripSpaces,
        bool usingWordsSegments,
        out int charCount,
        out int wordCount)
    {
        charCount = 0;
        wordCount = 0;
        if (lesson == null) return;

        if (lesson.Type == LessonType.Character)
        {
            if (segments != null && segments.Length > 0)
            {
                foreach (var s in segments)
                    charCount += string.IsNullOrEmpty(s) ? 0 :
                                 (stripSpaces ? s.Replace(" ", "").Length : s.Length);
            }
            else
            {
                string text = lesson.GetText() ?? "";
                charCount = stripSpaces ? text.Replace(" ", "").Length : text.Length;
            }
            wordCount = 0;
            return;
        }

        // Word/Audio → ลองดึง words[]
        var wordsField = (string[])lesson.GetType()
            .GetField("words", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .GetValue(lesson);

        if (wordsField != null && wordsField.Length > 0)
        {
            foreach (var w in wordsField)
                charCount += string.IsNullOrWhiteSpace(w) ? 0 :
                             (stripSpaces ? w.Replace(" ", "").Length : w.Length);
            wordCount = wordsField.Length;
            return;
        }

        // fallback: จาก GetText()
        string fallback = lesson.GetText() ?? "";
        charCount = stripSpaces ? fallback.Replace(" ", "").Length : fallback.Length;
        if (!string.IsNullOrWhiteSpace(fallback))
            wordCount = fallback.Split(' ', System.StringSplitOptions.RemoveEmptyEntries).Length;
    }
}
