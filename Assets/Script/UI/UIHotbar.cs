
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHotbar : MonoBehaviour
{
    [Header("Inventory")]
    public InventoryManager inventoryManager;

    [Header("Slot Images")]
    public Image slot1Image;
    public Image slot2Image;

    [Header("Item Names")]
    public TMP_Text slot1ItemText;
    public TMP_Text slot2ItemText;

    [Header("Slot Positions")]
    public float slot1CenterX = 0f;
    public float slot1LeftX = -46f;
    public float slot2RightX = 46f;

    [Header("Colors")]
    public Color normalColor = new Color(0.12f, 0.12f, 0.12f, 0.85f);
    public Color selectedColor = new Color(1f, 0.75f, 0.15f, 1f);

    void Update()
    {
        if (inventoryManager == null)
            return;

        UpdateHotbar();
    }

    void UpdateHotbar()
    {
        // ตรวจสอบว่ามีไอเทมในช่อง 2 หรือไม่
        bool hasItem =
            inventoryManager.HasBowlInInventory ||
            inventoryManager.HasSoupBagInInventory;

        // ซ่อนหรือแสดง Slot 2
        slot2Image.gameObject.SetActive(hasItem);

        // เลื่อนตำแหน่ง Slot 1
        RectTransform slot1Rect = slot1Image.rectTransform;

        Vector2 slot1Position = slot1Rect.anchoredPosition;
        slot1Position.x = hasItem ? slot1LeftX : slot1CenterX;
        slot1Rect.anchoredPosition = slot1Position;

        // ตำแหน่ง Slot 2
        RectTransform slot2Rect = slot2Image.rectTransform;

        Vector2 slot2Position = slot2Rect.anchoredPosition;
        slot2Position.x = slot2RightX;
        slot2Rect.anchoredPosition = slot2Position;

        // Slot 1: Gun
        slot1ItemText.text = "Gun";

        bool gunSelected =
            inventoryManager.currentSlot == InventoryManager.HeldSlot.Gun;

        slot1Image.color = gunSelected ? selectedColor : normalColor;

        // Slot 2: Bowl / Soup Bag
        string itemName = "Empty";

        if (inventoryManager.HasBowlInInventory)
            itemName = "Bowl";
        else if (inventoryManager.HasSoupBagInInventory)
            itemName = "Soup Bag";

        slot2ItemText.text = itemName;

        bool itemSelected =
            inventoryManager.currentSlot == InventoryManager.HeldSlot.Bowl ||
            inventoryManager.currentSlot == InventoryManager.HeldSlot.SoupBag;

        slot2Image.color = itemSelected ? selectedColor : normalColor;
    }
}
