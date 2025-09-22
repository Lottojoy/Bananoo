using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player : MonoBehaviour
{
    public string name;
    public GameObject model;
    public int streakDays;
    public Lesson currentLesson;

    public void UpdateStreak()
    {
        streakDays++;
    }

    public void SetCurrentLesson(Lesson lesson)
    {
        currentLesson = lesson;
    }
}
