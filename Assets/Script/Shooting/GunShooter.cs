using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

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

    [Header("Visual Effects (VFX)")]
    public ParticleSystem muzzleFlashParticles;
    public Light muzzleFlashLight;
    public GameObject impactEffectPrefab;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip shootSound;
    public AudioClip emptySound;
    public AudioClip reloadSound;

    [Header("Recoil / Kickback")]
    public Transform gunModelTransform;
    public float kickbackDistance = 0.05f;
    public float kickbackAngle = 3.5f;
    public float recoilReturnSpeed = 16f;

    private int currentAmmo;
    private float nextFireTime = 0f;
    private bool isEquipped = false;

    // Recoil state
    private Vector3 originalLocalPos;
    private Quaternion originalLocalRot;
    private Vector3 currentRecoilPos;
    private Vector3 currentRecoilRot;

    void Start()
    {
        currentAmmo = magazineSize;

        if (playerCamera == null)
            Debug.LogError("GunShooter: Player Camera is missing!");

        if (muzzlePoint == null)
            Debug.LogWarning("GunShooter: Muzzle Point is missing!");

        if (customerManager == null)
            Debug.LogError("GunShooter: Customer Manager is missing!");

        // Auto-assign AudioSource if not set
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        // Auto-assign Muzzle Flash if under muzzlePoint
        if (muzzleFlashParticles == null && muzzlePoint != null)
            muzzleFlashParticles = muzzlePoint.GetComponentInChildren<ParticleSystem>();

        // Auto-assign Muzzle Light if under muzzlePoint
        if (muzzleFlashLight == null && muzzlePoint != null)
        {
            muzzleFlashLight = muzzlePoint.GetComponentInChildren<Light>();
            if (muzzleFlashLight != null)
                muzzleFlashLight.enabled = false;
        }

        // Auto-assign Gun Model Transform for kickback
        if (gunModelTransform == null && muzzlePoint != null && muzzlePoint.parent != null)
            gunModelTransform = muzzlePoint.parent;

        if (gunModelTransform != null)
        {
            originalLocalPos = gunModelTransform.localPosition;
            originalLocalRot = gunModelTransform.localRotation;
        }
    }

    public void SetEquipped(bool equipped)
    {
        isEquipped = equipped;
        if (!equipped && gunModelTransform != null)
        {
            currentRecoilPos = Vector3.zero;
            currentRecoilRot = Vector3.zero;
            gunModelTransform.localPosition = originalLocalPos;
            gunModelTransform.localRotation = originalLocalRot;
        }
    }

    void Update()
    {
        // Handle recoil recovery
        UpdateRecoil();

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

    void UpdateRecoil()
    {
        if (gunModelTransform == null)
            return;

        currentRecoilPos = Vector3.Lerp(currentRecoilPos, Vector3.zero, Time.deltaTime * recoilReturnSpeed);
        currentRecoilRot = Vector3.Lerp(currentRecoilRot, Vector3.zero, Time.deltaTime * recoilReturnSpeed);

        gunModelTransform.localPosition = originalLocalPos + currentRecoilPos;
        gunModelTransform.localRotation = originalLocalRot * Quaternion.Euler(currentRecoilRot);
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
            if (audioSource != null && emptySound != null)
                audioSource.PlayOneShot(emptySound);
            return;
        }

        nextFireTime = Time.time + fireRate;
        currentAmmo--;

        Debug.Log("BANG! Ammo: " + currentAmmo + "/∞");

        // 1. Play Audio with subtle pitch variation
        if (audioSource != null && shootSound != null)
        {
            audioSource.pitch = Random.Range(0.96f, 1.04f);
            audioSource.PlayOneShot(shootSound);
        }

        // 2. Play Muzzle Flash Particle System
        if (muzzleFlashParticles != null)
            muzzleFlashParticles.Play(true);

        // 3. Trigger Muzzle Flash Light
        if (muzzleFlashLight != null)
            StartCoroutine(FlashMuzzleLight());

        // 4. Apply Procedural Gun Kickback
        ApplyRecoil();

        // 5. Raycast for Hit Detection
        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        if (Physics.Raycast(ray, out RaycastHit hit, shootRange))
        {
            Debug.Log("Shot hit: " + hit.collider.name);

            // Spawn Impact Particles
            if (impactEffectPrefab != null)
            {
                Quaternion hitRot = hit.normal.sqrMagnitude > 0.001f
                    ? Quaternion.LookRotation(hit.normal)
                    : Quaternion.identity;
                GameObject impact = Instantiate(impactEffectPrefab, hit.point, hitRot);
                Destroy(impact, 1.5f);
            }

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

    void ApplyRecoil()
    {
        // Push gun back along local Z and kick barrel up along local X
        currentRecoilPos += new Vector3(0f, 0.005f, -kickbackDistance);
        currentRecoilRot += new Vector3(-kickbackAngle, Random.Range(-0.5f, 0.5f), 0f);
    }

    IEnumerator FlashMuzzleLight()
    {
        muzzleFlashLight.enabled = true;
        yield return new WaitForSeconds(0.04f);
        if (muzzleFlashLight != null)
            muzzleFlashLight.enabled = false;
    }

    void Reload()
    {
        if (currentAmmo >= magazineSize)
        {
            Debug.Log("Magazine is already full!");
            return;
        }

        currentAmmo = magazineSize;

        if (audioSource != null && reloadSound != null)
        {
            audioSource.pitch = 1.0f;
            audioSource.PlayOneShot(reloadSound);
        }

        Debug.Log("Reloaded! Ammo: " + currentAmmo + "/∞");
    }
}
