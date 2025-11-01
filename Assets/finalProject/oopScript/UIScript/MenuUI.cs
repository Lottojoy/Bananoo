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

    private bool isLoading = false; // กันโหลดซีนซ้ำ

    private void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            int slot = i;

            // เคลียร์ลิสเนอร์กันซ้ำ (สำคัญมากเวลารีโหลดซีน)
            playerButtons[i].onClick.RemoveAllListeners();
            deleteButtons[i].onClick.RemoveAllListeners();

            UpdateSlotUI(slot);

            playerButtons[i].onClick.AddListener(() => OnSelectSlot(slot));
            deleteButtons[i].onClick.AddListener(() => OnDeleteSlot(slot));
        }
    }

    void UpdateSlotUI(int slot)
    {
        var hasSave = PlayerPrefs.HasKey($"player{slot}_name");
        if (hasSave)
        {
            // โหลดมาเพื่อโชว์ UI เท่านั้น (ไม่แตะ CurrentPlayer)
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
                // มีคีย์แต่โหลดไม่ได้ (เคสพิเศษ) — ถือว่าไม่มี
                playerButtonTexts[slot].text = "ไม่มีบันทึกตัวละคร";
                playerImages[slot].gameObject.SetActive(false);
            }
        }
        else
        {
            playerButtonTexts[slot].text = "ไม่มีบันทึกตัวละคร";
            playerImages[slot].gameObject.SetActive(false);
        }
    }

    void OnSelectSlot(int slot)
    {
        if (isLoading) return;
        isLoading = true;

        // เช็คว่าช่องนี้มี save จริงไหม จาก PlayerPrefs โดยตรง
        bool hasSave = PlayerPrefs.HasKey($"player{slot}_name");

        // ตั้ง slot ที่เลือกไว้เสมอ
        PlayerManager.Instance.SelectSlot(slot);
        PlayerPrefs.SetInt("SelectedSlot", slot);
        PlayerPrefs.Save();

        // ตัดสินใจซีนจาก hasSave ไม่พึ่ง CurrentPlayer (กันค่าเก่าค้าง)
        string scene = hasSave ? "MainScene" : "SelectScene";
        Debug.Log($"[MenuUI] Slot {slot} hasSave={hasSave} → load {scene}");
        SceneManager.LoadScene(scene);
    }

    void OnDeleteSlot(int slot)
    {
        PlayerManager.Instance.DeletePlayer(slot);

        // ถ้าลบช่องที่เลือกอยู่ ให้ล้าง SelectedSlot และกัน CurrentPlayer ค้าง
        int selected = PlayerPrefs.GetInt("SelectedSlot", -1);
        if (selected == slot)
        {
            PlayerPrefs.DeleteKey("SelectedSlot");
            if (PlayerManager.Instance.GetCurrentSlot() == slot)
            {
                // ให้เลือกช่อง -1 ชั่วคราว
                PlayerManager.Instance.SelectSlot(slot); // จะเซ็ต CurrentPlayer = null อยู่แล้ว
            }
        }

        PlayerPrefs.Save();
        UpdateSlotUI(slot);
        Debug.Log($"[MenuUI] Deleted slot {slot}. UI refreshed.");
    }
}
