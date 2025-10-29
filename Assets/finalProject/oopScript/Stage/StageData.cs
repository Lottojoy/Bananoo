[System.Serializable]
public class StageData
{
    public int lessonID;           // ให้เป็น int ตรงกับ Lesson.LessonID
    public string sceneName;       // ชื่อซีนที่จะเล่น
    public string infoText;

    public Lesson lessonAsset;     // ← ลาก Lesson.asset มาใส่ได้เลย (แนะนำ)
}