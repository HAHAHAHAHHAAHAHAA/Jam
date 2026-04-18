using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private float range = 100f;
    [SerializeField] private float fireRate = 0.1f;
    [SerializeField] private int magazineSize = 30;
    [SerializeField] private float reloadTime = 2f;

    [Header("Визуал")]
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private AudioSource shotSound;
    [SerializeField] private AudioSource reloadSound;

    private int currentAmmo;
    private bool isReloading = false;
    private float nextFireTime = 0f;
    private Camera playerCamera;

    void Start()
    {
        playerCamera = GetComponentInParent<Camera>();
        currentAmmo = magazineSize;
    }

    public void Shoot()
    {
        if (isReloading) return;
        if (Time.time < nextFireTime) return;
        if (currentAmmo <= 0)
        {
            Reload();
            return;
        }

        currentAmmo--;
        nextFireTime = Time.time + fireRate;

        if (muzzleFlash != null) muzzleFlash.Play();
        if (shotSound != null) shotSound.Play();

        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            Debug.Log($"Попал в: {hit.collider.name}");
        }

        Debug.Log($"Выстрел. Патроны: {currentAmmo}/{magazineSize}");
    }

    public void Reload()
    {
        if (isReloading) return;
        if (currentAmmo >= magazineSize) return;

        StartCoroutine(ReloadCoroutine());
    }

    private System.Collections.IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        if (reloadSound != null) reloadSound.Play();

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = magazineSize;
        isReloading = false;

        Debug.Log("Перезаряжено");
    }
}