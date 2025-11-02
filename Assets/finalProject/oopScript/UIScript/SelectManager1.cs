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
    public TMP_Text StatusText;

    private int slot;

    void Start()
    {
        slot = PlayerPrefs.GetInt("SelectedSlot", 0);

        // สถานะเริ่มต้น
        if (startButton) startButton.gameObject.SetActive(false);
        if (StatusText)  StatusText.text = "ให้เลือกตัวละครกับตั้งชื่อ";

        // subscribe การเปลี่ยนแปลง
        if (nameInput) nameInput.onValueChanged.AddListener(_ => RefreshUIState());
        if (pageSelector) pageSelector.OnSelectionChanged += _ => RefreshUIState();

        if (startButton) startButton.onClick.AddListener(OnStartClicked);

        // รีเฟรชครั้งแรก
        RefreshUIState();
    }

    private void RefreshUIState()
    {
        bool hasName = nameInput && !string.IsNullOrWhiteSpace(nameInput.text);
        bool hasChar = pageSelector && pageSelector.HasSelection;

        bool ready = hasName && hasChar;

        if (startButton) startButton.gameObject.SetActive(ready);
        if (StatusText)
            StatusText.text = ready ? "กดเพื่อบันทึก" : "ให้เลือกตัวละครกับตั้งชื่อ";
    }

    void OnStartClicked()
    {
        string playerName = nameInput ? nameInput.text : "";
        int charIndex = pageSelector ? pageSelector.GetSelectedCharacterIndex() : -1;

        if (string.IsNullOrWhiteSpace(playerName))
        {
            Debug.LogWarning("กรุณาใส่ชื่อผู้เล่น");
            RefreshUIState();
            return;
        }
        if (charIndex < 0)
        {
            Debug.LogWarning("กรุณาเลือกตัวละคร");
            RefreshUIState();
            return;
        }

        // สร้าง player ใหม่
        PlayerManager.Instance.SelectSlot(slot);
        PlayerManager.Instance.CreatePlayer(playerName, charIndex);

       SceneLoader.FadeToScene("MainScene");
    }
}
