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

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        currentSlot = PlayerPrefs.GetInt("CurrentSlot", 0);
        CurrentPlayer = LoadPlayer(currentSlot);

        if (CurrentPlayer == null && autoCreateGuestIfEmpty)
        {
            CreatePlayer("Guest", 0);
            Debug.Log($"[PlayerManager] Auto-created Guest at slot {currentSlot}");
        }

        if (CurrentPlayer != null && CurrentPlayer.currentLessonID < 1)
        {
            CurrentPlayer.currentLessonID = 1;
            SavePlayer(currentSlot, CurrentPlayer);
        }

        Debug.Log($"[PlayerManager] Awake: slot={currentSlot}, " +
                  $"player={(CurrentPlayer != null ? CurrentPlayer.playerName : "null")}, " +
                  $"progress={(CurrentPlayer != null ? CurrentPlayer.currentLessonID : 0)}");
    }

    // ⛑ เซฟกันพลาดตอนออก/พักแอป
    void OnApplicationQuit()
    {
        if (CurrentPlayer != null) SavePlayer(currentSlot, CurrentPlayer);
    }
    void OnApplicationPause(bool paused)
    {
        if (paused && CurrentPlayer != null) SavePlayer(currentSlot, CurrentPlayer);
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
        if (player == null) return;

        PlayerPrefs.SetString($"player{slot}_name", player.playerName);
        PlayerPrefs.SetInt   ($"player{slot}_charIndex", player.characterIndex);

        // —— streak fields ——
        PlayerPrefs.SetInt   ($"player{slot}_streakDays", player.streakDays);
        PlayerPrefs.SetString($"player{slot}_streakLastUtc",     player.streakLastClearUtc.ToString());
        PlayerPrefs.SetString($"player{slot}_streakNextEligUtc", player.streakNextEligibleUtc.ToString());
        PlayerPrefs.SetString($"player{slot}_streakResetAtUtc",  player.streakResetAtUtc.ToString());

        PlayerPrefs.SetInt($"player{slot}_lesson", Mathf.Max(1, player.currentLessonID));
        

        PlayerPrefs.Save(); // สำคัญ!

        Debug.Log($"[PlayerManager] SavePlayer - slot={slot}, name={player.playerName}, " +
                  $"charIndex={player.characterIndex}, streakDays={player.streakDays}, " +
                  $"last={player.streakLastClearUtc}, next={player.streakNextEligibleUtc}, reset={player.streakResetAtUtc}, " +
                  $"lesson={player.currentLessonID}");
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

        // —— streak fields ——
        player.streakDays = PlayerPrefs.GetInt($"player{slot}_streakDays", 0);
        player.streakLastClearUtc     = ReadLong($"player{slot}_streakLastUtc", 0);
        player.streakNextEligibleUtc  = ReadLong($"player{slot}_streakNextEligUtc", 0);
        player.streakResetAtUtc       = ReadLong($"player{slot}_streakResetAtUtc", 0);

        // ถ้ายังไม่เคยเซ็ต nextEligible → ให้พร้อมนับได้ทันที
        if (player.streakNextEligibleUtc <= 0)
            player.streakNextEligibleUtc = UtcNow();

        // progress
        player.currentLessonID = PlayerPrefs.GetInt($"player{slot}_lesson", 1);
        

        EnsureProgressMin(player);

        Debug.Log($"[PlayerManager] LoadPlayer - slot={slot}, name={player.playerName}, charIndex={player.characterIndex}, " +
                  $"streakDays={player.streakDays}, last={player.streakLastClearUtc}, next={player.streakNextEligibleUtc}, reset={player.streakResetAtUtc}, " +
                  $"lesson={player.currentLessonID}");
        return player;
    }

    public void DeletePlayer(int slot)
    {
        PlayerPrefs.DeleteKey($"player{slot}_name");
        PlayerPrefs.DeleteKey($"player{slot}_charIndex");

        PlayerPrefs.DeleteKey($"player{slot}_streakDays");
        PlayerPrefs.DeleteKey($"player{slot}_streakLastUtc");
        PlayerPrefs.DeleteKey($"player{slot}_streakNextEligUtc");
        PlayerPrefs.DeleteKey($"player{slot}_streakResetAtUtc");

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

    // —— utils for long in PlayerPrefs (เก็บเป็น string) ——
    static long ReadLong(string key, long def)
    {
        var s = PlayerPrefs.GetString(key, "");
        if (long.TryParse(s, out var v)) return v;
        return def;
    }

    static long UtcNow() =>
        (long)(DateTime.UtcNow - new DateTime(1970,1,1,0,0,0, DateTimeKind.Utc)).TotalSeconds;
}
