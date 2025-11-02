using UnityEngine;

public class AdminHotkey : MonoBehaviour
{
    [Tooltip("กดคีย์นี้เพื่อข้ามคูลดาวน์ (พร้อมนับทันที)")]
    public KeyCode hotkey = KeyCode.F9;

    void Update()
    {
        if (Input.GetKeyDown(hotkey) && StreakManager.Instance)
        {
            StreakManager.Instance.AdminSkipCooldown();
            Debug.Log("[Admin] Skip cooldown -> Ready now");
        }
    }
}
