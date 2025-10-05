using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadTypingScene(string lessonID)
    {
        GameDataManager.Instance.CurrentLessonID = lessonID;
        SceneManager.LoadScene("TypingScene");
    }

    public void LoadResultScene()
    {
        SceneManager.LoadScene("ResultScene");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
