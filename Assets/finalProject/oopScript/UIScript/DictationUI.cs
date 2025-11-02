using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DictationUI : MonoBehaviour
{
    [Header("Header / Prompt")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text promptText;

    [Header("Controls")]
    [SerializeField] private Button playButton;
    [SerializeField] private TMP_Text playButtonLabel; // “Play / Replay”
    [SerializeField] private TMP_InputField _answerInput;
    [SerializeField] private Button submitButton;

    [Header("Live Stats (on lesson)")]
    [SerializeField] private TMP_Text timeLiveText;    // ⏱ เวลาที่กำลังนับ

    [Header("Popup Result")]
    [SerializeField] private GameObject popupRoot;
    [SerializeField] private TMP_Text popupYourLine;
    [SerializeField] private TMP_Text popupCorrectLine;
    [SerializeField] private TMP_Text popupWpmText;    // WPM ตอนสรุป
    [SerializeField] private TMP_Text popupAccText;    // ACC ตอนสรุป
    [SerializeField] private TMP_Text popupTimeText;   // Time ตอนสรุป
    [SerializeField] private Button popupNextButton;
    [SerializeField] private Button popupCloseButton;

    public TMP_InputField answerInput => _answerInput;

    // ---------- Header ----------
    public void SetTitle(string t)  { if (titleText)  titleText.text  = t ?? ""; }
    public void SetPrompt(string t) { if (promptText) promptText.text = t ?? ""; }

    // ---------- Play Button UI ----------
    public void UpdatePlayButtonLabel(bool hasPlayed)
    {
        if (playButtonLabel)
            playButtonLabel.text = hasPlayed ? "Replay" : "Play";
    }
    public void UpdatePlayState(bool isPlaying, bool hasPlayed)
    {
        if (playButtonLabel)
            playButtonLabel.text = isPlaying ? "Playing..." : (hasPlayed ? "Replay" : "Play");
    }

    // เปิด/ปิดเฉพาะปุ่ม Play
    public void SetPlayEnabled(bool v)
    {
        if (playButton) playButton.interactable = v;
    }

    // เปิด/ปิดเฉพาะช่องพิมพ์ (อนุญาตให้พิมพ์ระหว่างเล่นเสียงได้)
    public void SetInputEnabled(bool v)
    {
        if (_answerInput) _answerInput.interactable = v;
    }

    // ซ่อน/แสดงปุ่ม Submit (ล่องหนจริง ๆ)
    public void SetSubmitActive(bool active)
    {
        if (submitButton) submitButton.gameObject.SetActive(active);
    }

    // เปิด/ปิดการกดปุ่ม Submit (ถ้าต้องการเฉย ๆ)
    public void SetSubmitEnabled(bool v)
    {
        if (submitButton) submitButton.interactable = v;
    }

    // ---------- Live time ----------
    public void SetLiveTime(float seconds)
    {
        if (!timeLiveText) return;
        int m = Mathf.FloorToInt(seconds / 60f);
        float s = seconds - m * 60f;
        timeLiveText.text = $"{m:00}:{s:00.0}";
    }

    // ---------- Answer / Submit ----------
    public void ClearAnswer() { if (_answerInput) _answerInput.text = ""; }

    // (เดิม) คุมทั้งสามอย่างพร้อมกัน — ถ้าจะใช้ต่อก็ได้ แต่ตอนนี้เราแยกเมธอดแล้ว
    public void SetInteractable(bool v)
    {
        if (playButton)   playButton.interactable   = v;
        if (submitButton) submitButton.interactable = v;
        if (_answerInput) _answerInput.interactable = v;
    }

    // Hook onValueChanged ของ Input
    public void WireAnswerChanged(System.Action<string> onChanged)
    {
        if (_answerInput)
        {
            _answerInput.onValueChanged.RemoveAllListeners();
            _answerInput.onValueChanged.AddListener(s => onChanged?.Invoke(s));
        }
    }

    // ---------- Popup ----------
    public void ShowPopupRich(string yourRich, string correctRich)
    {
        if (popupRoot) popupRoot.SetActive(true);
        if (popupYourLine)    popupYourLine.text    = $"คำตอบของคุณ: {yourRich}"  ?? "";
        if (popupCorrectLine) popupCorrectLine.text = $"เฉลย: {correctRich}" ?? "";
    }

    public void SetPopupStats(float wpm, float acc01, float timeSec)
    {
        if (popupWpmText)  popupWpmText.text  = $"WPM: {wpm:F1}";
        if (popupAccText)  popupAccText.text  = $"ACC: {acc01 * 100f:F1}%";
        if (popupTimeText)
        {
            int m = Mathf.FloorToInt(timeSec / 60f);
            float s = timeSec - m * 60f;
            popupTimeText.text = $"Time: {m:00}:{s:00.0}";
        }
    }

    public void HidePopup()
    {
        if (popupRoot) popupRoot.SetActive(false);
    }

    // ---------- Wire buttons ----------
    public void WirePlayButton(System.Action onClick)
    {
        if (playButton)
        {
            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(() => onClick?.Invoke());
        }
    }
    public void WireSubmitButton(System.Action onClick)
    {
        if (submitButton)
        {
            submitButton.onClick.RemoveAllListeners();
            submitButton.onClick.AddListener(() => onClick?.Invoke());
        }
    }
    public void WirePopupNextButton(System.Action onClick)
    {
        if (popupNextButton)
        {
            popupNextButton.onClick.RemoveAllListeners();
            popupNextButton.onClick.AddListener(() => onClick?.Invoke());
        }
    }
    public void WirePopupCloseButton(System.Action onClick)
    {
        if (popupCloseButton)
        {
            popupCloseButton.onClick.RemoveAllListeners();
            popupCloseButton.onClick.AddListener(() => onClick?.Invoke());
        }
    }
}
