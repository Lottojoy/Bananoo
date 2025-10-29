using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class StageButton : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private Lesson lesson;   // ลาก Lesson.asset ตรงนี้

    [Header("UI")]
    [SerializeField] private TMP_Text stageText;     // ข้อความบนปุ่ม (Title)
    [SerializeField] private Button playButton;
    [SerializeField] private Image stageImage;
    [SerializeField] private GameObject popupUI;     // กล่อง popup
    [SerializeField] private TMP_Text infoPopupText; // ← เปลี่ยนชื่อให้ชัด (เดิม InfopopupUI)
    [SerializeField] private Sprite lockedSprite;
    [SerializeField] private Sprite unlockedSprite;
    [SerializeField] private Button btnRoot;

    private bool isPopupOpen = false;

    // ถ้าสร้างด้วยโค้ด
    public void Initialize(Lesson lsn)
    {
        lesson = lsn;
        // รอ Refresh ใน Start เพื่อให้ PlayerManager พร้อมกว่า
    }

    private void Start()
    {
        WireUpButtons();
        if (popupUI) popupUI.SetActive(false);
        RefreshUI();
    }

    private void WireUpButtons()
    {
        if (btnRoot)
        {
            btnRoot.onClick.RemoveAllListeners();
            btnRoot.onClick.AddListener(OnRootClicked);
        }

        if (playButton)
        {
            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(OnPlayClicked);
        }
    }

    /// <summary>รีเฟรชสถานะปุ่ม/สไปรท์จาก PlayerManager</summary>
    private void RefreshUI()
    {
        if (lesson == null) return;

        var player   = PlayerManager.Instance?.CurrentPlayer;
        int progress = player != null ? player.currentLessonID : 0;
        int lessonNum = lesson.LessonID;

        // ✅ ตั้งชื่อบนปุ่ม (อยากเปลี่ยนเป็นชื่อ custom ก็เพิ่มฟิลด์ Title ใน Lesson ได้)
        if (stageText) stageText.text = $"Lesson {lessonNum}";

        bool unlocked = (player != null) && (lessonNum <= progress);

        if (playButton) playButton.interactable = unlocked;
        if (stageImage) stageImage.sprite = unlocked ? unlockedSprite : lockedSprite;

        // อย่า set popup text ที่นี่ (จะไป set ตอนเปิด popup)
        Debug.Log($"[StageButton] Refresh L{lessonNum} — progress={progress}, unlocked={unlocked}");
    }

    private void OnRootClicked()
    {
        RefreshUI(); // รีเฟรชอีกครั้งก่อนตัดสินใจ

        var player   = PlayerManager.Instance?.CurrentPlayer;
        int progress = player != null ? player.currentLessonID : 0;
        int lessonNum = lesson != null ? lesson.LessonID : -1;

        bool canEnter = (player != null) && (lessonNum <= progress);

        if (!canEnter)
        {
            Debug.LogWarning($"[StageButton] ด่านนี้คือเลข {lessonNum} แต่คุณเล่นได้ถึงแค่ด่าน {progress} จึงยังเข้าไม่ได้");
            if (popupUI) popupUI.SetActive(false);
            return;
        }

        // ✅ ตั้งข้อความใน popup ตอนเปิด
        if (infoPopupText) infoPopupText.text = lesson.InfoText;

        isPopupOpen = !isPopupOpen;
        if (popupUI) popupUI.SetActive(isPopupOpen);
        Debug.Log($"[StageButton] ด่าน {lessonNum} พร้อมเล่น (progress={progress}).");
    }

    private void OnPlayClicked()
    {
        RefreshUI(); // กันพลาด

        var player   = PlayerManager.Instance?.CurrentPlayer;
        int progress = player != null ? player.currentLessonID : 0;
        int lessonNum = lesson != null ? lesson.LessonID : -1;

        bool canEnter = (player != null) && (lessonNum <= progress);
    if (!canEnter) { Debug.LogWarning($"Locked L{lessonNum}"); return; }
    if (lesson == null) { Debug.LogError("Lesson is null"); return; }

    // ✅ ส่งค่าบทเรียนข้ามซีน
    LessonContext.SelectedLesson  = lesson;
    LessonContext.SelectedLessonID = lesson.LessonID;

    SceneManager.LoadScene(lesson.SceneName); // หรือ "LessonWordScene"
    }
}
