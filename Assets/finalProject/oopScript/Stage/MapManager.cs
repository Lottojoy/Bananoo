using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapManager : MonoBehaviour
{
    [Header("Map Buttons")]
     public Button naxtMapButton;
      public Button prevMapButton;   
    public GameObject[] mapButtonsPrefab;       // ปุ่มเลือก Map ทั้งหมด
     public Transform[] mapButtonPositions;
     public TMP_Text textInfoMap;
    [Header("Stage Buttons")]
    public Transform[] stageButtonPositions;       // พาเรนต์สำหรับ Stage Button
    public GameObject[] stageButtonPrefab;      // Prefab Stage Button 20 ด่าน

    [Header("Settings")]
    public int mapsPerPage = 5;       // จำนวน Map ต่อหน้า
    public int stagesPerMap = 4;      // จำนวน Stage ต่อ Map

    private int currentMapPage = 0;   // แสดงหน้า Map ไหน
    private GameObject[] currentMapButtons;  // เก็บ Map Button ที่สร้างตอนนี้
    private void Awake()
    {
        naxtMapButton.onClick.AddListener(NextPage);
        prevMapButton.onClick.AddListener(PrevPage);

        GenerateMapButtons();
        UpdateMapButtons();
        OnMapClicked(1);
    }

    private void GenerateMapButtons()
    {
        // สร้าง array สำหรับ Map Button ปัจจุบัน
        currentMapButtons = new GameObject[mapsPerPage];

        for (int i = 0; i < mapsPerPage; i++)
        {
            GameObject go = Instantiate(mapButtonsPrefab[i], mapButtonPositions[i].position, Quaternion.identity, mapButtonPositions[i]);
            currentMapButtons[i] = go;
        }
    }

    private void UpdateMapButtons()
{
    // ปิด/เปิดปุ่ม Next / Prev ตามหน้า
    int totalPages = Mathf.CeilToInt((float)mapButtonsPrefab.Length / mapsPerPage);
    prevMapButton.interactable = currentMapPage > 0;
    naxtMapButton.interactable = currentMapPage < totalPages - 1;

    //Debug.Log($"=== currentMapPage: {currentMapPage} ===");

    // อัปเดต Map Button ในหน้านี้
    for (int i = 0; i < mapsPerPage; i++)
    {
        int mapIndex = currentMapPage * mapsPerPage + i;

        if (mapIndex < mapButtonsPrefab.Length)
        {
            if (currentMapButtons[i] != null)
                Destroy(currentMapButtons[i]); // ลบปุ่มเก่าก่อนสร้างใหม่

            currentMapButtons[i] = Instantiate(mapButtonsPrefab[mapIndex], mapButtonPositions[i].position, Quaternion.identity, mapButtonPositions[i]);
            //currentMapButtons[i].name = $"MapButton_{mapIndex + 1}";

            // กำหนด listener ของแต่ละปุ่ม
            int capturedIndex = mapIndex + 1; // ป้องกัน closure bug
            Button btn = currentMapButtons[i].GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnMapClicked(capturedIndex));
            currentMapButtons[i].SetActive(true);   
            //Debug.Log($"Map Button {capturedIndex} ถูกสร้างที่ index {i}");
        }
        else
        {
            if (currentMapButtons[i] != null)
                currentMapButtons[i].SetActive(false);
        }
    }
}

    private void NextPage()
    {
        currentMapPage++;
        UpdateMapButtons();
    }

    private void PrevPage()
    {
        currentMapPage--;
        UpdateMapButtons();
    }

    private void OnMapClicked(int mapID)
    {
        Debug.Log($"✅ เปิด Map {mapID}");
        textInfoMap.text = $"Map : ปัจจุบัน {mapID}";
        // ต่อไปจะสร้าง Stage Button ของ Map นี้
        // ลบ Stage Button เก่าออกก่อน
    foreach (Transform pos in stageButtonPositions)
    {
        foreach (Transform child in pos)
            Destroy(child.gameObject);
    }

    // คำนวณ Stage เริ่มต้นของ Map
    int startStageIndex = (mapID - 1) * stagesPerMap;

    // สร้าง Stage Button
    for (int i = 0; i < stagesPerMap; i++)
    {
        int stageIndex = startStageIndex + i;
        if (stageIndex >= stageButtonPrefab.Length) break; // ถ้าเกินจำนวน Stage ทั้งหมด

        // สร้าง Stage Button ที่ตำแหน่งของ Stage
        GameObject stageBtn = Instantiate(stageButtonPrefab[stageIndex], stageButtonPositions[i]);
        stageBtn.transform.localPosition = Vector3.zero; // Reset local position
        stageBtn.name = $"Stage_{stageIndex + 1}";

       

    }
    }
}
