using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneLoader : MonoBehaviour
{
    public Animator animator;
    private static SceneLoader instance;
    private string sceneToLoad;

    void Awake()
    {
        // ทำให้ตัวเดียวอยู่ข้ามซีนได้
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static void FadeToScene(string sceneName)
    {
        if (instance == null)
        {
            Debug.LogWarning("⚠️ ไม่มี SceneFader ในฉากนี้");
            SceneManager.LoadScene(sceneName);
            return;
        }

        instance.sceneToLoad = sceneName;
        instance.animator.SetTrigger("FadeOut"); // เริ่ม fade out
    }

    // ใช้ใน animation event
    public void OnFadeComplete()
    {
        SceneManager.LoadScene(sceneToLoad);
        animator.SetTrigger("FadeIn"); // fade in ตอนเข้าซีนใหม่
    }
   
}
