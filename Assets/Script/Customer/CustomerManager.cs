
using UnityEngine;
using System.Collections;

public class CustomerManager : MonoBehaviour
{
    [Header("Customer Settings")]
    public GameObject customerPrefab;

    [Header("Spawn Points")]
    public Transform spawnPoint;
    public Transform waitPoint;
    public Transform exitPoint;

    [Header("UI")]
    public OrderUI orderUI;
    public PlayerStatusUI playerStatusUI;

    [Header("Spawn Settings")]
    public float nextCustomerDelay = 2f;

    [Header("System References")]
    public CookingManager cookingManager;
    public InventoryManager inventoryManager;

    [Header("Money Settings")]
    public int currentMoney = 0;
    public int moneyPerOrder = 100;

    [Header("Health Settings")]
    public int maxHealth = 3;
    public int currentHealth = 3;

    private GameObject currentCustomer;
    private bool isSpawning = false;

    [Header("Anomaly Settings")]
    [Range(0, 100)]
    public int anomalyChancePercent = 20;

    public int CurrentMoney => currentMoney;
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public GameObject[] normalModels;
    public GameObject[] anomalyModels;

    public bool HasActiveOrder
    {
        get
        {
            if (currentCustomer == null)
                return false;

            CustomerController controller =
                currentCustomer.GetComponent<CustomerController>();

            return controller != null && controller.CanPrepareFood;
        }
    }

    void Start()
    {
        currentMoney = 0;
        currentHealth = maxHealth;

        UpdateStatusUI();
        SpawnCustomer();
    }

    // เรียกเมื่อลูกค้าสั่งออร์เดอร์
    public void OnCustomerOrdered()
    {
        currentMoney += moneyPerOrder;

        Debug.Log("MONEY: +" + moneyPerOrder);
        Debug.Log("TOTAL MONEY: " + currentMoney);

        UpdateStatusUI();
    }

    // ลดเลือดผู้เล่น
    void LoseHealth(string reason)
    {
        currentHealth = Mathf.Max(0, currentHealth - 1);

        Debug.Log(reason + " -1 HEART");
        Debug.Log("HEALTH: " + currentHealth + "/" + maxHealth);

        UpdateStatusUI();

        if (currentHealth <= 0)
        {
            Debug.Log("GAME OVER! PLAYER HAS NO HEALTH!");
        }
    }

    void UpdateStatusUI()
    {
        if (playerStatusUI != null)
            playerStatusUI.RefreshUI();
    }

    void SpawnCustomer()
    {
        if (isSpawning || currentCustomer != null)
            return;

        if (currentHealth <= 0)
            return;

        if (customerPrefab == null ||
            spawnPoint == null ||
            waitPoint == null ||
            exitPoint == null ||
            orderUI == null)
        {
            Debug.LogError("CustomerManager references are missing!");
            return;
        }

        GameObject newCustomer = Instantiate(
            customerPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        currentCustomer = newCustomer;

        CustomerType customerType =
            newCustomer.GetComponent<CustomerType>();

        if (customerType == null)
            customerType = newCustomer.AddComponent<CustomerType>();

        bool isAnomaly =
            Random.Range(0, 100) < anomalyChancePercent;

        customerType.customerCategory = isAnomaly
            ? CustomerType.Category.Anomaly
            : CustomerType.Category.Normal;
        CustomerVisualSelector visualSelector =
            newCustomer.GetComponent<CustomerVisualSelector>();

        if (visualSelector != null)
        {
            visualSelector.SetModel(isAnomaly);
        }
        else
        {
            Debug.LogWarning("CustomerVisualSelector is missing!");
        }

        Debug.Log(isAnomaly
            ? "WARNING: Anomaly Customer Spawned!"
            : "Normal Customer Spawned!");

        CustomerController controller =
            newCustomer.GetComponent<CustomerController>();

        if (controller == null)
        {
            Debug.LogError("Customer prefab needs CustomerController!");
            Destroy(newCustomer);
            currentCustomer = null;
            return;
        }

        controller.Initialize(
            waitPoint,
            exitPoint,
            orderUI,
            OnCustomerLeft,
            OnCustomerOrdered
        );

        Debug.Log("New customer spawned!");
    }

    public void TryServe(CookingManager cookingManager)
    {
        if (currentCustomer == null)
        {
            Debug.Log("There is no customer!");
            return;
        }

        CustomerController controller =
            currentCustomer.GetComponent<CustomerController>();

        if (controller == null)
            return;

        bool success = controller.TryServe(cookingManager);

        if (success)
        {
            CustomerType type =
                controller.GetComponent<CustomerType>();

            // เสิร์ฟลูกค้าผิดปกติ = เสียเลือด
            if (type != null && type.IsAnomaly)
            {
                LoseHealth("WRONG CUSTOMER SERVED!");
            }

            cookingManager.ResetFood();
        }
    }

    // รับข้อมูลเมื่อลูกค้าถูกยิง
    public void HandleCustomerShot(CustomerController controller)
    {
        if (controller == null || currentCustomer == null)
            return;

        if (controller.gameObject != currentCustomer)
            return;

        if (!controller.TryRegisterShot(out bool isAnomaly))
            return;

        if (isAnomaly)
        {
            Debug.Log("ANOMALY ELIMINATED!");
        }
        else
        {
            // ยิงลูกค้าปกติ = เสียเลือด
            LoseHealth("WRONGFUL SHOOTING!");
        }

        // ซ่อน UI ออร์เดอร์
        if (orderUI != null)
            orderUI.HideOrder();

        // ล้างไอเทมใน Inventory ก่อนทำลายวัตถุดิบ
        if (inventoryManager != null)
            inventoryManager.ClearCookingItems();

        // ยกเลิกอาหารที่กำลังทำอยู่
        if (cookingManager != null)
            cookingManager.ResetFood();

        // ปลดลูกค้าคนปัจจุบัน
        currentCustomer = null;

        // ทำลายลูกค้าที่ถูกยิง
        controller.DestroyAfterShot();

        // เริ่มนับเวลาก่อนเกิดลูกค้าคนใหม่
        StartCoroutine(SpawnNextCustomer());
    }

    void OnCustomerLeft()
    {
        currentCustomer = null;
        StartCoroutine(SpawnNextCustomer());
    }

    IEnumerator SpawnNextCustomer()
    {
        if (isSpawning || currentHealth <= 0)
            yield break;

        isSpawning = true;

        yield return new WaitForSeconds(nextCustomerDelay);

        isSpawning = false;

        SpawnCustomer();
    }
}
