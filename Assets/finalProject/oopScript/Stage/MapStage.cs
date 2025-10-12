using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class MapStage
{
    public string mapName;
    public StageData[] stages;           // 4 Stage ต่อ map
    public Vector3[] stagePositions;     // 4 ตำแหน่งวาง StageButton
}