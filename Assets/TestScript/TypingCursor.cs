using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TypingCursor : MonoBehaviour
{/*
    public TMP_Text typingText;      // TMP Text ของ TypingManager
    public RectTransform cursor;     // Cursor UI (RawImage)
    public float moveSpeed = 15f;    // ความเร็ว cursor เคลื่อน
    public float blinkSpeed = 2f;    // ความเร็ว blink

    void Update()
    {
        if (typingText == null || cursor == null) return;

        // อัพเดต TMP mesh
        typingText.ForceMeshUpdate();
        TMP_TextInfo textInfo = typingText.textInfo;
        if (textInfo.characterCount == 0) return;

        // หา index ของ character ปัจจุบันจาก TypingManager
        int charIndex = GetCurrentCharIndex();
        if (charIndex < 0 || charIndex >= textInfo.characterCount) return;

        TMP_CharacterInfo charInfo = textInfo.characterInfo[charIndex];

        // Debug แสดงตำแหน่ง cursor และตัวอักษร
        Debug.Log($"Cursor charIndex: {charIndex}, Char: '{charInfo.character}'");

        // แปลง bottomLeft เป็น world position ของ cursor parent
        Vector3 charPos = typingText.transform.TransformPoint(charInfo.bottomLeft);
        cursor.position = Vector3.Lerp(cursor.position, charPos, Time.deltaTime * moveSpeed);

        // ปรับความสูง cursor ตามตัวอักษร
        float height = charInfo.ascender - charInfo.descender;
        cursor.sizeDelta = new Vector2(cursor.sizeDelta.x, height);

        // Blink cursor
        if (cursor.TryGetComponent<RawImage>(out RawImage img))
        {
            float alpha = Mathf.PingPong(Time.time * blinkSpeed, 1f);
            img.color = new Color(1f, 1f, 1f, alpha);
        }
    }

    int GetCurrentCharIndex()
    {
        var manager = typingText.GetComponentInParent<TypingManager>();
        if (manager == null) return 0;

        string input = manager.CurrentInput ?? "";
        string target = manager.CurrentTarget ?? "";
        if (string.IsNullOrEmpty(target)) return 0;

        int desiredIndex = Mathf.Min(input.Length, target.Length);

        TMP_TextInfo textInfo = typingText.textInfo;
        int visibleCount = 0;

        // วน TMP characterInfo เพื่อหา visible character ปัจจุบัน
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo c = textInfo.characterInfo[i];
            if (!c.isVisible) continue;

            if (visibleCount == desiredIndex)
                return i; // เจอตัวอักษรปัจจุบัน

            visibleCount++;
        }

        // fallback
        return textInfo.characterCount - 1;
    }*/
}
