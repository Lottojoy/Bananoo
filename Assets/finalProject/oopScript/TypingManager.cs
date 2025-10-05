using UnityEngine;

public class TypingManager : MonoBehaviour
{
    public LessonWordUI uiManager;

    private Lesson currentLesson;
    private string lessonContent;
    private int currentIndex = 0;
    private int correctCount = 0;

    private float startTime;
    private float endTime;

    void Start()
    {
        currentLesson = GameManager.Instance.CurrentLesson;

        if (currentLesson == null)
        {
            Debug.LogError("No current lesson!");
            return;
        }

        lessonContent = currentLesson.GetText();
        uiManager.SetLessonText(lessonContent);

        startTime = Time.time;
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            foreach (char c in Input.inputString)
                CheckInput(c);
        }
    }

    void CheckInput(char c)
    {
        if (currentIndex >= lessonContent.Length) return;

        char expected = lessonContent[currentIndex];

        if (c == expected)
        {
            correctCount++;
            currentIndex++;
            uiManager.UpdateTypingProgress(currentIndex);
        }
        else
        {
            uiManager.ShowErrorEffect();
        }

        if (currentIndex >= lessonContent.Length)
            FinishLesson();
    }

    void FinishLesson()
    {
        endTime = Time.time;
        float timeUsed = endTime - startTime;
        float accuracy = (float)correctCount / lessonContent.Length;
        float wpm = (correctCount / 5f) / (timeUsed / 60f);

        uiManager.ShowResult(wpm, accuracy, timeUsed);

        GameManager.Instance.FinishLesson(currentLesson.LessonID, accuracy);
    }
}
