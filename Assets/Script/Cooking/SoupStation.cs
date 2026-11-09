
using UnityEngine;

public class SoupStation : MonoBehaviour, IInteractable
{
    [Header("Soup Bag Settings")]
    public GameObject soupBagPrefab;

    [Header("References")]
    public CookingManager cookingManager;
    public InventoryManager inventoryManager;

    public void Interact()
    {
        if (soupBagPrefab == null ||
            cookingManager == null ||
            inventoryManager == null)
        {
            Debug.LogError("SoupStation references are missing!");
            return;
        }

        if (!cookingManager.IsPlatePlaced())
        {
            Debug.Log("Place the noodle package first!");
            return;
        }

        if (cookingManager.HasSoupBag())
        {
            Debug.Log("You already have a soup bag!");
            return;
        }

        if (!inventoryManager.CanPickupNewSoupBag)
        {
            Debug.Log("Your item slot is occupied!");
            return;
        }

        GameObject newSoupBag = Instantiate(soupBagPrefab);

        Collider[] colliders =
            newSoupBag.GetComponentsInChildren<Collider>();

        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

        // ลงทะเบียนถุงน้ำซุปใน CookingManager
        bool success = cookingManager.SetCurrentSoupBag(newSoupBag);

        if (!success)
        {
            Destroy(newSoupBag);
            return;
        }

        // ส่งถุงน้ำซุปเข้า Inventory
        if (!inventoryManager.RegisterNewSoupBag(newSoupBag))
        {
            Debug.LogError("Could not register soup bag!");
            return;
        }

        Debug.Log("Picked up a soup bag!");
    }
}
