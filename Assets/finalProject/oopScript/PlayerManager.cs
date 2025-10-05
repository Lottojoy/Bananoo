using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public Player CurrentPlayer { get; private set; }
    private int currentSlot = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SelectSlot(int slot)
    {
        currentSlot = slot;
        CurrentPlayer = LoadPlayer(slot);
    }

    public void CreatePlayer(string name, int characterIndex)
    {
        CurrentPlayer = new Player(name, characterIndex);
        SavePlayer(currentSlot, CurrentPlayer);
    }

    public void SavePlayer(int slot, Player player)
    {
        PlayerPrefs.SetString($"player{slot}_name", player.playerName);
        PlayerPrefs.SetInt($"player{slot}_charIndex", player.characterIndex);
        PlayerPrefs.SetInt($"player{slot}_streak", player.streakDays);
        PlayerPrefs.SetString($"player{slot}_lesson", player.currentLessonID);
        PlayerPrefs.SetString($"player{slot}_lastPlayed", player.lastPlayedDate);
        PlayerPrefs.Save();
    }

    public Player LoadPlayer(int slot)
    {
        if (!PlayerPrefs.HasKey($"player{slot}_name")) return null;

        Player player = new Player(
            PlayerPrefs.GetString($"player{slot}_name"),
            PlayerPrefs.GetInt($"player{slot}_charIndex")
        );
        player.streakDays = PlayerPrefs.GetInt($"player{slot}_streak");
        player.currentLessonID = PlayerPrefs.GetString($"player{slot}_lesson");
        player.lastPlayedDate = PlayerPrefs.GetString($"player{slot}_lastPlayed");
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
    }

    public int GetCurrentSlot() => currentSlot;
}
