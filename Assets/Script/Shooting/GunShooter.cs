
using UnityEngine;
using UnityEngine.InputSystem;

public class GunShooter : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public Transform muzzlePoint;
    public CustomerManager customerManager;

    [Header("Shooting Settings")]
    public float shootRange = 50f;
    public float fireRate = 0.25f;

    [Header("Ammo")]
    public int magazineSize = 12;

    private int currentAmmo;
    private float nextFireTime = 0f;

    private bool isEquipped = false;

    void Start()
    {
        currentAmmo = magazineSize;

        if (playerCamera == null)
            Debug.LogError("GunShooter: Player Camera is missing!");

        if (muzzlePoint == null)
            Debug.LogWarning("GunShooter: Muzzle Point is missing!");

        if (customerManager == null)
            Debug.LogError("GunShooter: Customer Manager is missing!");
    }

    public void SetEquipped(bool equipped)
    {
        isEquipped = equipped;
    }

    void Update()
    {
        if (!isEquipped)
            return;

        if (Mouse.current == null)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
            Shoot();

        if (Keyboard.current != null &&
            Keyboard.current.rKey.wasPressedThisFrame)
        {
            Reload();
        }
    }

    void Shoot()
    {
        if (playerCamera == null)
            return;

        if (Time.time < nextFireTime)
            return;

        if (currentAmmo <= 0)
        {
            Debug.Log("Out of ammo! Press R to reload.");
            return;
        }

        nextFireTime = Time.time + fireRate;
        currentAmmo--;

        Debug.Log("BANG! Ammo: " + currentAmmo + "/∞");

        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        if (Physics.Raycast(ray, out RaycastHit hit, shootRange))
        {
            Debug.Log("Shot hit: " + hit.collider.name);

            CustomerController customer =
                hit.collider.GetComponentInParent<CustomerController>();

            if (customer != null)
            {
                if (customerManager == null)
                {
                    Debug.LogError("CustomerManager is missing!");
                    return;
                }

                customerManager.HandleCustomerShot(customer);
            }
        }
        else
        {
            Debug.Log("Shot missed!");
        }

        Debug.DrawRay(
            ray.origin,
            ray.direction * shootRange,
            Color.red,
            1f
        );
    }

    void Reload()
    {
        if (currentAmmo >= magazineSize)
        {
            Debug.Log("Magazine is already full!");
            return;
        }

        currentAmmo = magazineSize;

        Debug.Log("Reloaded! Ammo: " + currentAmmo + "/∞");
    }
}
