using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainSceneUI : MonoBehaviour
{
    [Header("UI")]
    public RawImage characterImage;
    public TMP_Text playerNameText;
    public TMP_Text streakDaysText;
    public TMP_Text lessonText;

    [Header("Assets")]
    public Texture[] characterImages;  // รูปตัวละครทั้งหมด

    private int slot;

    void Start()
    {
        // โหลด slot ที่เคยเลือกไว้
        slot = PlayerPrefs.GetInt("CurrentSlot", 0);

        // โหลดข้อมูล Player จาก Save
        Player playerData = SaveManager.LoadPlayer(slot);
        if (playerData == null)
        {
            Debug.LogWarning("ไม่พบข้อมูลผู้เล่นใน slot " + slot);
            return;
        }

        // แสดงผลใน UI
        if (characterImage != null && playerData.characterIndex >= 0 && playerData.characterIndex < characterImages.Length)
        {
            characterImage.texture = characterImages[playerData.characterIndex];
        }

        if (playerNameText != null)
            playerNameText.text = playerData.playerName;

        if (streakDaysText != null)
            streakDaysText.text = playerData.streakDays + " วันต่อเนื่อง";

        if (lessonText != null)
            lessonText.text = ""+playerData.currentLessonIndex;
    }
}
