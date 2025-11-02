using UnityEngine;

public class AudioBg: MonoBehaviour
{
    public AudioClip clip;       // ไฟล์เสียงที่จะเล่น
    [Range(0f, 1f)] public float volume = 1f;
    public bool loop = true;
    public bool playOnStart = true;

    private AudioSource audioSource;
    private static AudioBg instance;

    void Awake()
    {
        // ทำให้มีแค่ตัวเดียวในเกม
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // อยู่ข้ามซีนได้

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.loop = loop;
        audioSource.volume = volume;
    }

    void Start()
    {
        if (playOnStart && clip != null)
            audioSource.Play();
    }

    void Update()
    {
        // อัปเดตระดับเสียงขณะเล่น (สามารถเปลี่ยนใน Inspector ได้ระหว่างเล่น)
        audioSource.volume = volume;
    }

    public void Play() => audioSource.Play();
    public void Stop() => audioSource.Stop();
}
