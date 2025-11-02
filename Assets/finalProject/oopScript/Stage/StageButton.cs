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
    [SerializeField] private TMP_Text infoPopupText; // ข้อความใน popup
    [SerializeField] private Sprite lockedSprite;
    [SerializeField] private Sprite unlockedSprite;
    [SerializeField] private Button btnRoot;

    

    private bool isPopupOpen = false;

    // ✅ ถ้าสร้างด้วยโค้ด
    public void Initialize(Lesson lsn)
    {
        lesson = lsn;
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

        // ตั้งชื่อบนปุ่ม
        if (stageText) stageText.text = $"Lesson {lessonNum}";

        bool unlocked = (player != null) && (lessonNum <= progress);

        if (playButton) playButton.interactable = unlocked;
        if (stageImage) stageImage.sprite = unlocked ? unlockedSprite : lockedSprite;

        Debug.Log($"[StageButton] Refresh L{lessonNum} — progress={progress}, unlocked={unlocked}");
    }

    private void OnRootClicked()
    {
        // 🎧 เสียงคลิกปุ่ม
        ButtonSound.PlayClick();
        // 🎬 แอนิเมชันปุ่ม
        
        RefreshUI(); 

        var player   = PlayerManager.Instance?.CurrentPlayer;
        int progress = player != null ? player.currentLessonID : 0;
        int lessonNum = lesson != null ? lesson.LessonID : -1;

        bool canEnter = (player != null) && (lessonNum <= progress);

        if (!canEnter)
        {
            Debug.LogWarning($"[StageButton] ด่านนี้คือเลข {lessonNum} แต่คุณเล่นได้ถึงแค่ {progress} ยังเข้าไม่ได้");
            if (popupUI) popupUI.SetActive(false);
            return;
        }

        // ตั้งข้อความใน popup ตอนเปิด
        if (infoPopupText) infoPopupText.text = lesson.InfoText;

        isPopupOpen = !isPopupOpen;
        if (popupUI) popupUI.SetActive(isPopupOpen);

       
        Debug.Log($"[StageButton] ด่าน {lessonNum} พร้อมเล่น (progress={progress}).");
    }

    private void OnPlayClicked()
    {
        // 🎧 เสียงคลิกปุ่ม
        ButtonSound.PlayClick();
        
        RefreshUI();

        var player   = PlayerManager.Instance?.CurrentPlayer;
        int progress = player != null ? player.currentLessonID : 0;
        int lessonNum = lesson != null ? lesson.LessonID : -1;

        bool canEnter = (player != null) && (lessonNum <= progress);
        if (!canEnter) { Debug.LogWarning($"Locked L{lessonNum}"); return; }
        if (lesson == null) { Debug.LogError("Lesson is null"); return; }

        // ✅ ส่งค่าบทเรียนข้ามซีน
        LessonContext.SelectedLesson  = lesson;
        LessonContext.SelectedLessonID = lesson.LessonID;

        
        SceneLoader.FadeToScene(lesson.SceneName);
    }
}
