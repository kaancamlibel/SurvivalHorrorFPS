using UnityEngine;
using UnityEngine.UI; // UI bileþenleri için gerekli

public class Gun : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float range = 100f;

    [SerializeField] private int maxAmmo = 80;
    public int currentAmmo;

    [SerializeField] private Camera fpsCam;
    [SerializeField] private ParticleSystem muzzleFlash;

    [SerializeField] private float pickupRange = 3f; // Mermi alabilme mesafesi
    [SerializeField] private LayerMask pickupLayer;  // Sadece belirli objeleri algýlamak için

    [SerializeField] private GameObject defaultEffect; // Normal yüzeylere çarptýðýnda kullanýlacak efekt
    [SerializeField] private GameObject bloodEffect;   // Düþmana çarptýðýnda kullanýlacak efekt

    [SerializeField] private Text ammoText; // UI Text bileþeni

    [SerializeField] private AudioSource gunAudioSource; // Ses kaynaðý
    [SerializeField] private AudioClip gunShotSound; // Silah sesi

    private void Start()
    {
        currentAmmo = maxAmmo;
        UpdateAmmoUI(); // Baþlangýçta UI'yý güncelle
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1") && currentAmmo > 0)
        {
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.E)) // "E" tuþuna basýldýðýnda
        {
            TryPickupAmmo();
        }
    }

    private void Shoot()
    {
        muzzleFlash.Play();
        gunAudioSource.PlayOneShot(gunShotSound);

        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log("Vurulan Obje: " + hit.transform.name);

            GameObject effectToSpawn = defaultEffect; // Varsayýlan olarak normal efekt

            // Tag kontrolüne göre efekt seç
            if (hit.transform.CompareTag("Enemy"))
            {
                Debug.Log("Düþmana isabet etti! Hasar veriliyor...");

                // Düþmana hasar ver
                Target target = hit.transform.GetComponentInParent<Target>();
                if (target != null)
                {
                    target.TakeDamage(damage);
                    effectToSpawn = bloodEffect; // Düþmana çarptýðýnda kan efekti
                }
                else
                {
                    Debug.LogWarning("Düþman objesinde Target scripti bulunamadý!");
                }
            }
            else
            {
                Debug.Log("Vurulan obje düþman deðil. Tag: " + hit.transform.tag);
            }

            // Seçilen etkiyi oluþtur
            if (effectToSpawn != null)
            {
                Instantiate(effectToSpawn, hit.point, Quaternion.LookRotation(hit.normal));
            }
            else
            {
                Debug.LogWarning("Efekt prefabý atanmamýþ!");
            }

            currentAmmo--;
            UpdateAmmoUI(); // Mermi sayýsý deðiþtiðinde UI'yý güncelle
        }
    }

    private void TryPickupAmmo()
    {
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, pickupRange, pickupLayer))
        {
            if (hit.collider.CompareTag("Clip")) // "Clip" tag'li objeyi kontrol et
            {
                currentAmmo = Mathf.Min(currentAmmo + 5, maxAmmo); // Cephane ekle (maxAmmo'yu aþmasýn)
                UpdateAmmoUI(); // Mermi sayýsý deðiþtiðinde UI'yý güncelle
                Destroy(hit.collider.gameObject); // Objeyi yok et
            }
        }
    }

    private void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = currentAmmo.ToString(); // UI Text'i güncelle
        }
        else
        {
            Debug.LogWarning("Ammo Text bileþeni atanmamýþ!");
        }
    }
}