using UnityEngine;
using UnityEngine.UI;

public class Star : MonoBehaviour
{
    public Image YellowStar;

    private void Awake()
    {
        YellowStar = GetComponent<Image>();
        // อย่าตั้ง scale เป็น 0 ตรงนี้ — ให้ทำตอน ShowStars แทน
    }
}