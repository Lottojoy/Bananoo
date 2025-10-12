using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public StageButton stageButtonPrefab;
    public Transform buttonParent;
    public StageData[] stageDatas; // ใส่ผ่าน Inspector หรือ Load จาก ScriptableObject

    void Start()
    {
        foreach (var data in stageDatas)
        {
            StageButton btn = Instantiate(stageButtonPrefab, buttonParent);
            btn.Initialize(data); // ส่ง StageData เข้าไป
        }
    }
}
