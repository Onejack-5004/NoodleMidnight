
using UnityEngine;

public class ServingStation : MonoBehaviour, IInteractable
{
    [Header("References")]
    public CookingManager cookingManager;
    public CustomerManager customerManager;
    public InventoryManager inventoryManager;

    [Header("Station Points")]
    public Transform servingPoint;
    public Transform soupBagPoint;

    public void Interact()
    {
        if (cookingManager == null ||
            servingPoint == null ||
            soupBagPoint == null ||
            customerManager == null ||
            inventoryManager == null)
        {
            Debug.LogError("ServingStation references are missing!");
            return;
        }

        // ขั้นตอนที่ 1: วางแพ็กเกจเส้น
        if (!cookingManager.IsPlatePlaced())
        {
            bool success =
                cookingManager.PlaceBowlOnStation(servingPoint);

            if (success)
                inventoryManager.OnBowlPlaced();

            return;
        }

        // ขั้นตอนที่ 2: วางถุงน้ำซุป
        if (!cookingManager.IsSoupBagPlaced())
        {
            bool success =
                cookingManager.PlaceSoupBagOnStation(soupBagPoint);

            if (success)
                inventoryManager.OnSoupBagPlaced();

            return;
        }

        // ขั้นตอนที่ 3: เสิร์ฟลูกค้า
        cookingManager.CompleteServing();
        customerManager.TryServe(cookingManager);
    }
}
