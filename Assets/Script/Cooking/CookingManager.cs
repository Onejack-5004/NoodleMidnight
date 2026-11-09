using UnityEngine;

public class CookingManager : MonoBehaviour
{
    [Header("Current Plate")]
    public GameObject currentBowl;

    [Header("Current Soup Bag")]
    public GameObject currentSoupBag;

    private Transform noodlePoint;
    private Transform toppingPoint;
    private Transform packedVisual;

    private GameObject noodleVisual;
    private GameObject toppingVisual;

    private Renderer bowlRenderer;

    private bool hasBowl = false;
    private bool hasNoodle = false;
    private bool hasTopping = false;
    private bool isPlatePlaced = false;
    private bool hasSoupBag = false;
    private bool isSoupBagPlaced = false;

    private bool isBowlHeld = false;
    private bool isSoupBagHeld = false;

    [Header("Order Check")]
    public CustomerManager customerManager;


    [Header("Cooking Progress UI")]
    public CookingProgressUI progressUI;

    private bool previousOrderState = false;


    public bool CanPrepareFood()
    {
        if (customerManager == null)
        {
            Debug.LogError("Customer Manager is not assigned!");
            return false;
        }

        if (!customerManager.HasActiveOrder)
        {
            Debug.Log("Wait for the customer to place an order first!");
            return false;
        }

        return true;
    }

    public bool IsPlatePlaced()
    {
        return isPlatePlaced;
    }

    public bool IsSoupBagPlaced()
    {
        return isSoupBagPlaced;
    }

    public bool HasSoupBag()
    {
        return hasSoupBag;
    }
    public void SetBowlHeld(bool held)
    {
        if (!hasBowl || isPlatePlaced)
            return;

        isBowlHeld = held;
        RefreshCookingProgress();
    }
    public void SetSoupBagHeld(bool held)
    {
        if (!hasSoupBag || isSoupBagPlaced)
            return;

        isSoupBagHeld = held;
        RefreshCookingProgress();
    }

    public bool IsSoupBagHeld()
    {
        return isSoupBagHeld;
    }

    public bool IsBowlHeld()
    {
        return isBowlHeld;
    }

    public void SetCurrentBowl(GameObject newBowl)
    {
        if (!CanPrepareFood()) return;
        currentBowl = newBowl;

        if (currentBowl == null)
        {
            Debug.LogError("Bowl is missing!");
            return;
        }

        noodlePoint = FindChild(currentBowl.transform, "NoodlePoint");
        toppingPoint = FindChild(currentBowl.transform, "ToppingPoint");
        packedVisual = FindChild(currentBowl.transform, "PackedVisual");

        if (noodlePoint == null ||
            toppingPoint == null ||
            packedVisual == null)
        {
            Debug.LogError("Prefab references are missing!");
            return;
        }

        noodleVisual = FindChild(noodlePoint, "NoodleVisual")?.gameObject;
        toppingVisual = FindChild(toppingPoint, "ToppingVisual")?.gameObject;

        bowlRenderer = currentBowl.GetComponent<Renderer>();

        if (noodleVisual == null ||
            toppingVisual == null ||
            bowlRenderer == null)
        {
            Debug.LogError("Food visuals or Bowl Renderer are missing!");
            return;
        }

        noodleVisual.SetActive(false);
        toppingVisual.SetActive(false);
        packedVisual.gameObject.SetActive(false);
        bowlRenderer.enabled = true;

        hasBowl = true;
        isBowlHeld = true;
        hasNoodle = false;
        hasTopping = false;
        isPlatePlaced = false;
        RefreshCookingProgress();

        Debug.Log("Plate is ready!");
    }

    public bool SetCurrentSoupBag(GameObject newBag)
    {
        if (newBag == null)
        {
            Debug.LogError("Soup Bag is missing!");
            return false;
        }

        if (!isPlatePlaced)
        {
            Debug.Log("Place the noodle package first!");
            return false;
        }

        if (hasSoupBag)
        {
            Debug.Log("You already have a soup bag!");
            return false;
        }

        currentSoupBag = newBag;
        hasSoupBag = true;
        isSoupBagPlaced = false;
        isSoupBagHeld = true;

        RefreshCookingProgress();

        Debug.Log("Picked up soup bag!");
        return true;
    }

    Transform FindChild(Transform parent, string childName)
    {
        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
        {
            if (child.name == childName)
                return child;
        }

        return null;
    }

    public void AddNoodle()
    {
        if (!CanPrepareFood()) return;
        if (!isBowlHeld)
        {
            Debug.Log("Equip your bowl first! Press 2.");
            return;
        }
        if (!hasBowl || isPlatePlaced)
        {
            Debug.Log("Pick up a plate first!");
            return;
        }

        if (hasNoodle) return;

        noodleVisual.SetActive(true);
        hasNoodle = true;
        RefreshCookingProgress();

        Debug.Log("Noodles added!");
    }

    public void AddTopping()
    {
        if (!CanPrepareFood()) return;
        if (!isBowlHeld)
        {
            Debug.Log("Equip your bowl first! Press 2.");
            return;
        }
        if (!hasBowl || !hasNoodle || isPlatePlaced)
        {
            Debug.Log("Add noodles first!");
            return;
        }

        if (hasTopping) return;

        toppingVisual.SetActive(true);
        hasTopping = true;
        RefreshCookingProgress();

        Debug.Log("Pork added!");
    }

    public bool IsFoodReady()
    {
        return hasBowl && hasNoodle && hasTopping;
    }

    public bool IsCorrectRecipe(string noodle, string soup, string topping)
    {
        return IsFoodReady() &&
               noodle == "Small Noodle" &&
               soup == "Clear Soup" &&
               topping == "Pork";
    }

    public bool PlaceBowlOnStation(Transform servingPoint)
    {
        if (!isBowlHeld)
        {
            Debug.Log("Equip your bowl first! Press 2.");
            return false;
        }
        if (currentBowl == null)
        {
            Debug.Log("You are not holding a plate!");
            return false;
        }

        if (!IsFoodReady())
        {
            Debug.Log("Complete the noodles and topping first!");
            return false;
        }

        if (isPlatePlaced)
        {
            Debug.Log("Plate is already placed!");
            return false;
        }

        if (servingPoint == null)
        {
            Debug.LogError("Serving Point is missing!");
            return false;
        }


        currentBowl.transform.SetParent(servingPoint);
        currentBowl.transform.localPosition = Vector3.zero;
        currentBowl.transform.localRotation = Quaternion.identity;

        bowlRenderer.enabled = false;
        noodleVisual.SetActive(false);
        toppingVisual.SetActive(false);
        packedVisual.gameObject.SetActive(true);

        isPlatePlaced = true;
        isBowlHeld = false;
        RefreshCookingProgress();

        Debug.Log("Noodle package placed!");
        return true;
    }

    public bool PlaceSoupBagOnStation(Transform soupBagPoint)
    {
        if (!isPlatePlaced)
        {
            Debug.Log("Place the noodle package first!");
            return false;
        }
        if (!isSoupBagHeld)
        {
            Debug.Log("Equip your soup bag first! Press 2.");
            return false;
        }

        if (currentSoupBag == null || !hasSoupBag)
        {
            Debug.Log("Pick up a soup bag first!");
            return false;
        }

        if (isSoupBagPlaced)
        {
            Debug.Log("Soup bag is already placed!");
            return false;
        }

        if (soupBagPoint == null)
        {
            Debug.LogError("Soup Bag Point is missing!");
            return false;
        }

        currentSoupBag.transform.SetParent(soupBagPoint);
        currentSoupBag.transform.localPosition = Vector3.zero;
        currentSoupBag.transform.localRotation = Quaternion.identity;

        isSoupBagPlaced = true;
        isSoupBagHeld = false;
        RefreshCookingProgress();

        Debug.Log("Soup bag placed! Order is ready to serve.");
        return true;
    }
    public void CompleteServing()
    {
        if (!isPlatePlaced || !isSoupBagPlaced)
        {
            Debug.Log("Food is not complete!");
            return;
        }

        Debug.Log("Food is ready for customer!");
    }

    public void ResetFood()
    {
        if (currentBowl != null)
            Destroy(currentBowl);

        if (currentSoupBag != null)
            Destroy(currentSoupBag);

        currentBowl = null;
        currentSoupBag = null;

        noodlePoint = null;
        toppingPoint = null;
        packedVisual = null;

        noodleVisual = null;
        toppingVisual = null;
        bowlRenderer = null;

        hasBowl = false;
        hasNoodle = false;
        hasTopping = false;
        isPlatePlaced = false;
        hasSoupBag = false;
        isSoupBagPlaced = false;
        isBowlHeld = false;
        isSoupBagHeld = false;
        RefreshCookingProgress();

        Debug.Log("Food reset!");
    }

    void Update()
    {
        // ตรวจจับตอนลูกค้าเริ่มสั่งอาหาร
        bool orderActive =
            customerManager != null &&
            customerManager.HasActiveOrder;

        if (orderActive != previousOrderState)
        {
            previousOrderState = orderActive;
            RefreshCookingProgress();
        }
    }

    public void RefreshCookingProgress()
    {
        if (progressUI == null)
            return;

        if (customerManager == null ||
            !customerManager.HasActiveOrder)
        {
            progressUI.Hide();
            return;
        }

        // ขั้นตอนที่ 1: รับชาม
        if (!hasBowl)
        {
            progressUI.ShowStep(
                "Pick up a plate",
                "Go to the Plate Station",
                1, 7);
            return;
        }
        if (!isBowlHeld && !isPlatePlaced)
        {
            int step = hasTopping ? 4 : hasNoodle ? 3 : 2;

            progressUI.ShowStep(
                "Equip your bowl",
                "Press 2 to take your bowl from inventory",
                step, 7);
            return;
        }

        // ขั้นตอนที่ 2: ใส่เส้น
        if (!hasNoodle)
        {
            progressUI.ShowStep(
                "Add noodles",
                "Go to the Noodle Station",
                2, 7);
            return;
        }

        // ขั้นตอนที่ 3: ใส่หมู
        if (!hasTopping)
        {
            progressUI.ShowStep(
                "Add pork",
                "Go to the Topping Station",
                3, 7);
            return;
        }

        // ขั้นตอนที่ 4: วางชามที่จุดเสิร์ฟ
        if (!isPlatePlaced)
        {
            progressUI.ShowStep(
                "Pack the noodles",
                "Place the plate at the Serving Station",
                4, 7);
            return;
        }

        // ขั้นตอนที่ 5: รับถุงน้ำซุป
        if (!isSoupBagPlaced)
        {
            if (!isSoupBagHeld)
            {
                progressUI.ShowStep(
                    "Equip soup bag",
                    "Press 2 to take the soup bag",
                    6, 7);
            }
            else
            {
                progressUI.ShowStep(
                    "Place the soup",
                    "Put the soup bag at the Serving Station",
                    6, 7);
            }

            return;
        }

        // ขั้นตอนที่ 6: วางถุงน้ำซุป
        if (!isSoupBagPlaced)
        {
            progressUI.ShowStep(
                "Place the soup",
                "Put the soup bag at the Serving Station",
                6, 7);
            return;
        }

        // ขั้นตอนที่ 7: เสิร์ฟลูกค้า
        progressUI.ShowStep(
            "Serve the customer",
            "Press E at the Serving Station",
            7, 7);
    }

}