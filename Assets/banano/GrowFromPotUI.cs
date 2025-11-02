using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowFromPotUI : MonoBehaviour
{
    public RectTransform plant;   // RectTransform ของ Image ตัวเขียว (ลูกของ PotMask)
    public float startY = -120f;  // ค่าตำแหน่งตอนเห็นแต่หัว (ปรับตามฉาก)
    public float endY = 0f;       // ค่าตำแหน่งตอนโผล่พอดี
    public float duration = 1.5f; // ระยะเวลาโต

    void OnEnable() { StartCoroutine(Grow()); }

    System.Collections.IEnumerator Grow()
    {
        float t = 0f;
        Vector2 p = plant.anchoredPosition;
        p.y = startY;
        plant.anchoredPosition = p;

        while (t < duration)
        {
            t += Time.deltaTime;
            float k = Mathf.SmoothStep(0f, 1f, t / duration); // นิ่มๆ
            p.y = Mathf.Lerp(startY, endY, k);
            plant.anchoredPosition = p;
            yield return null;
        }
        p.y = endY;
        plant.anchoredPosition = p;
    }
}

