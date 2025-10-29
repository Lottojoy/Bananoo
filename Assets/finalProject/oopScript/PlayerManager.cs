using UnityEngine;
using System;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public Player CurrentPlayer { get; private set; }
    private int currentSlot = 0;

    [Header("Options")]
    [Tooltip("ถ้า true และช่องว่าง จะสร้าง Guest อัตโนมัติ")]
    public bool autoCreateGuestIfEmpty = true;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // ดึง slot ล่าสุดที่ใช้ (ถ้าไม่เคยตั้ง จะเป็น 0)
        currentSlot = PlayerPrefs.GetInt("CurrentSlot", 0);

        // โหลดผู้เล่นทันทีตั้งแต่ Awake (ให้ UI อ่านได้)
        CurrentPlayer = LoadPlayer(currentSlot);

        if (CurrentPlayer == null && autoCreateGuestIfEmpty)
        {
            // ช่องว่าง → สร้าง Guest เริ่มต้น (progress=1)
            CreatePlayer("Guest", 0);
            Debug.Log($"[PlayerManager] Auto-created Guest at slot {currentSlot}");
        }

        // กัน progress < 1 (กรณีคีย์หาย/ไม่เคยเซฟ)
        if (CurrentPlayer != null && CurrentPlayer.currentLessonID < 1)
        {
            CurrentPlayer.currentLessonID = 1;
            SavePlayer(currentSlot, CurrentPlayer);
        }

        Debug.Log($"[PlayerManager] Awake: slot={currentSlot}, " +
                  $"player={(CurrentPlayer != null ? CurrentPlayer.playerName : "null")}, " +
                  $"progress={(CurrentPlayer != null ? CurrentPlayer.currentLessonID : 0)}");
    }

    public void SelectSlot(int slot)
    {
        currentSlot = slot;
        PlayerPrefs.SetInt("CurrentSlot", currentSlot);

        CurrentPlayer = LoadPlayer(slot);
        if (CurrentPlayer == null && autoCreateGuestIfEmpty)
        {
            CreatePlayer("Guest", 0);
        }

        if (CurrentPlayer != null && CurrentPlayer.currentLessonID < 1)
        {
            CurrentPlayer.currentLessonID = 1;
            SavePlayer(currentSlot, CurrentPlayer);
        }

        Debug.Log($"[PlayerManager] Selected slot {slot}, " +
                  $"CurrentPlayer: {(CurrentPlayer != null ? CurrentPlayer.playerName : "null")}, " +
                  $"progress={(CurrentPlayer != null ? CurrentPlayer.currentLessonID : 0)}");
    }

    public void CreatePlayer(string name, int characterIndex)
    {
        CurrentPlayer = new Player(name, characterIndex);
        EnsureProgressMin(CurrentPlayer);
        SavePlayer(currentSlot, CurrentPlayer);
        Debug.Log($"[PlayerManager] CreatePlayer - slot={currentSlot}, name={name}, charIndex={characterIndex}, progress={CurrentPlayer.currentLessonID}");
    }

    public void SavePlayer(int slot, Player player)
    {
        PlayerPrefs.SetString($"player{slot}_name", player.playerName);
        PlayerPrefs.SetInt($"player{slot}_charIndex", player.characterIndex);
        PlayerPrefs.SetInt($"player{slot}_streak", player.streakDays);
        PlayerPrefs.SetInt($"player{slot}_lesson", Mathf.Max(1, player.currentLessonID)); // กันต่ำกว่า 1
        PlayerPrefs.SetString($"player{slot}_lastPlayed", player.lastPlayedDate);
        PlayerPrefs.Save();

        Debug.Log($"[PlayerManager] SavePlayer - slot={slot}, name={player.playerName}, charIndex={player.characterIndex}, streak={player.streakDays}, lesson={player.currentLessonID}");
    }

    public Player LoadPlayer(int slot)
    {
        if (!PlayerPrefs.HasKey($"player{slot}_name"))
        {
            Debug.Log($"[PlayerManager] LoadPlayer - slot {slot} empty");
            return null;
        }

        Player player = new Player(
            PlayerPrefs.GetString($"player{slot}_name"),
            PlayerPrefs.GetInt($"player{slot}_charIndex")
        );

        player.streakDays = PlayerPrefs.GetInt($"player{slot}_streak");
        // ถ้าคีย์ lesson ยังไม่เคยมี จะได้ 0 → ใส่ default เป็น 1
        player.currentLessonID = PlayerPrefs.GetInt($"player{slot}_lesson", 1);
        player.lastPlayedDate  = PlayerPrefs.GetString($"player{slot}_lastPlayed", DateTime.Now.ToString("yyyy-MM-dd"));

        EnsureProgressMin(player);

        Debug.Log($"[PlayerManager] LoadPlayer - slot={slot}, name={player.playerName}, charIndex={player.characterIndex}, streak={player.streakDays}, lesson={player.currentLessonID}");
        return player;
    }

    public void DeletePlayer(int slot)
    {
        PlayerPrefs.DeleteKey($"player{slot}_name");
        PlayerPrefs.DeleteKey($"player{slot}_charIndex");
        PlayerPrefs.DeleteKey($"player{slot}_streak");
        PlayerPrefs.DeleteKey($"player{slot}_lesson");
        PlayerPrefs.DeleteKey($"player{slot}_lastPlayed");

        if (CurrentPlayer != null && slot == currentSlot) CurrentPlayer = null;

        Debug.Log($"[PlayerManager] DeletePlayer - slot={slot}, CurrentPlayer cleared={(CurrentPlayer == null)}");
    }

    public int GetCurrentSlot() => currentSlot;

    // ===== Helpers =====
    private void EnsureProgressMin(Player p)
    {
        if (p.currentLessonID < 1) p.currentLessonID = 1;
    }

    // เรียกตอนผ่านด่าน: อัปเดต progress และเซฟในที่เดียว
    public void OnLessonCleared(int clearedLessonID)
    {
        if (CurrentPlayer == null) return;
        int next = clearedLessonID + 1;
        if (CurrentPlayer.currentLessonID < next)
        {
            CurrentPlayer.currentLessonID = next;
            SavePlayer(currentSlot, CurrentPlayer);
            Debug.Log($"[PlayerManager] Progress updated to {CurrentPlayer.currentLessonID} after clearing {clearedLessonID}");
        }
    }
}
