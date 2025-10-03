using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    public StageMap[] maps;

    void Start()
    {
        // ผูกปุ่ม Map แต่ละปุ่ม
        foreach (var map in maps)
        {
            if (map.mapButton != null)
            {
                map.mapButton.onClick.AddListener(() => SpawnMap(map));
            }
        }

        // แสดง Map แรกเป็น Default
        if (maps.Length > 0)
            SpawnMap(maps[0]);
    }

    public void SpawnMap(StageMap map)
    {
        // ปิด Map เก่า (ลบปุ่ม Stage เดิม)
        foreach (var m in maps)
        {
            foreach (var parent in m.stageParents)
            {
                foreach (Transform t in parent)
                    Destroy(t.gameObject);
            }
        }

        // สร้างปุ่ม Stage ของ Map นี้
        for (int i = 0; i < map.stageParents.Length; i++)
        {
            
                StageButton btn = Instantiate(map.stagePrefabs[i], map.stageParents[i]);
               
        }

       
    }
}
