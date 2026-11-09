
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CookingProgressUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI stepTitle;
    public TextMeshProUGUI stepDescription;
    public TextMeshProUGUI stepCounter;
    public Image progressBarFill;

    // แสดงขั้นตอนที่ต้องทำต่อไป
    public void ShowStep(
        string title,
        string description,
        int currentStep,
        int totalSteps)
    {
        gameObject.SetActive(true);

        stepTitle.text = title;
        stepDescription.text = description;

        stepCounter.text =
            "STEP " + currentStep + " OF " + totalSteps;

        progressBarFill.fillAmount =
            (float)(currentStep - 1) / totalSteps;
    }

    // ซ่อน UI เมื่อไม่มีคำสั่งอาหาร
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
