using UnityEngine;
using TMPro;

public class PlayerStatusUI : MonoBehaviour
{
    [Header("References")]
    public CustomerManager customerManager;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI heartText;

    public void RefreshUI()
    {
        if (customerManager == null)
        {
            Debug.LogError("CustomerManager is missing!");
            return;
        }

        if (moneyText != null)
            moneyText.text = "THB " + customerManager.CurrentMoney.ToString("N0");

        if (heartText != null)
        {
            heartText.text = "";

            for (int i = 0; i < customerManager.CurrentHealth; i++)
            {
                heartText.text += "♥ ";
            }

            Debug.Log("Heart UI Updated: " + customerManager.CurrentHealth);
        }
        else
        {
            Debug.LogError("Heart Text is missing!");
        }
    }

    void Start()
    {
        RefreshUI();
    }
}