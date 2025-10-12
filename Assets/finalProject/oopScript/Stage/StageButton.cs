using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageButton : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text stageText;
    [SerializeField] private Button playButton;
    [SerializeField] private Image stageImage;
    [SerializeField] private GameObject popupUI;
    [SerializeField] private Sprite lockedSprite;
    [SerializeField] private Sprite unlockedSprite;
    public Button btnRoot;
    private StageData data;
    private bool isPopupOpen = false;

    public void Initialize(StageData stageData)
{
    data = stageData;
    stageText.text = data.infoText;

    CheckAccess();

    // กดปุ่มใหญ่ → เปิด/ปิด popup
    btnRoot.onClick.AddListener(() =>
    {
        if (playButton.interactable) // กดได้เฉพาะ unlocked
        {
            isPopupOpen = !isPopupOpen;
            popupUI.SetActive(isPopupOpen);
        }
    });

    // กด playButton → เข้า LessonScene
    playButton.onClick.AddListener(OnPlayClicked);
}

    private void CheckAccess()
{
    var player = GameManager.Instance.CurrentPlayer;

    // สมมติ data.stageID เป็น string lessonID ของด่านนี้
    int lessonNum = int.Parse(data.lessonID.Replace("Lesson", ""));
bool unlocked = player != null && lessonNum <= player.currentLessonID;

    playButton.interactable = unlocked;
    stageImage.sprite = unlocked ? unlockedSprite : lockedSprite;
    popupUI.SetActive(false); // เริ่มต้นปิด popup

}


    private void OnPlayClicked()
{
   
   int lessonNum = int.Parse(data.lessonID.Replace("Lesson", ""));
Lesson lesson = DataManager.Instance.GetLessonByID(lessonNum);
    if (lesson == null)
    {
        Debug.LogError($"Lesson {data.lessonID} not found!");
        return;
    }

    GameManager.Instance.SelectLesson(lesson);
    
}

}
