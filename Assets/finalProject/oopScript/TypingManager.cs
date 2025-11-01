using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text;
using System.Collections.Generic;
public class TypingManager : MonoBehaviour
{
   [Header("Refs")]
    [SerializeField] private LessonWordUI ui;  // ใช้ตัวเดิม
    [SerializeField] private bool useWordsAsSegmentsIfTypeWord = true; // ถ้า Lesson.Type=Word ให้ใช้ words[] เป็น segment

    private Lesson lesson;
    private string[] segments;          // บล็อคทั้งหมด (มาจาก characters[] หรือ words[])
    private int segIdx = 0;             // segment ปัจจุบัน
    private string current;             // ข้อความของ segment (ตัด space ออกแล้ว)
    private int charIdx = 0;            // index ใน segment ปัจจุบัน
    private int correctTotal = 0;       // นับถูกทั้งบท
    private int totalChars = 0;         // จำนวนตัวอักษรทั้งหมด (ตัด space ออกแล้ว)

    private float startTime = 0f;
    private bool started = false;
    private bool finished = false;
    private bool segmentFinished = false;

    private enum S { Untyped, Correct, Wrong, Corrected }
    private S[] states;                 // ของ segment ปัจจุบัน

    private const string GREEN="#2ECC71", RED="#E74C3C", YELLOW="#F1C40F", CURSOR="#FFFFFF";

    void Awake()
    {
        if (!ui) ui = FindObjectOfType<LessonWordUI>(true);
    }

    void Start()
    {
        lesson = LessonContext.SelectedLesson;
        if (lesson == null && LessonContext.SelectedLessonID > 0)
            lesson = DataManager.Instance.GetLessonByID(LessonContext.SelectedLessonID);

        if (lesson == null || ui == null)
        {
            Debug.LogError("[TypingManagerSegmented] Missing refs / lesson.");
            ui?.SetLessonText("No lesson loaded.");
            enabled = false; return;
        }

        // 1) สร้าง segments
        if (lesson.Type == LessonType.Character)
            segments = (string[])(lesson.GetType()
                        .GetField("characters", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        .GetValue(lesson));
        else if (useWordsAsSegmentsIfTypeWord)
            segments = (string[])(lesson.GetType()
                        .GetField("words", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        .GetValue(lesson));
        else
            segments = new string[] { lesson.GetText() }; // โหมดเดิม ทั้งบทเป็น segment เดียว

        if (segments == null || segments.Length == 0)
        {
            segments = new string[] { lesson.GetText() ?? "" };
        }

        // 2) คำนวน totalChars (ตัดช่องว่าง)
        totalChars = 0;
        foreach (var s in segments)
            totalChars += CountCharsNoSpace(s);

        // 3) โหลด segment แรก
        LoadSegment(segIdx);

        // status เริ่มต้น
        started = false; finished = false;
        ui.UpdateTypingProgress(0, totalChars);
        ui.UpdateLiveStats(0f, 0f, 0f);
    }

    void Update()
    {
        if (finished || current == null) return;

        // SPACE = ไป segment ถัดไป (เฉพาะกรณีพิมพ์ครบแล้ว)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (segmentFinished)
            {
                GoNextSegment();
            }
            // ถ้ายังไม่จบ segment ให้ไม่ทำอะไร (กันผู้เล่นกดข้าม)
            return;
        }

        // BACKSPACE
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (!started) { started = true; startTime = Time.time; }
            HandleBackspace();
            return;
        }

        // พิมพ์ตัวอักษร (ข้าม control และช่องว่าง เพราะเราไม่ให้พิมพ์เว้นวรรคในโหมดนี้)
        if (Input.anyKeyDown && Input.inputString.Length > 0)
        {
            foreach (char c in Input.inputString)
            {
                if (char.IsControl(c) || c == ' ') continue; // ตัด space ออก
                if (!started) { started = true; startTime = Time.time; }
                HandleChar(c);
            }
        }

        // อัปเดตสถิติ
        if (started && !segmentFinished && !finished)
        {
            float t = Mathf.Max(Time.time - startTime, 0f);
            float wpm = (t > 0f) ? (correctTotal / 5f) / (t / 60f) : 0f;
            float acc = totalChars > 0 ? (float)correctTotal / totalChars : 0f;
            ui.UpdateLiveStats(wpm, acc, t);
        }
    }

    // ---------- Core ----------
    void LoadSegment(int index)
    {
        if (index >= segments.Length)
        {
            FinishAll();
            return;
        }

        // current = segment ตัดเว้นวรรคเพื่อให้ผู้เล่นไม่ต้องพิมพ์ช่องว่าง
        current = (segments[index] ?? "").Replace(" ", "");
        charIdx = 0;
        segmentFinished = false;
        states = new S[current.Length];

        RenderSegment();
    }

    void HandleChar(char c)
    {
        if (segmentFinished) return;
        if (charIdx >= current.Length) { segmentFinished = true; return; }

        char expected = current[charIdx];
        if (c == expected)
        {
            if (states[charIdx] != S.Correct) correctTotal++;
            states[charIdx] = S.Correct;
        }
        else
        {
            if (states[charIdx] == S.Correct) correctTotal--;
            states[charIdx] = S.Wrong;
            ui.ShowErrorEffect();
        }

        charIdx++;
        if (charIdx >= current.Length) segmentFinished = true;

        RenderSegment();
        ui.UpdateTypingProgress(CalcTypedGlobalCount(), totalChars);

        // ไม่ข้ามอัตโนมัติ รอ Space จากผู้เล่น
    }

    void HandleBackspace()
    {
        if (charIdx <= 0) return;
        int i = charIdx - 1;

        if (states[i] == S.Correct) { correctTotal--; states[i] = S.Corrected; }
        else                        { states[i] = S.Untyped; }

        charIdx = i;
        segmentFinished = false;
        RenderSegment();
        ui.UpdateTypingProgress(CalcTypedGlobalCount(), totalChars);
    }

    void GoNextSegment()
    {
        segIdx++;
        if (segIdx >= segments.Length)
        {
            FinishAll();
        }
        else
        {
            LoadSegment(segIdx);
        }
    }

    void FinishAll()
    {
        if (finished) return;
        finished = true;

        float used  = (started ? Mathf.Max(Time.time - startTime, 0.0001f) : 0f);
        float acc01 = totalChars > 0 ? (float)correctTotal / totalChars : 0f;
        float wpm   = (used > 0f) ? (correctTotal / 5f) / (used / 60f) : 0f;

        ui.ShowResult(wpm, acc01, used);

        // TODO: ถ้าจะส่งค่าไป Result/GDM ให้ใส่ตรงนี้เหมือนตัวเดิม
        // var pack = new ScoreData { ... };
        // GameDataManager.Instance.SetScore(pack);
        // StartCoroutine(LoadResultAfterDelay(1.0f));
    }

    // ---------- Render ----------
    void RenderSegment()
    {
        // สร้าง RichText แค่ของ segment ปัจจุบัน (ให้รู้สึกว่า “โฟกัสบล็อคนี้”)
        var sb = new StringBuilder(current.Length * 20);
        for (int i = 0; i < current.Length; i++)
        {
            if (i == charIdx && !segmentFinished) sb.Append($"<color={CURSOR}>|</color>");

            char ch = current[i];
            switch (states[i])
            {
                case S.Correct:    sb.Append($"<color={GREEN}>{ch}</color>"); break;
                case S.Wrong:      sb.Append($"<color={RED}>{ch}</color>");   break;
                case S.Corrected:  sb.Append($"<color={YELLOW}>{ch}</color>");break;
                default:           sb.Append(ch); break;
            }
        }
        if (segmentFinished) sb.Append($"  <size=70%><color=#999999>(Space → ต่อ)</color></size>");
        ui.SetLessonRichText(sb.ToString());
    }

    // ---------- Utils ----------
    int CountCharsNoSpace(string s)
        => string.IsNullOrEmpty(s) ? 0 : s.Replace(" ", "").Length;

    int CalcTypedGlobalCount()
    {
        // รวม “ตัวที่พิมพ์ครบ” ก่อนหน้า + ที่พิมพ์แล้วใน segment ปัจจุบัน
        int sum = 0;
        for (int i = 0; i < segIdx; i++) sum += CountCharsNoSpace(segments[i]);
        sum += Mathf.Min(charIdx, current.Length);
        return sum;
    }
}
