using UnityEngine;

public class ButtonSound : MonoBehaviour
{
    private static ButtonSound instance;
    public AudioClip clickSound;     // เสียงคลิกปุ่ม
    [Range(0f, 1f)] public float volume = 1f;

    void Awake()
    {
        // ให้มีตัวเดียวในเกม
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static void PlayClick()
    {
        if (instance == null || instance.clickSound == null)
        {
            Debug.LogWarning(" GlobalButtonSound ไม่มี instance หรือ clip");
            return;
        }

        // สร้าง GameObject แยกไว้เล่นเสียง
        GameObject temp = new GameObject("TempButtonSound");
        DontDestroyOnLoad(temp);

        AudioSource src = temp.AddComponent<AudioSource>();
        src.clip = instance.clickSound;
        src.volume = instance.volume;
        src.loop = false;
        src.Play();

        // ลบตัวเองหลังเสียงเล่นจบ
        Object.Destroy(temp, instance.clickSound.length);
    }
}
