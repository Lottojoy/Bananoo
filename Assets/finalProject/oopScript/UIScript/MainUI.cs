using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainUI : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text playerNameText;
    public TMP_Text currentLessonText;
    public RawImage characterImage;

    [Header("Character Images")]
    public Texture[] characterImages; // เหมือน MenuUI

    private void Start()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        var player = PlayerManager.Instance.CurrentPlayer;
        if (player == null)
        {
            playerNameText.text = "ไม่มีผู้เล่น";
            currentLessonText.text = "-";
            characterImage.gameObject.SetActive(false);
            return;
        }

        // ชื่อผู้เล่น
        playerNameText.text = player.playerName;

        // บทเรียนปัจจุบัน
        currentLessonText.text = ""+player.currentLessonID;

        // แสดงไอคอนตัวละคร
        if (player.characterIndex >= 0 && player.characterIndex < characterImages.Length)
        {
            characterImage.texture = characterImages[player.characterIndex];
            characterImage.gameObject.SetActive(true);
        }
        else
        {
            characterImage.gameObject.SetActive(false);
        }
    }
}
