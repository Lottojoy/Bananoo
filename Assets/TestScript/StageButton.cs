using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StageButton : MonoBehaviour
{
    public GameObject popupUI;
    public string lessonID;        
    public int stageID;
    public string nameScene;
    public string infoText;
    public TMP_Text stageText;

    public Button buttonstate;
    public Button buttonpopup;
    public Image stageImage;          // Image ของปุ่ม
    public Sprite unlockedSprite;     // รูปที่เล่นได้
    public Sprite lockedSprite;       // รูปที่ล็อค

    private bool checkToggle = true;

    void Start()
    {
        stageText.text = infoText;

        // ตรวจสอบสิทธิ์ก่อน
        CheckAccess();

        if(buttonstate != null)
            buttonstate.onClick.AddListener(TogglePopup);

        if(buttonpopup != null)
            buttonpopup.onClick.AddListener(PlayLesson);
    }

    void CheckAccess()
    {
        int slot = PlayerPrefs.GetInt("CurrentSlot", 0);
        Player playerData = SaveManager.LoadPlayer(slot);

        if (playerData == null)
        {
            Debug.LogWarning("ไม่พบข้อมูล Player");
            buttonstate.interactable = false;
            buttonpopup.interactable = false;
            if (stageImage != null) stageImage.sprite = lockedSprite;
            return;
        }

        if (stageID > playerData.currentLessonIndex)
        {
            // ด่านมากกว่า current → ล็อค
            buttonstate.interactable = false;
            buttonpopup.interactable = false;
            if (stageImage != null) stageImage.sprite = lockedSprite;
        }
        else
        {
            // ด่านเล่นได้
            buttonstate.interactable = true;
            buttonpopup.interactable = true;
            if (stageImage != null) stageImage.sprite = unlockedSprite;
        }
    }

    void TogglePopup()
    {
        if(checkToggle)
        {
            checkToggle = false;
            OpenPopup();
        }
        else
        {
            checkToggle = true;
            ClosePopup();
        }
    }

    void OpenPopup()
    {
        if (popupUI != null)
            popupUI.SetActive(true);
    }

    public void ClosePopup()
    {
        if (popupUI != null)
            popupUI.SetActive(false);
    }

    public void PlayLesson()
    {
        int slot = PlayerPrefs.GetInt("CurrentSlot", 0);
        Player playerData = SaveManager.LoadPlayer(slot);

        if (playerData == null || stageID > playerData.currentLessonIndex)
        {
            Debug.Log("ยังไม่สามารถเข้าเล่นด่านนี้ได้");
            return;
        }

        GameData.CurrentLessonID = lessonID;
        GameData.CurrentStageID = stageID;
        SceneManager.LoadScene(nameScene);
    }
}
