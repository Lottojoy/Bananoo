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

    private StageData data;
    private bool isPopupOpen = false;

    public void Initialize(StageData stageData)
    {
        data = stageData;
        stageText.text = data.infoText;

        CheckAccess();

        playButton.onClick.AddListener(OnPlayClicked);
    }

    private void CheckAccess()
{
    var player = GameManager.Instance.CurrentPlayer;

    // สมมติ data.stageID เป็น string lessonID ของด่านนี้
    bool unlocked = player != null && string.Compare(data.lessonID, player.currentLessonID) <= 0;

    playButton.interactable = unlocked;
    stageImage.sprite = unlocked ? unlockedSprite : lockedSprite;
}


    private void OnPlayClicked()
    {
        Lesson lesson = DataManager.Instance.GetLessonByID(data.lessonID);
        if (lesson == null)
        {
            Debug.LogError($"Lesson {data.lessonID} not found!");
            return;
        }

       GameManager.Instance.SelectLesson(lesson);
        SceneController.Instance.LoadScene(data.sceneName);
    }
}
