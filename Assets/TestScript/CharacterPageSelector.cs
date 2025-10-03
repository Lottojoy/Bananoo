using UnityEngine;
using UnityEngine.UI;

public class CharacterPageSelector : MonoBehaviour
{
    [Header("UI")]
    public RawImage[] slots;            // ช่องแสดงภาพ 3 รูป
    public Button[] slotButtons;        // ปุ่มกดเลือกในแต่ละช่อง
    public Texture[] characterImages;   // ภาพตัวละครทั้งหมด
    public Button nextPageButton;
    public Button prevPageButton;
     public RawImage selectedCharacterImage;
    private int pageIndex = 0;
    private int charactersPerPage = 3;

    // เก็บ index ที่เลือก (global index ไม่ใช่แค่ 0–2)
    private int selectedCharacterIndex = -1;

    void Start()
    {
        UpdatePage();

        nextPageButton.onClick.AddListener(NextPage);
        prevPageButton.onClick.AddListener(PreviousPage);

        // ให้ปุ่มในแต่ละช่องกดเลือกตัวละครได้
        for (int i = 0; i < slotButtons.Length; i++)
        {
            int localIndex = i;
            slotButtons[i].onClick.AddListener(() => SelectCharacter(localIndex));
        }
    }

    void UpdatePage()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            int characterIndex = pageIndex * charactersPerPage + i;
            if (characterIndex < characterImages.Length)
            {
                slots[i].texture = characterImages[characterIndex];
                slots[i].gameObject.SetActive(true);
                slotButtons[i].gameObject.SetActive(true);
            }
            else
            {
                slots[i].gameObject.SetActive(false);
                slotButtons[i].gameObject.SetActive(false);
            }
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
         // อัปเดตภาพ preview
       
        if ( selectedCharacterImage.texture != null) selectedCharacterImage.texture = GetSelectedCharacterTexture();
    }

    public int GetSelectedCharacterIndex()
    {
        return selectedCharacterIndex;
    }

    public Texture GetSelectedCharacterTexture()
    {
        if (selectedCharacterIndex >= 0 && selectedCharacterIndex < characterImages.Length)
            return characterImages[selectedCharacterIndex];
        return null;
    }
}
