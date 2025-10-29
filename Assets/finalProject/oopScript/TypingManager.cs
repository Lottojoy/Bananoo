using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text;
using System.Collections.Generic;
public class TypingManager : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private LessonWordUI uiManager;
    private readonly Dictionary<char,int> wrongPerExpected = new Dictionary<char,int>();
    private const string GREEN = "#2ECC71";
    private const string RED   = "#E74C3C";
    private const string YELLOW= "#F1C40F";
    private const string CURSOR= "#FFFFFF";

    private Lesson currentLesson;
    private string lessonContent;

    private int currentIndex = 0;
    private int correctCount = 0;

    private float startTime = 0f;
    // เพิ่มฟิลด์สถานะ
private bool started = false;
private bool finished = false;

    private enum CharState { Untyped, Correct, Wrong, Corrected }
    private CharState[] states;

    void Awake()
    {
        if (uiManager == null)
            uiManager = FindObjectOfType<LessonWordUI>(includeInactive: true);
    }

    void Start()
    {
        currentLesson = LessonContext.SelectedLesson;
        if (currentLesson == null && LessonContext.SelectedLessonID > 0)
            currentLesson = DataManager.Instance.GetLessonByID(LessonContext.SelectedLessonID);

        if (currentLesson == null || uiManager == null)
        {
            Debug.LogError("[TypingManager] Missing refs.");
            if (uiManager) uiManager.SetLessonText("No lesson loaded.");
            enabled = false;
            return;
        }

        lessonContent = currentLesson.GetText() ?? "";
        states = new CharState[lessonContent.Length];
        currentIndex = 0;
        correctCount = 0;

        Render();
        uiManager.UpdateTypingProgress(currentIndex, lessonContent.Length);

        // ⛔ อย่า set startTime ที่นี่ — จะ set ตอนพิมพ์ครั้งแรกแทน
        started = false;
        uiManager.UpdateLiveStats(0f, 0f, 0f); // เคลียร์ค่าเริ่มต้น
    }

    void Update()
{
    if (string.IsNullOrEmpty(lessonContent)) return;

    // ถ้าจบแล้ว ไม่รับอินพุต ไม่อัปเดตสถิติ
    if (finished) return;

    // Backspace
    if (Input.GetKeyDown(KeyCode.Backspace))
    {
        if (!started) { started = true; startTime = Time.time; } // จะเอาออกได้ถ้าไม่อยากเริ่มด้วย backspace
        HandleBackspace();
        return;
    }

    // ตัวอักษรจริง
    if (Input.anyKeyDown && Input.inputString.Length > 0)
    {
        foreach (char c in Input.inputString)
        {
            if (char.IsControl(c)) continue;
            if (!started) { started = true; startTime = Time.time; }
            HandleCharInput(c);
        }
    }

    // อัปเดต WPM/เวลาแบบเรียลไทม์ (เฉพาะยังไม่จบ)
    if (started && currentIndex < lessonContent.Length)
    {
        float elapsed = Mathf.Max(Time.time - startTime, 0f);
        float wpmLive = elapsed > 0f ? (correctCount / 5f) / (elapsed / 60f) : 0f;
        float accLive = (lessonContent.Length > 0) ? (float)correctCount / lessonContent.Length : 0f;
        uiManager.UpdateLiveStats(wpmLive, accLive, elapsed);
    }
}

    private void HandleCharInput(char c)
    {
        if (currentIndex >= lessonContent.Length) return;
        char expected = lessonContent[currentIndex];

        if (c == expected)
        {
            if (states[currentIndex] != CharState.Correct) correctCount += 1;
            states[currentIndex] = CharState.Correct;
        }
        else
{
    if (states[currentIndex] == CharState.Correct) correctCount -= 1;
    states[currentIndex] = CharState.Wrong;

    // ✅ นับผิดต่อ "expected char"
    char expectedChar = lessonContent[currentIndex];
    if (!wrongPerExpected.ContainsKey(expectedChar)) wrongPerExpected[expectedChar] = 0;
    wrongPerExpected[expectedChar]++;
}

        currentIndex++;
        Render();
        uiManager.UpdateTypingProgress(currentIndex, lessonContent.Length);

        if (currentIndex >= lessonContent.Length)
            FinishLesson();
    }

    private void HandleBackspace()
    {
        if (currentIndex <= 0) return;
        int i = currentIndex - 1;

        if (states[i] == CharState.Correct) { correctCount -= 1; states[i] = CharState.Corrected; }
        else                                { states[i] = CharState.Untyped; }

        currentIndex = i;
        Render();
        uiManager.UpdateTypingProgress(currentIndex, lessonContent.Length);
    }

    private void Render()
    {
        if (uiManager == null) return;
        var sb = new StringBuilder(lessonContent.Length * 20);

        for (int i = 0; i < lessonContent.Length; i++)
        {
            if (i == currentIndex) sb.Append($"<color={CURSOR}>|</color>");

            char ch = lessonContent[i];
            switch (states[i])
            {
                case CharState.Correct:    sb.Append($"<color={GREEN}>{ch}</color>"); break;
                case CharState.Wrong:      sb.Append($"<color={RED}>{ch}</color>");   break;
                case CharState.Corrected:  sb.Append($"<color={YELLOW}>{ch}</color>");break;
                default:                   sb.Append(ch);                              break;
            }
        }
        if (currentIndex == lessonContent.Length) sb.Append($"<color={CURSOR}>|</color>");
        uiManager.SetLessonRichText(sb.ToString());
    }

private void FinishLesson()
{
    if (finished) return;
    finished = true;

    float timeUsed = (started ? Mathf.Max(Time.time - startTime, 0.0001f) : 0f);
    float acc01    = (lessonContent.Length > 0) ? (float)correctCount / lessonContent.Length : 0f; // 0..1
    float wpm      = (timeUsed > 0f) ? (correctCount / 5f) / (timeUsed / 60f) : 0f;

    // 1) ล็อกค่าบน UI ตอนจบ
    if (uiManager != null)
    {
        uiManager.ShowResult(wpm, acc01, timeUsed);
        uiManager.UpdateLiveStats(wpm, acc01, timeUsed);
    }
    else
    {
        Debug.LogWarning("[TypingManager] uiManager is null at finish; cannot show result UI.");
    }

    // 2) แพ็กผลลัพธ์
    var pack = new ScoreData
    {
        LessonID    = (currentLesson != null) ? currentLesson.LessonID : 0,
        LessonTitle = (currentLesson != null) ? currentLesson.name      : "Unknown",
        WPM         = wpm,
        ACC         = acc01 * 100f,   // เก็บเป็นเปอร์เซ็นต์
        TimeUsed    = timeUsed,
        FinalScore  = Mathf.Round((wpm * 10f) * acc01)  // สูตรตัวอย่าง
    };

    // 3) คัดลอกสถิติอักขระที่ผิด
    foreach (var kv in wrongPerExpected)
    {
        pack.WrongChars.Add(kv.Key);
        pack.WrongCounts.Add(kv.Value);
    }

    // 4) ส่งเข้าผู้จัดการกลาง (และสายสำรอง)
    var gdm = GetGDM();
    if (gdm != null)
    {
        gdm.SetScore(pack);           // <-- ใช้เมธอดนี้ (ดูด้านล่าง)
        // หรือถ้าเปิด public set: gdm.ScoreData = pack;
    }
    ResultContext.Set(pack); // กันตายไว้ด้วย

    // 5) ปลดล็อกด่าน (ถ้ามี PlayerManager)
    var pm = GetPM();
    if (pm != null && currentLesson != null)
        pm.OnLessonCleared(currentLesson.LessonID);

    // 6) หน่วง 3 วิแล้วไป Result
    StartCoroutine(LoadResultAfterDelay(3f));
}


private System.Collections.IEnumerator LoadResultAfterDelay(float sec)
{
    yield return new WaitForSeconds(sec);
    UnityEngine.SceneManagement.SceneManager.LoadScene("ResultScene");
}
// หา/สร้าง GameDataManager ถ้ายังไม่มี (กัน NRE)
private GameDataManager GetGDM()
{
    var gdm = GameDataManager.Instance ?? FindObjectOfType<GameDataManager>(true);
    if (gdm == null)
    {
        var go = new GameObject("GameDataManager(Auto)");
        gdm = go.AddComponent<GameDataManager>(); // Awake จะเซ็ต Instance + DontDestroyOnLoad
        Debug.LogWarning("[TypingManager] Auto-created GameDataManager because none was present.");
    }
    return gdm;
}

// หา PlayerManager ถ้าไม่มี instance (ไม่สร้างใหม่ เพื่อเลี่ยง duplicate save owner)
private PlayerManager GetPM()
{
    var pm = PlayerManager.Instance ?? FindObjectOfType<PlayerManager>(true);
    if (pm == null)
        Debug.LogWarning("[TypingManager] PlayerManager not found. Progress will not be saved this time.");
    return pm;
}

}
