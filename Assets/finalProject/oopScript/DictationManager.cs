using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class DictationManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private DictationUI ui;

    [Header("Flow")]
    [SerializeField] private bool  autoGoResult = true;
    [SerializeField] private float resultDelay  = 0.8f;

    private Lesson      lesson;
    private AudioSource audioSrc;

    private bool isPlaying   = false;
    private bool hasPlayed   = false;

    // gate เงื่อนไขการโชว์ submit
    private bool hasPlayedOnce = false;
    private bool hasTypedAny   = false;

    private bool  timeStarted = false; // เริ่มเมื่อ "กด Play ครั้งแรก"
    private float startedAt   = 0f;
    private bool  submitted   = false; // กันอัปเดตเวลา/สถิติสดหลัง submit

    private string correctText;

    // ค่าความยาวบทเรียน (เพื่อเก็บใน ScoreData)
    private int playedChars = 0;  // จำนวนตัวอักษร (ตัด space)
    private int playedWords = 0;  // จำนวนคำ

    void Awake()
    {
        if (!ui) ui = FindObjectOfType<DictationUI>(true);
        audioSrc = GetComponent<AudioSource>();
        audioSrc.playOnAwake = false;
    }

    void Start()
    {
        // ---- โหลดบทเรียน ----
        lesson = LessonContext.SelectedLesson;
        if (lesson == null && LessonContext.SelectedLessonID > 0)
            lesson = DataManager.Instance.GetLessonByID(LessonContext.SelectedLessonID);

        if (lesson == null || ui == null)
        {
            Debug.LogError("[DictationManagerAudio] Missing lesson/UI");
            enabled = false; return;
        }
        if (lesson.Type != LessonType.Audio || !lesson.HasAudio)
        {
            Debug.LogError("[DictationManagerAudio] Lesson is not Audio or missing clip");
            enabled = false; return;
        }

        audioSrc.clip = lesson.VoiceClip;
        correctText   = lesson.GetText() ?? "";

        // ✅ คำนวณความยาวจาก Lesson
        CalcLessonCountsFromLesson(lesson, out playedChars, out playedWords);

        // ---- ตั้งค่า UI เริ่มต้น ----
        ui.SetTitle(lesson.name);
        ui.SetPrompt(string.IsNullOrWhiteSpace(lesson.InfoText) ? "แบบฝึกฟังเสียงแล้วพิมพ์ตาม" : lesson.InfoText);
        ui.ClearAnswer();
        ui.UpdatePlayButtonLabel(false);
        ui.UpdatePlayState(false, false);
        ui.SetLiveTime(0f);

        // เปิดให้พิมพ์เสมอ, ซ่อน submit ไว้ก่อน
        ui.SetInputEnabled(true);
        ui.SetPlayEnabled(true);
        ui.SetSubmitActive(false);
        ui.HidePopup();

        // ---- ผูกปุ่ม / อินพุต ----
        ui.WirePlayButton(OnPlayClicked);
        ui.WireSubmitButton(OnSubmitClicked);
        ui.WirePopupNextButton(OnPopupNext);
        ui.WirePopupCloseButton(() => ui.HidePopup());

        ui.WireAnswerChanged(s =>
        {
            hasTypedAny = !string.IsNullOrEmpty(s); // จะ .Trim() ก็ได้ถ้าต้องการกัน space ล้วน
            UpdateSubmitGate();
        });
    }

    void Update()
    {
        // แสดงเวลาสดเฉพาะยังไม่ submit และกด Play แล้ว
        if (timeStarted && !submitted)
        {
            float elapsed = Mathf.Max(Time.time - startedAt, 0f);
            ui.SetLiveTime(elapsed);
        }
    }

    // ---------- Gate การโชว์ submit ----------
    private void UpdateSubmitGate()
    {
        bool canShow = hasPlayedOnce && hasTypedAny;
        ui.SetSubmitActive(canShow);
    }

    // ---------- Play / Replay ----------
    private void OnPlayClicked()
    {
        if (isPlaying) return;

        if (!timeStarted)
        {
            timeStarted = true;
            startedAt   = Time.time; // ⏱ เริ่มนับเมื่อ Play ครั้งแรก
        }

        if (!hasPlayedOnce)
        {
            hasPlayedOnce = true;
            UpdateSubmitGate(); // อัปเดตเกท submit เมื่อเล่นครั้งแรกสำเร็จ
        }

        StartCoroutine(Co_PlayOnce());
    }

    private IEnumerator Co_PlayOnce()
    {
        if (!audioSrc.clip) yield break;

        isPlaying = true;

        // ปิดเฉพาะปุ่ม Play ชั่วคราว แต่ "ช่องพิมพ์" เปิดไว้
        ui.SetPlayEnabled(false);
        ui.UpdatePlayState(true, hasPlayed);

        audioSrc.Stop();
        audioSrc.time = 0f;
        audioSrc.Play();

        while (audioSrc.isPlaying) yield return null;

        isPlaying = false;
        hasPlayed = true;
        ui.SetPlayEnabled(true);
        ui.UpdatePlayState(false, hasPlayed);
    }

    // ---------- Submit ----------
    private void OnSubmitClicked()
    {
        if (isPlaying) return;

        // กันเคสหลุดเกท (เช่น ยังไม่เคยกด play หรือข้อความว่าง)
        string rawAns = ui && ui.answerInput ? (ui.answerInput.text ?? "") : "";
        if (!hasPlayedOnce || string.IsNullOrEmpty(rawAns))
        {
            Debug.LogWarning("[DictationManager] Submit blocked: need played once AND non-empty input.");
            return;
        }

        submitted = true; // หยุดอัปเดตเวลาสดใน Update()

        string your = rawAns;
        string yourRich    = BuildColoredDiff(your, correctText, "#2ECC71", "#E74C3C");
        string correctRich = BuildColoredMaskFromUser(correctText, your, "#2ECC71", "#E74C3C");

        // คำนวนสถิติ
        float used = timeStarted ? Mathf.Max(Time.time - startedAt, 0.0001f) : 0f;
        string yourNoSpace    = (your        ?? "").Replace(" ", "");
        string correctNoSpace = (correctText ?? "").Replace(" ", "");
        int matched = CountMatches(yourNoSpace, correctNoSpace);
        int total   = Mathf.Max(correctNoSpace.Length, 1);
        float acc01 = (float)matched / total;
        float wpm   = (used > 0f) ? (matched / 5f) / (used / 60f) : 0f;

        // แสดงป๊อปอัป + สถิติในป๊อปอัป
        ui.ShowPopupRich(yourRich, correctRich);
        ui.SetPopupStats(wpm, acc01, used);

        // บันทึกลง GameDataManager (พร้อม playedCounts)
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
    }

    private void OnPopupNext()
    {
        if (autoGoResult)
            StartCoroutine(GoResultAfter(resultDelay));
        else
            ui.HidePopup();
    }

    // ---------- Diff helpers ----------
    private string BuildColoredDiff(string your, string correct, string okHex, string badHex)
    {
        your    = your    ?? "";
        correct = correct ?? "";
        int n = Mathf.Max(your.Length, correct.Length);
        var sb = new StringBuilder(n * 20);
        for (int i = 0; i < n; i++)
        {
            char yc = (i < your.Length)    ? your[i]    : '\0';
            char cc = (i < correct.Length) ? correct[i] : '\0';
            if (yc == '\0') continue;
            bool ok = (cc != '\0' && yc == cc);
            AppendColored(sb, yc, ok ? okHex : badHex);
        }
        return sb.ToString();
    }

    private string BuildColoredMaskFromUser(string correct, string your, string okHex, string badHex)
    {
        correct = correct ?? "";
        your    = your    ?? "";
        var sb = new StringBuilder(correct.Length * 20);
        for (int i = 0; i < correct.Length; i++)
        {
            char cc = correct[i];
            char yc = (i < your.Length) ? your[i] : '\0';
            bool ok = (yc != '\0' && yc == cc);
            AppendColored(sb, cc, ok ? okHex : badHex);
        }
        return sb.ToString();
    }

    private void AppendColored(StringBuilder sb, char ch, string hex)
    {
        if (ch == ' ' || ch == '\t' || ch == '\n')
        {
            sb.Append(ch); return;
        }
        sb.Append("<color=").Append(hex).Append('>').Append(ch).Append("</color>");
    }

    private int CountMatches(string a, string b)
    {
        int n = Mathf.Min(a.Length, b.Length);
        int c = 0;
        for (int i = 0; i < n; i++) if (a[i] == b[i]) c++;
        return c;
    }

    // ---------- Result / Managers ----------
    private IEnumerator GoResultAfter(float sec)
    {
        yield return new WaitForSeconds(sec);
        // ถ้ามี SceneLoader.FadeToScene ก็ใช้บรรทัดนี้
        SceneLoader.FadeToScene("ResultScene");
        // หรือถ้าไม่มี ให้ใช้:
        // SceneManager.LoadScene("ResultScene");
    }

    private GameDataManager GetGDM()
    {
        var gdm = GameDataManager.Instance ?? FindObjectOfType<GameDataManager>(true);
        if (gdm == null)
        {
            var go = new GameObject("GameDataManager(Auto)");
            gdm = go.AddComponent<GameDataManager>();
            Debug.LogWarning("[DictationManagerAudio] Auto-created GameDataManager.");
        }
        return gdm;
    }

    private PlayerManager GetPM()
    {
        var pm = PlayerManager.Instance ?? FindObjectOfType<PlayerManager>(true);
        if (pm == null) Debug.LogWarning("[DictationManagerAudio] PlayerManager not found.");
        return pm;
    }

    // ===== helper: คำนวณจำนวนตัว/คำจาก Lesson =====
    private static void CalcLessonCountsFromLesson(Lesson lesson, out int charCount, out int wordCount)
    {
        charCount = 0;
        wordCount = 0;
        if (lesson == null) return;

        // สำหรับ Audio/Word ใช้ words[] เป็นหลัก (ดึงผ่าน reflection)
        var wordsField = (string[]) lesson.GetType()
            .GetField("words", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.GetValue(lesson);

        if (wordsField != null && wordsField.Length > 0)
        {
            for (int i = 0; i < wordsField.Length; i++)
            {
                var w = wordsField[i];
                if (!string.IsNullOrWhiteSpace(w))
                    charCount += w.Replace(" ", "").Length;
            }
            wordCount = wordsField.Length;
            return;
        }

        // fallback: จาก GetText()
        string text = lesson.GetText() ?? "";
        charCount = text.Replace(" ", "").Length;

        if (!string.IsNullOrWhiteSpace(text))
        {
            var parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            wordCount = parts.Length;
        }
    }
}
