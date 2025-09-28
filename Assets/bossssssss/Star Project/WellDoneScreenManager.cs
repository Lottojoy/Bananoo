using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WellDoneScreenManager : MonoBehaviour
{
    [Header("Star Settings")]
    [SerializeField] private Star[] Stars;

    [SerializeField] private float EnlargeScale = 1.5f;
    [SerializeField] private float ShrinkScale = 1f;
    [SerializeField] private float EnlargeDuration = 0.25f;
    [SerializeField] private float ShrinkDuration = 0.25f;

    private void Start()
    {
        int score = 87; // ใส่คะแนนตัวอย่างที่ได้จากเกม
        ShowStarsFromScore(score); // แสดงดาวตามคะแนน
    }

    /// <summary>
    /// แสดงดาวตามคะแนนที่ได้
    /// </summary>
    public void ShowStarsFromScore(int score)
    {
        int starCount = GetStarCountFromScore(score);
        StartCoroutine(ShowStarsRoutine(starCount));
    }

    /// <summary>
    /// แปลงคะแนนให้กลายเป็นจำนวนดาว (ปรับเงื่อนไขได้ตามเกม)
    /// </summary>
    private int GetStarCountFromScore(int score)
    {
        if (score >= 100) return 5;
        else if (score >= 80) return 4;
        else if (score >= 60) return 3;
        else if (score >= 40) return 2;
        else if (score > 0) return 1;
        else return 0;
    }

    /// <summary>
    /// แสดงดาวทีละดวงตามจำนวนที่ส่งเข้ามา
    /// </summary>
    private IEnumerator ShowStarsRoutine(int numberOfStars)
    {
        foreach (Star star in Stars)
        {
            star.YellowStar.gameObject.SetActive(true); // เผื่อโดนปิดไว้
            star.YellowStar.transform.localScale = Vector3.one * 0.01f; // เริ่มจากเล็กๆ
        }

        yield return null; // รอ 1 เฟรมให้ Unity อัปเดต

        for (int i = 0; i < numberOfStars && i < Stars.Length; i++)
        {
            yield return StartCoroutine(EnlargeAndShrinkStar(Stars[i]));
        }
    }

    /// <summary>
    /// Animation pop ดาว (ขยาย แล้วหด)
    /// </summary>
    private IEnumerator EnlargeAndShrinkStar(Star star)
    {
        yield return StartCoroutine(ChangeStarScale(star, EnlargeScale, EnlargeDuration));
        yield return StartCoroutine(ChangeStarScale(star, ShrinkScale, ShrinkDuration));
    }

    /// <summary>
    /// เปลี่ยนขนาดดาวแบบ Smooth
    /// </summary>
    private IEnumerator ChangeStarScale(Star star, float targetScale, float duration)
    {
        Vector3 initialScale = star.YellowStar.transform.localScale;
        Vector3 finalScale = new Vector3(targetScale, targetScale, targetScale);

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            star.YellowStar.transform.localScale = Vector3.Lerp(initialScale, finalScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        star.YellowStar.transform.localScale = finalScale;
    }
}
