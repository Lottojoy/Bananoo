using UnityEngine;
using System.Collections.Generic;

public class DataManager : MonoBehaviour
{
    //ข้อมูล Lesson ทั้งหมด
    public static DataManager Instance { get; private set; }

    private Dictionary<int, Lesson> lessons = new Dictionary<int, Lesson>();


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadAllLessons();
    }

    void LoadAllLessons()
    {
        Lesson[] allLessons = Resources.LoadAll<Lesson>("Lessons");
        foreach (var l in allLessons)
        {
            if (!lessons.ContainsKey(l.LessonID))
                lessons.Add(l.LessonID, l);
        }
    }

    public Lesson GetLessonByID(int id)
{
    if (lessons.TryGetValue(id, out Lesson lesson))
        return lesson;
    return null;
}
}
