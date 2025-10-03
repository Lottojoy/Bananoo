using UnityEngine;
using UnityEngine.UI;
// ให้เห็นใน Inspector
[System.Serializable]
public class StageMap
{
    public Button mapButton;                     
    public Transform[] stageParents;    // แถววางปุ่มของ Map
    public StageButton[] stagePrefabs;  // ปุ่ม Stage ของ Map
}