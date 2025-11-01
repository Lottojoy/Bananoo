using UnityEngine;
using TMPro;
using System.Text;
using System.Linq;

public class ResultUI : MonoBehaviour
{
    [SerializeField] private TMP_Text wpmText;
    [SerializeField] private TMP_Text accText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text timeText;

    [Header("Coaching")]
    [SerializeField] private TMP_Text praiseText;  // คำชม/กำลังใจ
    [SerializeField] private TMP_Text adviceText;  // คำแนะนำ

    // ชุดข้อความ (แก้ให้เข้าคาแรกเตอร์เกมได้)
    private static readonly string[] PRAISES = {
        "สุดยอด! ความแม่นยำคมกริบ แถมเร็วด้วย",
        "โหดมาก! จังหวะนิ้วดีมาก รักษามาตรฐานนี้ไว้เลย",
        "ดีมากๆ! ทั้งเร็วทั้งชัวร์ แบบนี้ไปต่อได้อีกไกล",
        "เฟี้ยวจริง! คุมจังหวะกับความถูกต้องได้เหลือเชื่อ",
        "เทพแล้วปะ! ตัวเลขสวยสะอาดตา นับถือเลย"
    };

    private static readonly string[] ENCOURAGES = {
        "ไม่เป็นไร เดินถูกทางแล้ว ลุยต่ออีกนิด!",
        "ชิลๆ ค่อยๆ ไหล เดี๋ยวก็เป็นจังหวะเอง",
        "วันนี้อาจยังไม่สุด แต่นายกำลังพัฒนาอยู่",
        "ผิดได้ แต่อย่าหยุดซ้อม เดี๋ยวมันจะเข้าที่เอง",
        "อย่าเพิ่งท้อ หนึ่งก้าวเล็กๆ รวมกันคือก้าวใหญ่"
    };

    void Start()
    {
        // ดึงคะแนน
        ScoreData data = null;
        var gdm = GameDataManager.Instance ?? FindObjectOfType<GameDataManager>(true);
        if (gdm != null) data = gdm.ScoreData;
        if (data == null) data = ResultContext.LastScore;

        if (data == null)
        {
            Debug.LogWarning("[ResultUI] No score found. Showing defaults.");
            SetBasics("-", "-", "-", "-");
            SetTexts("—", "—");
            return;
        }

        SetBasics(
            $"{data.WPM:F1}",
            $"{data.ACC:F1}%",
            $"{data.FinalScore:F0}",
            $"{data.TimeUsed:F1}s"
        );

        // ตัดสิน "คำชม" หรือ "กำลังใจ"
        bool isPraise = (data.ACC > 90f && data.WPM >= 30f);
        string msg1 = isPraise
            ? PRAISES[Random.Range(0, PRAISES.Length)]
            : ENCOURAGES[Random.Range(0, ENCOURAGES.Length)];

        // ถ้าเป็นคำชม → ไม่ต้องมีคำแนะนำ
        string msg2 = "";
        if (!isPraise)
        {
            // เงื่อนไขคำแนะนำหลัก
            if (data.ACC < 70f)
                msg2 = "คำแนะนำ:พิมพ์ผิดมากไป ต้องฝึกมากกว่านี้";
            else if (data.WPM < 20f)
                msg2 = "คำแนะนำ:พิมพ์ช้าเกินไป ควรพัฒนาความเร็ว";

            // เพิ่มคำแนะนำอิงตัวที่ผิดเกิน 5 ครั้ง
            if (data.WrongChars != null && data.WrongCounts != null && data.WrongChars.Count == data.WrongCounts.Count)
            {
                var hardOnes = new System.Collections.Generic.List<char>();
                for (int i = 0; i < data.WrongChars.Count; i++)
                {
                    if (data.WrongCounts[i] > 5) hardOnes.Add(data.WrongChars[i]);
                }

                if (hardOnes.Count > 0)
                {
                    // แสดงเป็นรายการสั้นๆ เช่น a, s, ;  (ไม่ใส่เว้นวรรคเยอะให้ดูสวย)
                    var list = string.Join(", ", hardOnes.Select(c => c.ToString()));
                    msg2 += (msg2.Length > 0 ? "\n" : "") + $"ควรไปฝึกกดอักขระ: {list}";
                }
            }

            if (string.IsNullOrEmpty(msg2))
                msg2 = "คำแนะนำ:โฟกัสความสม่ำเสมอ ลองซ้อมด่านเดิมอีกรอบแล้วค่อยเพิ่มความเร็ว";
        }

        SetTexts(msg1, isPraise ? "" : msg2);
    }

    private void SetBasics(string wpm, string acc, string score, string time)
    {
        if (wpmText)  wpmText.text  = $"WPM: {wpm}";
        if (accText)  accText.text  = $"ACC: {acc}";
        if (scoreText)scoreText.text= $"Score: {score}";
        if (timeText) timeText.text = $"Time: {time}";
    }

    private void SetTexts(string praise, string advice)
    {
        if (praiseText) praiseText.text = praise;
        if (adviceText)
        {
            adviceText.gameObject.SetActive(!string.IsNullOrEmpty(advice));
            adviceText.text = advice;
        }
    }

    public void BackToMenu()
    {
        GameDataManager.Instance?.ResetAll();
        GameFlowManager.Instance?.LoadMainMenu();
        // หรือ SceneManager.LoadScene("MenuScene");
    }
}
