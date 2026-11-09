
using UnityEngine;

public class BowlPickupStation : MonoBehaviour, IInteractable
{
    [Header("Bowl Settings")]
    public GameObject bowlPrefab;
    public Transform holdPoint;

    [Header("References")]
    public CookingManager cookingManager;
    public InventoryManager inventoryManager;

    public void Interact()
    {
        if (cookingManager == null ||
            inventoryManager == null ||
            bowlPrefab == null ||
            holdPoint == null)
        {
            Debug.LogError("BowlPickupStation references are missing!");
            return;
        }

        // ตรวจสอบว่ามีจานในกระเป๋าหรือไม่
        if (!inventoryManager.CanPickupNewBowl)
        {
            Debug.Log("You already have a bowl!");
            return;
        }

        GameObject newBowl = Instantiate(bowlPrefab);

        newBowl.transform.SetParent(holdPoint);
        newBowl.transform.localPosition = Vector3.zero;
        newBowl.transform.localRotation = Quaternion.identity;

        // ปิด Collider ของจาน
        Collider[] colliders =
            newBowl.GetComponentsInChildren<Collider>();

        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

        // ลงทะเบียนจานใน CookingManager ก่อน
        cookingManager.SetCurrentBowl(newBowl);

        if (cookingManager.currentBowl != newBowl)
        {
            Destroy(newBowl);
            return;
        }

        // ส่งจานเข้า Inventory
        if (!inventoryManager.RegisterNewBowl(newBowl))
        {
            Debug.LogError("Could not register bowl in inventory!");
            return;
        }

        Debug.Log("Picked up a new bowl!");
    }
}
