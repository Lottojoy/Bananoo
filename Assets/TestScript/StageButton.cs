using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StageButton : MonoBehaviour
{
    public GameObject popupUI;
    public string lessonID;        
    public int mapID;
    public int stageID;

    public string nameScene;
    public string infoText;
    public TMP_Text stageText;
    public Button buttonstate;
     public Button buttonpopup;
    private bool checkToggle = true;
    void Start()
    {
        stageText.text = infoText;

        if(buttonstate != null)
            buttonstate.onClick.AddListener(TogglePopup);

        if(buttonpopup != null)
            buttonpopup.onClick.AddListener(PlayLesson);
    }
    
    void TogglePopup()
    {
        
        
        if(checkToggle){
            //Debug.Log("กดปุ่มเปิด");
            checkToggle = false;
                OpenPopup();
        }else{
            //Debug.Log("กดปุ่มปิด");
            checkToggle = true;
            ClosePopup();
        }
            
    }
    void OpenPopup()
    {
        if (popupUI != null)
        {
            popupUI.SetActive(true);

            
        }
    }

    public void ClosePopup()
    {
        if (popupUI != null)
            popupUI.SetActive(false);
    }

    // ใช้ปุ่ม Play ใน Popup
    public void PlayLesson()
    {
        // ส่งค่าปัจจุบันไป GameData
            GameData.CurrentLessonID = lessonID;
            GameData.CurrentMapID = mapID;
            GameData.CurrentStageID = stageID;
        SceneManager.LoadScene(nameScene); // เปลี่ยนชื่อเป็น Scene การเล่นจริง
    }
}
