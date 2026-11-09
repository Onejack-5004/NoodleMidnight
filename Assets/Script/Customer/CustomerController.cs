
using UnityEngine;
using System;

public class CustomerController : MonoBehaviour, IInteractable
{
    [Header("Movement")]
    public float moveSpeed = 1.5f;
    public float stopDistance = 0.1f;

    private Transform waitPoint;
    private Transform exitPoint;
    private OrderUI orderUI;

    private bool hasArrived = false;
    private bool isLeaving = false;
    private bool hasOrdered = false;
    private bool isDead = false;

    private Action onCustomerLeft;
    private Action onCustomerOrdered;

    private string noodle;
    private string soup;
    private string topping;

    public bool CanPrepareFood
    {
        get
        {
            return hasArrived && hasOrdered && !isLeaving && !isDead;
        }
    }

    public bool WasServed { get; private set; } = false;

    public void Initialize(
        Transform wait,
        Transform exit,
        OrderUI ui,
        Action leftCallback,
        Action orderedCallback)
    {
        waitPoint = wait;
        exitPoint = exit;
        orderUI = ui;
        onCustomerLeft = leftCallback;
        onCustomerOrdered = orderedCallback;

        GenerateOrder();
    }

    void GenerateOrder()
    {
        noodle = "Small Noodle";
        soup = "Clear Soup";
        topping = "Pork";
    }

    void Update()
    {
        if (isDead)
            return;

        if (isLeaving)
            MoveToTarget(exitPoint);
        else if (!hasArrived)
            MoveToTarget(waitPoint);
    }

    void MoveToTarget(Transform target)
    {
        if (target == null)
            return;

        Vector3 targetPosition = target.position;
        targetPosition.y = transform.position.y;

        Vector3 direction = targetPosition - transform.position;

        if (direction.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(direction);

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPosition)
            <= stopDistance)
        {
            if (isLeaving)
                LeaveShop();
            else
            {
                hasArrived = true;
                Debug.Log("Customer has arrived!");
            }
        }
    }

    public void Interact()
    {
        if (isDead || !hasArrived || isLeaving || hasOrdered)
            return;

        hasOrdered = true;

        Debug.Log("Customer ordered Clear Soup Noodles!");

        // แจ้ง CustomerManager เพื่อเพิ่มเงิน
        onCustomerOrdered?.Invoke();

        if (orderUI != null)
            orderUI.ShowOrder(noodle, soup, topping);
    }

    public bool TryServe(CookingManager cookingManager)
    {
        if (isDead || !hasArrived || !hasOrdered || isLeaving)
        {
            Debug.Log("Customer is not ready to receive food!");
            return false;
        }

        if (cookingManager == null)
            return false;

        if (!cookingManager.IsFoodReady())
        {
            Debug.Log("Food is incomplete!");

            if (orderUI != null)
                orderUI.SetStatus("Food is incomplete!");

            return false;
        }

        bool correctOrder = cookingManager.IsCorrectRecipe(
            noodle,
            soup,
            topping
        );

        if (!correctOrder)
        {
            Debug.Log("Wrong order!");

            if (orderUI != null)
                orderUI.SetStatus("Wrong order! Try again.");

            return false;
        }

        Debug.Log("Correct order served!");

        CompleteOrder();
        return true;
    }

    public void CompleteOrder()
    {
        if (isDead || !hasOrdered || isLeaving)
            return;

        WasServed = true;

        if (orderUI != null)
            orderUI.HideOrder();

        StartLeaving();
    }

    void StartLeaving()
    {
        if (exitPoint == null)
            return;

        isLeaving = true;
        Debug.Log("Customer is leaving...");
    }

    void LeaveShop()
    {
        if (isDead)
            return;

        onCustomerLeft?.Invoke();
        Destroy(gameObject);
    }

    public bool TryRegisterShot(out bool isAnomaly)
    {
        isAnomaly = false;

        if (isDead)
            return false;

        isDead = true;

        CustomerType customerType = GetComponent<CustomerType>();

        if (customerType != null)
            isAnomaly = customerType.IsAnomaly;
        else
            Debug.LogWarning("CustomerType is missing! Treating as Normal.");

        return true;
    }

    public void DestroyAfterShot()
    {
        Destroy(gameObject);
    }
}
