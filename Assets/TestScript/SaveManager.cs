using UnityEngine;

public static class SaveManager
{
    /*// Save Player
    public static void SavePlayer(int slot, Player data)
    {
        PlayerPrefs.SetString("player" + slot + "_name", data.playerName);
        PlayerPrefs.SetInt("player" + slot + "_charIndex", data.characterIndex);
        PlayerPrefs.SetInt("player" + slot + "_streak", data.streakDays);
        PlayerPrefs.SetInt("player" + slot + "_lesson", data.currentLessonIndex);
        PlayerPrefs.Save(); // บังคับบันทึก
    }

    // Load Player
    public static Player LoadPlayer(int slot)
    {
        if (PlayerPrefs.HasKey("player" + slot + "_name"))
        {
            Player data = new Player();
            data.playerName = PlayerPrefs.GetString("player" + slot + "_name");
            data.characterIndex = PlayerPrefs.GetInt("player" + slot + "_charIndex");
            data.streakDays = PlayerPrefs.GetInt("player" + slot + "_streak");
            data.currentLessonIndex = PlayerPrefs.GetInt("player" + slot + "_lesson");
            return data;
        }
        return null; // ยังไม่มีเซฟ
    }*/
}
