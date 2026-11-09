
using UnityEngine;
using TMPro;

public class OrderUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject orderPanel;
    public TextMeshProUGUI orderText;

    private string currentNoodle;
    private string currentSoup;
    private string currentTopping;

    public void ShowOrder(string noodle, string soup, string topping)
    {
        if (orderPanel == null || orderText == null)
            return;

        currentNoodle = noodle;
        currentSoup = soup;
        currentTopping = topping;

        UpdateOrderText("Waiting...");

        orderPanel.SetActive(true);
    }

    public void SetStatus(string status)
    {
        if (orderPanel == null || orderText == null)
            return;

        UpdateOrderText(status);
    }

    void UpdateOrderText(string status)
    {
        orderText.text =
            "CUSTOMER ORDER\n\n" +
            "Menu: Clear Soup Noodles\n" +
            "Noodle: " + currentNoodle + "\n" +
            "Soup: " + currentSoup + "\n" +
            "Topping: " + currentTopping + "\n\n" +
            "Status: " + status;
    }

    public void HideOrder()
    {
        if (orderPanel != null)
            orderPanel.SetActive(false);
    }
}
