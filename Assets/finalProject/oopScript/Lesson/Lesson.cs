using UnityEngine;

public enum LessonType { Character, Word }

[CreateAssetMenu(fileName = "NewLesson", menuName = "Lesson")]
public class Lesson : ScriptableObject
{
    [Header("Lesson Info")]
    [SerializeField] private int lessonID;
    [SerializeField] private LessonType type;

    [Header("Character Lesson")]
    [SerializeField] private string[] characters;

    [Header("Word Lesson")]
    [SerializeField] private string[] words;

    [Header("Info Lesson")]
    [SerializeField, TextArea] private string info;   // ← เปลี่ยนชื่อเป็นตัวเล็ก + TextArea

     // ✅ เพิ่มส่วนนี้
    [Header("Scene")]
    [Tooltip("ปล่อยว่างจะใช้ค่าเริ่มต้น: LessonWordScene")]
    [SerializeField] private string sceneNameOverride;

    public int LessonID => lessonID;
    public LessonType Type => type;
    public string InfoText => info;                   // ← getter ไว้ให้ StageButton เรียก

     // ✅ ถ้าไม่กรอก จะ fallback เป็น "LessonWordScene"
    public string SceneName => string.IsNullOrWhiteSpace(sceneNameOverride)
        ? "LessonWordScene"
        : sceneNameOverride;
        
    // คืนค่าข้อความรวมของบทเรียน
    public string GetText()
    {
        return type == LessonType.Character ? string.Join("", characters) : string.Join(" ", words);
    }
}
