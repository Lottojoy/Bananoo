using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TypingManagerKey : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private LessonKeyUI ui;          // ใส่ LessonKeyUI ของฉากนี้
    [SerializeField] private KeyHintMap keyHintMap;   // SO ที่แมประหว่างตัวอักษร -> รูปคีย์ (Texture2D)

    [Header("Lesson / Segments")]
    [SerializeField] private bool useWordsAsSegmentsIfWordType = true; // ถ้าเป็น Word ใช้ words[] เป็น segment
    [SerializeField] private bool stripSpacesInSegment = true;         // ตัดช่องว่างออกจากข้อความที่ต้องพิมพ์

    [Header("Result Flow (optional)")]
    [SerializeField] private bool  autoGoResult = true; // จบแล้วไป ResultScene?
    [SerializeField] private float resultDelay  = 1.5f; // หน่วงก่อนเปลี่ยนซีน

    // ---- internal state ----
    private Lesson lesson;
    private string[] segments;     // บล็อกทั้งหมด (characters[] หรือ words[])
    private int    segIdx = 0;     // index ของบล็อกปัจจุบัน
    private string current;        // ข้อความของบล็อก (อาจตัด space แล้ว)
    private int    charIdx = 0;    // cursor ภายในบล็อก

    private int   correctTotal = 0; // ตัวถูกสะสมทั้งบท
    private int   totalChars   = 0; // จำนวนตัวทั้งหมด (ตามโหมด stripSpacesInSegment)
    private float startTime    = 0f;

    private bool started         = false;
    private bool finished        = false;
    private bool segmentFinished = false;

    // สถานะการระบายสี
    private enum S { Untyped, Correct, Wrong, Corrected }
    private S[]    states;   // ต่ออักขระใน segment
    private bool[] edited;   // ✅ เคยถูก Backspace/แก้ไขไหม

    // นับว่า “ถูก” เมื่อเป็นสองสถานะนี้
    private bool IsCountedCorrect(S st) => (st == S.Correct || st == S.Corrected);

    private const string GREEN  = "#2ECC71";
    private const string RED    = "#E74C3C";
    private const string YELLOW = "#F1C40F";
    private const string CURSOR = "#FFFFFF";

    void Awake()
    {
        if (!ui) ui = FindObjectOfType<LessonKeyUI>(includeInactive: true);
    }

    void Start()
    {
        // 1) รับบทเรียนจาก context
        lesson = LessonContext.SelectedLesson;
        if (lesson == null && LessonContext.SelectedLessonID > 0)
            lesson = DataManager.Instance.GetLessonByID(LessonContext.SelectedLessonID);

        if (lesson == null || ui == null)
        {
            Debug.LogError("[TypingManagerKey] Missing refs or lesson.");
            ui?.SetLessonText("No lesson loaded.");
            enabled = false; 
            return;
        }

        // 2) แตกเป็นเซกเมนต์
        BuildSegments();

        // 3) นับตัวอักษรรวม
        totalChars = 0;
        foreach (var s in segments) totalChars += CountChars(s, stripSpacesInSegment);

        // 4) โหลดเซกเมนต์แรก
        segIdx = 0;
        LoadSegment(segIdx);

        // 5) เคลียร์ค่าเริ่มต้น
        started = false; 
        finished = false;
        ui.UpdateTypingProgress(0, totalChars);
        ui.UpdateLiveStats(0f, 0f, 0f);
    }

    void Update()
    {
        if (finished || current == null) return;

        // Space = ข้ามไปบล็อกถัดไป (เฉพาะเมื่อพิมพ์ครบแล้ว)
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

        // ตัวอักษรจริง (ข้าม control และช่องว่างถ้าตั้ง strip)
        if (Input.anyKeyDown && Input.inputString.Length > 0)
        {
            foreach (char raw in Input.inputString)
            {
                char c = raw;
                if (char.IsControl(c)) continue;
                if (stripSpacesInSegment && c == ' ') continue;

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

    // ================= CORE =================

    void BuildSegments()
    {
        // Character -> ใช้ characters[]
        if (lesson.Type == LessonType.Character)
        {
            segments = (string[])(lesson.GetType()
                .GetField("characters", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .GetValue(lesson));
        }
        // Word + อนุญาต -> ใช้ words[]
        else if (useWordsAsSegmentsIfWordType)
        {
            segments = (string[])(lesson.GetType()
                .GetField("words", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .GetValue(lesson));
        }
        // ไม่งั้นทั้งบทเป็นบล็อกเดียว
        else
        {
            segments = new string[] { lesson.GetText() ?? "" };
        }

        if (segments == null || segments.Length == 0)
            segments = new string[] { lesson.GetText() ?? "" };
    }

    void LoadSegment(int index)
    {
        if (index >= segments.Length)
        {
            FinishAll();
            return;
        }

        current = segments[index] ?? "";
        if (stripSpacesInSegment) current = current.Replace(" ", "");

        charIdx = 0;
        segmentFinished = false;

        states = new S[current.Length];
        edited = new bool[current.Length];   // ✅ reset ธงแก้ไขของ segment นี้

        RenderSegment();
        ui.UpdateTypingProgress(CalcTypedGlobalCount(), totalChars);
        UpdateKeyHint();
    }

    void HandleChar(char c)
    {
        if (segmentFinished) return;
        if (charIdx >= current.Length) { segmentFinished = true; UpdateKeyHint(); return; }

        char expected = current[charIdx];
        S prev = states[charIdx];

        if (c == expected)
        {
            // ✅ ถูกครั้งแรก = เขียว, ถูกหลังเคยแก้ = เหลือง
            S now = edited[charIdx] ? S.Corrected : S.Correct;

            if (!IsCountedCorrect(prev)) correctTotal++; // จากไม่ถูก -> ถูก (+1)
            states[charIdx] = now;
        }
        else
        {
            // ผิด = แดง; ถ้าก่อนหน้านับเป็นถูกอยู่ ให้ -1
            if (IsCountedCorrect(prev)) correctTotal--;
            states[charIdx] = S.Wrong;
            ui.ShowErrorEffect();
        }

        charIdx++;
        if (charIdx >= current.Length) segmentFinished = true;

        RenderSegment();
        ui.UpdateTypingProgress(CalcTypedGlobalCount(), totalChars);
        UpdateKeyHint();
    }

    void HandleBackspace()
    {
        if (charIdx <= 0) return;
        int i = charIdx - 1;

        // ถ้าเคยนับถูก (เขียว/เหลือง) อยู่ ให้ -1 ออกก่อน
        if (IsCountedCorrect(states[i])) correctTotal--;

        edited[i] = true;      // ✅ มาร์คว่า “ตำแหน่งนี้เคยแก้”
        states[i] = S.Untyped; // ✅ ล้างสีตามสเปค (ไม่มีสี)

        charIdx = i;
        segmentFinished = false;

        RenderSegment();
        ui.UpdateTypingProgress(CalcTypedGlobalCount(), totalChars);
        UpdateKeyHint();
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
        ui.HideKeyHint();

        // —— ส่งผลลัพธ์ -> GameDataManager (ถ้าใช้) ——
        var pack = new ScoreData
        {
            LessonID    = lesson ? lesson.LessonID : 0,
            LessonTitle = lesson ? lesson.name     : "Unknown",
            WPM         = wpm,
            ACC         = acc01 * 100f,
            TimeUsed    = used,
            FinalScore  = Mathf.Round((wpm * 10f) * acc01)
        };
        var gdm = GetGDM();
        if (gdm != null) gdm.SetScore(pack);

        var pm = GetPM();
        if (pm != null && lesson != null) pm.OnLessonCleared(lesson.LessonID);

        if (autoGoResult) StartCoroutine(GoResultAfter(resultDelay));
    }

    // ================= RENDER =================
    void RenderSegment()
    {
        var sb = new StringBuilder(current.Length * 20);

        for (int i = 0; i < current.Length; i++)
        {
            if (i == charIdx && !segmentFinished) sb.Append($"<color={CURSOR}>|</color>");

            char ch = current[i];
            switch (states[i])
            {
                case S.Correct:   sb.Append($"<color={GREEN}>{ch}</color>");  break; // เขียว
                case S.Wrong:     sb.Append($"<color={RED}>{ch}</color>");    break; // แดง
                case S.Corrected: sb.Append($"<color={YELLOW}>{ch}</color>"); break; // เหลือง (ถูกหลังแก้)
                default:          sb.Append(ch);                               break; // ไม่มีสี
            }
        }

        if (segmentFinished) sb.Append("  <size=70%><color=#999999>(Space → ต่อ)</color></size>");
        ui.SetLessonRichText(sb.ToString());
    }

    // ================= KEY HINT (IMAGE) =================
    void UpdateKeyHint()
    {
        if (!ui) return;

        // จบเซกเมนต์หรือหมดตัวอักษร -> ซ่อน
        if (segmentFinished || charIdx >= current.Length)
        {
            ui.HideKeyHint();
            return;
        }

        char next = current[charIdx];
        Texture2D tex = null;
        bool found = false;

        if (keyHintMap)
        {
            // ปุ่มพิเศษ
            if      (next == ' '  && keyHintMap.TryGetSpecial("space", out var t1)) { tex = t1; found = true; }
            else if (next == '\n' && keyHintMap.TryGetSpecial("enter", out var t2)) { tex = t2; found = true; }
            else if (next == '\t' && keyHintMap.TryGetSpecial("tab",   out var t3)) { tex = t3; found = true; }
            // ตัวปกติ
            else if (keyHintMap.TryGet(next, out var t4)) { tex = t4; found = true; }
        }

        if (found) ui.ShowKeyHint(tex);
        else       ui.HideKeyHint();
    }

    // ================= UTILS =================
    int CountChars(string s, bool stripSpace)
        => string.IsNullOrEmpty(s) ? 0 : (stripSpace ? s.Replace(" ", "").Length : s.Length);

    int CalcTypedGlobalCount()
    {
        int sum = 0;
        for (int i = 0; i < segIdx; i++) sum += CountChars(segments[i], stripSpacesInSegment);
        sum += Mathf.Min(charIdx, current.Length);
        return sum;
    }

    System.Collections.IEnumerator GoResultAfter(float sec)
    {
        yield return new WaitForSeconds(sec);
        SceneManager.LoadScene("ResultScene");
    }

    private GameDataManager GetGDM()
    {
        var gdm = GameDataManager.Instance ?? FindObjectOfType<GameDataManager>(true);
        if (gdm == null)
        {
            var go = new GameObject("GameDataManager(Auto)");
            gdm = go.AddComponent<GameDataManager>();
            Debug.LogWarning("[TypingManagerKey] Auto-created GameDataManager.");
        }
        return gdm;
    }

    private PlayerManager GetPM()
    {
        var pm = PlayerManager.Instance ?? FindObjectOfType<PlayerManager>(true);
        if (pm == null) Debug.LogWarning("[TypingManagerKey] PlayerManager not found.");
        return pm;
    }
}
