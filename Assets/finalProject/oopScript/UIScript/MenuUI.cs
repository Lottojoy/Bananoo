using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
   [Header("UI References")]
    public Button[] playerButtons;
    public TMP_Text[] playerButtonTexts;
    public RawImage[] playerImages;
    public Texture[] characterImages;
    public Button[] deleteButtons;

    private void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            int slot = i;
            UpdateSlotUI(slot);

            playerButtons[i].onClick.AddListener(() => OnSelectSlot(slot));
            deleteButtons[i].onClick.AddListener(() => OnDeleteSlot(slot));
        }
    }

    void UpdateSlotUI(int slot)
    {
        Player player = PlayerManager.Instance.LoadPlayer(slot);

        if (player != null)
        {
            playerButtonTexts[slot].text = $"{player.playerName}, {player.streakDays} วัน, บทเรียน {player.currentLessonID}";
            if (player.characterIndex >= 0 && player.characterIndex < characterImages.Length)
            {
                playerImages[slot].texture = characterImages[player.characterIndex];
                playerImages[slot].gameObject.SetActive(true);
            }
            else playerImages[slot].gameObject.SetActive(false);
        }
        else
        {
            playerButtonTexts[slot].text = "ไม่มีบันทึกตัวละคร";
            playerImages[slot].gameObject.SetActive(false);
        }
    }

    void OnSelectSlot(int slot)
    {
        PlayerManager.Instance.SelectSlot(slot);

        if (PlayerManager.Instance.CurrentPlayer != null)
            SceneManager.LoadScene("MainScene");
        else
            SceneManager.LoadScene("SelectScene");
    }

    void OnDeleteSlot(int slot)
    {
        PlayerManager.Instance.DeletePlayer(slot);
        UpdateSlotUI(slot);
    }
}
