using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
     public StageButton stageButtonPrefab;
    public Transform[] slots;
    public Lesson[] lessons; // ลิสต์บทเรียนของ map นี้

    void Start()
    {
        for (int i = 0; i < Mathf.Min(slots.Length, lessons.Length); i++)
        {
            var go = Instantiate(stageButtonPrefab, slots[i]);
            //go.Initialize(lessons[i]); // ใส่ Lesson ให้ปุ่มตอนสร้าง
        }
    }
}
