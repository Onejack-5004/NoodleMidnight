
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    public enum HeldSlot
    {
        None,
        Gun,
        Bowl,
        SoupBag
    }

    [Header("Gun")]
    public GameObject gunModel;
    public GunShooter gunShooter;

    [Header("Item Hold Points")]
    public Transform bowlHoldPoint;
    public Transform soupBagHoldPoint;

    [Header("Cooking")]
    public CookingManager cookingManager;

    private GameObject storedBowl;
    private GameObject storedSoupBag;

    public HeldSlot currentSlot = HeldSlot.None;
    public bool HasBowlInInventory => storedBowl != null;
    public bool HasSoupBagInInventory => storedSoupBag != null;

    public bool CanPickupNewBowl
    {
        get
        {
            return storedBowl == null &&
                   storedSoupBag == null &&
                   currentSlot != HeldSlot.Bowl &&
                   currentSlot != HeldSlot.SoupBag &&
                   cookingManager != null &&
                   cookingManager.currentBowl == null;
        }
    }

    public bool CanPickupNewSoupBag
    {
        get
        {
            return storedBowl == null &&
                   storedSoupBag == null &&
                   currentSlot != HeldSlot.Bowl &&
                   currentSlot != HeldSlot.SoupBag &&
                   cookingManager != null &&
                   cookingManager.currentSoupBag == null;
        }
    }

    void Start()
    {
        if (gunModel != null)
            gunModel.SetActive(false);

        if (gunShooter != null)
            gunShooter.SetEquipped(false);

        currentSlot = HeldSlot.None;
    }

    void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            ToggleGun();

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
            ToggleItem();
    }

    void ToggleGun()
    {
        if (currentSlot == HeldSlot.Gun)
        {
            SetGunEquipped(false);
            currentSlot = HeldSlot.None;

            Debug.Log("Gun stored!");
            return;
        }

        if (currentSlot == HeldSlot.Bowl ||
            currentSlot == HeldSlot.SoupBag)
        {
            StoreCurrentItem();
        }

        SetGunEquipped(true);
        currentSlot = HeldSlot.Gun;

        Debug.Log("Gun equipped!");
    }


    void ToggleItem()
    {
        // ถ้ากำลังถือไอเทมอยู่ ให้เก็บ
        if (currentSlot == HeldSlot.Bowl ||
            currentSlot == HeldSlot.SoupBag)
        {
            StoreCurrentItem();
            Debug.Log("Item stored!");
            return;
        }

        // ตรวจว่ามีไอเทมให้หยิบหรือไม่
        if (storedBowl == null && storedSoupBag == null)
        {
            Debug.Log("You don't have an item in Slot 2!");
            return;
        }

        // ถ้าถือปืนอยู่ ให้เก็บปืนก่อน
        if (currentSlot == HeldSlot.Gun)
        {
            SetGunEquipped(false);
            currentSlot = HeldSlot.None;
        }

        if (storedBowl != null)
        {
            EquipBowl();
            return;
        }

        if (storedSoupBag != null)
        {
            EquipSoupBag();
        }
    }


    void StoreCurrentItem()
    {
        if (currentSlot == HeldSlot.Bowl)
        {
            if (cookingManager == null ||
                cookingManager.currentBowl == null)
                return;

            storedBowl = cookingManager.currentBowl;

            storedBowl.transform.SetParent(transform);
            storedBowl.transform.localPosition = Vector3.zero;
            storedBowl.transform.localRotation = Quaternion.identity;
            storedBowl.SetActive(false);

            cookingManager.SetBowlHeld(false);
        }
        else if (currentSlot == HeldSlot.SoupBag)
        {
            if (cookingManager == null ||
                cookingManager.currentSoupBag == null)
                return;

            storedSoupBag = cookingManager.currentSoupBag;

            storedSoupBag.transform.SetParent(transform);
            storedSoupBag.transform.localPosition = Vector3.zero;
            storedSoupBag.transform.localRotation = Quaternion.identity;
            storedSoupBag.SetActive(false);

            cookingManager.SetSoupBagHeld(false);
        }

        currentSlot = HeldSlot.None;
    }

    public bool RegisterNewBowl(GameObject newBowl)
    {
        if (newBowl == null ||
            storedBowl != null ||
            storedSoupBag != null ||
            currentSlot == HeldSlot.Bowl ||
            currentSlot == HeldSlot.SoupBag ||
            cookingManager == null ||
            cookingManager.currentBowl != newBowl)
        {
            Debug.Log("Cannot register bowl! Check inventory or bowl state.");
            return false;
        }

        storedBowl = newBowl;

        if (currentSlot == HeldSlot.Gun)
            SetGunEquipped(false);

        EquipBowl();

        Debug.Log("Bowl added to inventory!");
        return true;
    }


    public bool RegisterNewSoupBag(GameObject newBag)
    {
        if (newBag == null ||
            storedBowl != null ||
            storedSoupBag != null ||
            currentSlot == HeldSlot.Bowl ||
            currentSlot == HeldSlot.SoupBag)
        {
            Debug.Log("You already have an item!");
            return false;
        }

        storedSoupBag = newBag;

        if (currentSlot == HeldSlot.Gun)
            SetGunEquipped(false);

        EquipSoupBag();

        Debug.Log("Soup bag added to inventory!");
        return true;
    }


    void EquipBowl()
    {
        if (storedBowl == null || bowlHoldPoint == null)
        {
            Debug.LogError("Bowl or Bowl Hold Point is missing!");
            return;
        }

        storedBowl.SetActive(true);
        storedBowl.transform.SetParent(bowlHoldPoint);
        storedBowl.transform.localPosition = Vector3.zero;
        storedBowl.transform.localRotation = Quaternion.identity;

        cookingManager.SetBowlHeld(true);

        currentSlot = HeldSlot.Bowl;

        Debug.Log("Bowl equipped!");
    }

    void EquipSoupBag()
    {
        if (storedSoupBag == null || soupBagHoldPoint == null)
        {
            Debug.LogError("Soup Bag or Soup Bag Hold Point is missing!");
            return;
        }

        storedSoupBag.SetActive(true);
        storedSoupBag.transform.SetParent(soupBagHoldPoint);
        storedSoupBag.transform.localPosition = Vector3.zero;
        storedSoupBag.transform.localRotation = Quaternion.identity;

        cookingManager.SetSoupBagHeld(true);

        currentSlot = HeldSlot.SoupBag;

        Debug.Log("Soup bag equipped!");
    }

    void SetGunEquipped(bool equipped)
    {
        if (gunModel != null)
            gunModel.SetActive(equipped);

        if (gunShooter != null)
            gunShooter.SetEquipped(equipped);
    }

    public void OnBowlPlaced()
    {
        storedBowl = null;
        currentSlot = HeldSlot.None;

        Debug.Log("Bowl removed from inventory!");
    }

    public void OnSoupBagPlaced()
    {
        storedSoupBag = null;
        currentSlot = HeldSlot.None;

        Debug.Log("Soup bag removed from inventory!");
    }

    public void ClearCookingItems()
    {
        storedBowl = null;
        storedSoupBag = null;

        if (currentSlot == HeldSlot.Bowl ||
            currentSlot == HeldSlot.SoupBag)
        {
            currentSlot = HeldSlot.None;
        }

        Debug.Log("Inventory cooking items cleared!");
    }

}
