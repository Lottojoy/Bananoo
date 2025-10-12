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

    public int LessonID => lessonID;
    public LessonType Type => type;

    // คืนค่าข้อความรวมของบทเรียน
    public string GetText()
    {
        return type == LessonType.Character ? string.Join("", characters) : string.Join(" ", words);
    }
}
