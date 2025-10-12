using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SelectManager1 : MonoBehaviour
{
    [Header("UI")]
    public TMP_InputField nameInput;
    public Button startButton;
    public CharacterPageSelector pageSelector;

    private int slot;

    void Start()
    {
         slot = PlayerPrefs.GetInt("SelectedSlot", 0); 
        startButton.onClick.AddListener(OnStartClicked);
    }

    void OnStartClicked()
    {
        string playerName = nameInput.text;
        int charIndex = pageSelector.GetSelectedCharacterIndex();

        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogWarning("กรุณาใส่ชื่อผู้เล่น");
            return;
        }

        if (charIndex == -1)
        {
            Debug.LogWarning("กรุณาเลือกตัวละคร");
            return;
        }

        // สร้าง player ใหม่
        PlayerManager.Instance.SelectSlot(slot); // ตั้ง slot ปัจจุบัน
        PlayerManager.Instance.CreatePlayer(playerName, charIndex);     

        SceneManager.LoadScene("MainScene");
    }
}
