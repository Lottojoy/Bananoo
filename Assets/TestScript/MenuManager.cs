using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
   /* public Button[] playerButtons;        // ปุ่มเลือกตัวละคร
    public TMP_Text[] playerButtonTexts;  // ข้อความใต้ปุ่ม
    public RawImage[] playerImages;       // ช่องภาพตัวละครของแต่ละ slot
    public Texture[] characterImages;     // ภาพตัวละครทั้งหมด (เหมือนที่ SelectScene ใช้)

    public Button[] deleteButtons;        // ปุ่มลบแต่ละ slot

    void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            int slot = i; 
            Player data = SaveManager.LoadPlayer(slot);

            if (data != null)
            {
                playerButtonTexts[i].text = $"{data.playerName}, {data.streakDays} วัน, บทเรียน {data.currentLessonIndex}";

                // แสดงภาพตัวละคร
                if (data.characterIndex >= 0 && data.characterIndex < characterImages.Length)
                {
                    playerImages[i].texture = characterImages[data.characterIndex];
                    playerImages[i].gameObject.SetActive(true);
                }
                else
                {
                    playerImages[i].gameObject.SetActive(false);
                }
            }
            else
            {
                playerButtonTexts[i].text = "ไม่มีบันทึกตัวละคร";
                playerImages[i].gameObject.SetActive(false); // ไม่มีข้อมูล → ซ่อน
            }

            // ปุ่มเลือก slot
            playerButtons[i].onClick.AddListener(() => OnPlayerButton(slot));

            // ปุ่มลบข้อมูล slot
            deleteButtons[i].onClick.AddListener(() => DeleteSlot(slot));
        }
    }

    void OnPlayerButton(int slot)
    {
        Player data = SaveManager.LoadPlayer(slot);
        PlayerPrefs.SetInt("CurrentSlot", slot);

        if (data != null)
        {
            SceneManager.LoadScene("MainScene");
        }
        else
        {
            SceneManager.LoadScene("SelectScene");
        }
    }

    void DeleteSlot(int slot)
    {
        // ลบข้อมูล slot ใน PlayerPrefs
        PlayerPrefs.DeleteKey($"player{slot}_name");
        PlayerPrefs.DeleteKey($"player{slot}_charIndex");
        PlayerPrefs.DeleteKey($"player{slot}_streak");
        PlayerPrefs.DeleteKey($"player{slot}_lesson");
        PlayerPrefs.Save();

        // อัปเดต UI
        playerButtonTexts[slot].text = "ไม่มีบันทึกตัวละคร";
        playerImages[slot].gameObject.SetActive(false);

        Debug.Log($"ลบข้อมูล slot {slot} แล้ว");
    }*/
}
