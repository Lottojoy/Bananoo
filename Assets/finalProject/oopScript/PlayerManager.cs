using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public Player CurrentPlayer { get; private set; }
    private int currentSlot = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("[PlayerManager] Awake - Instance initialized");
    }

    public void SelectSlot(int slot)
    {
        currentSlot = slot;
        CurrentPlayer = LoadPlayer(slot);
        Debug.Log($"[PlayerManager] Selected slot {slot}, CurrentPlayer: {(CurrentPlayer != null ? CurrentPlayer.playerName : "null")}");
    }

    public void CreatePlayer(string name, int characterIndex)
    {
        CurrentPlayer = new Player(name, characterIndex);
        SavePlayer(currentSlot, CurrentPlayer);
        Debug.Log($"[PlayerManager] CreatePlayer - slot={currentSlot}, name={name}, charIndex={characterIndex}");
    }

    public void SavePlayer(int slot, Player player)
    {
        PlayerPrefs.SetString($"player{slot}_name", player.playerName);
        PlayerPrefs.SetInt($"player{slot}_charIndex", player.characterIndex);
        PlayerPrefs.SetInt($"player{slot}_streak", player.streakDays);
        PlayerPrefs.SetInt($"player{slot}_lesson", player.currentLessonID);
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
        player.currentLessonID = PlayerPrefs.GetInt($"player{slot}_lesson");
        player.lastPlayedDate = PlayerPrefs.GetString($"player{slot}_lastPlayed");

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
}
