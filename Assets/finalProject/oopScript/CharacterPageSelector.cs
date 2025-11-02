using UnityEngine;
using UnityEngine.UI;
using System;

public class CharacterPageSelector : MonoBehaviour
{
    [Header("UI")]
    public RawImage[] slots;            // ช่องแสดงภาพ 3 รูป
    public Button[] slotButtons;        // ปุ่มกดเลือกในแต่ละช่อง
    public Texture[] characterImages;   // ภาพตัวละครทั้งหมด
    public Button nextPageButton;
    public Button prevPageButton;

    [Header("Preview")]
    public RawImage selectedCharacterImage;  // ไอคอนตัวละครที่เลือก (Preview)

    private int pageIndex = 0;
    private int charactersPerPage = 3;

    private int selectedCharacterIndex = -1;

    // แจ้งเตือนเมื่อเลือกตัวละคร
    public event Action<int> OnSelectionChanged;

    public bool HasSelection => selectedCharacterIndex >= 0;

    void Start()
    {
        UpdatePage();

        if (nextPageButton) nextPageButton.onClick.AddListener(NextPage);
        if (prevPageButton) prevPageButton.onClick.AddListener(PreviousPage);

        for (int i = 0; i < slotButtons.Length; i++)
        {
            int localIndex = i;
            if (slotButtons[i])
                slotButtons[i].onClick.AddListener(() => SelectCharacter(localIndex));
        }

        // ซ่อน Preview ไว้ก่อนจนกว่าจะเลือก
        if (selectedCharacterImage) selectedCharacterImage.gameObject.SetActive(false);
    }

    void UpdatePage()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            int characterIndex = pageIndex * charactersPerPage + i;
            bool has = (characterIndex < characterImages.Length);

            if (slots[i]) slots[i].gameObject.SetActive(has);
            if (slotButtons[i]) slotButtons[i].gameObject.SetActive(has);

            if (has && slots[i]) slots[i].texture = characterImages[characterIndex];
        }
    }

    void NextPage()
    {
        pageIndex++;
        int maxPage = Mathf.CeilToInt((float)characterImages.Length / charactersPerPage) - 1;
        if (pageIndex > maxPage) pageIndex = 0;
        UpdatePage();
    }

    void PreviousPage()
    {
        pageIndex--;
        int maxPage = Mathf.CeilToInt((float)characterImages.Length / charactersPerPage) - 1;
        if (pageIndex < 0) pageIndex = maxPage;
        UpdatePage();
    }

    void SelectCharacter(int localIndex)
    {
        selectedCharacterIndex = pageIndex * charactersPerPage + localIndex;
        Debug.Log("เลือกตัวละคร index: " + selectedCharacterIndex);

        // อัปเดต Preview + โชว์
        if (selectedCharacterImage)
        {
            selectedCharacterImage.texture = GetSelectedCharacterTexture();
            selectedCharacterImage.gameObject.SetActive(true);
        }

        // แจ้งผู้ฟังว่าเลือกแล้ว
        OnSelectionChanged?.Invoke(selectedCharacterIndex);
    }

    public int GetSelectedCharacterIndex() => selectedCharacterIndex;

    public Texture GetSelectedCharacterTexture()
    {
        if (selectedCharacterIndex >= 0 && selectedCharacterIndex < characterImages.Length)
            return characterImages[selectedCharacterIndex];
        return null;
    }
}
