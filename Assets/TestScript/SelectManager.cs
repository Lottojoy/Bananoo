using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SelectManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_InputField nameInput;
    
    public Button startButton;

    public CharacterPageSelector pageSelector; // อ้างอิงจาก CharacterPageSelector

    private int slot = 0;

    void Start()
    {
        slot = PlayerPrefs.GetInt("CurrentSlot", 0);
        startButton.onClick.AddListener(SaveAndStart);
    }

    void SaveAndStart()
    {
        string playerName = nameInput.text;

        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogWarning("กรุณาใส่ชื่อผู้เล่น");
            return;
        }

        int selectedIndex = pageSelector.GetSelectedCharacterIndex();
        if (selectedIndex == -1)
        {
            Debug.LogWarning("กรุณาเลือกตัวละคร");
            return;
        }

       

        Player newPlayer = new Player()
        {
            playerName = playerName,
            characterIndex = selectedIndex,
            streakDays = 1,
            currentLessonIndex = 0
        };

        SaveManager.SavePlayer(slot, newPlayer);
        SceneManager.LoadScene("MainScene");
    }
}
