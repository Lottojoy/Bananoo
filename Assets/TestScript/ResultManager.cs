using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    public Button backButton;
    private int slot;

    void Start()
    {
        // โหลด slot ปัจจุบันที่เล่นจาก PlayerPrefs
        slot = PlayerPrefs.GetInt("CurrentSlot", 0);

        if (backButton != null)
            backButton.onClick.AddListener(OnBackToMain);
    }

    void OnBackToMain()
    {
        // โหลดข้อมูล Player ปัจจุบัน
        Player player = SaveManager.LoadPlayer(slot);
        if (player == null)
        {
            Debug.LogWarning("ไม่มีข้อมูล Player ใน slot นี้");
            SceneManager.LoadScene("MainScene");
            return;
        }

        Debug.Log($"[Result] Player {player.playerName} เล่นถึงด่าน {GameData.CurrentStageID} (ปัจจุบัน={player.currentLessonIndex})");

        // ถ้าผ่านด่านใหม่ (ไกลกว่าเดิม)
        if (GameData.CurrentStageID >= player.currentLessonIndex)
        {
            player.currentLessonIndex = GameData.CurrentStageID+1;
            Debug.Log($"🎉 อัปเดตบทเรียนใหม่ = {player.currentLessonIndex}");
            SaveManager.SavePlayer(slot, player);
        }

        // ล้างค่า GameData
        ResetGameData();

        // กลับไปหน้า Main
        SceneManager.LoadScene("MainScene");
    }

    void ResetGameData()
    {
        GameData.CurrentLessonID = null;
        GameData.CurrentStageID = 0;
        Debug.Log("[Result] รีเซ็ต GameData เรียบร้อย");
    }
}
