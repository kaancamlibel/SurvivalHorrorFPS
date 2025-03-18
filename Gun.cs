using UnityEngine;
using UnityEngine.UI; // UI bile�enleri i�in gerekli

public class Gun : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float range = 100f;

    [SerializeField] private int maxAmmo = 80;
    public int currentAmmo;

    [SerializeField] private Camera fpsCam;
    [SerializeField] private ParticleSystem muzzleFlash;

    [SerializeField] private float pickupRange = 3f; // Mermi alabilme mesafesi
    [SerializeField] private LayerMask pickupLayer;  // Sadece belirli objeleri alg�lamak i�in

    [SerializeField] private GameObject defaultEffect; // Normal y�zeylere �arpt���nda kullan�lacak efekt
    [SerializeField] private GameObject bloodEffect;   // D��mana �arpt���nda kullan�lacak efekt

    [SerializeField] private Text ammoText; // UI Text bile�eni

    [SerializeField] private AudioSource gunAudioSource; // Ses kayna��
    [SerializeField] private AudioClip gunShotSound; // Silah sesi

    private void Start()
    {
        currentAmmo = maxAmmo;
        UpdateAmmoUI(); // Ba�lang��ta UI'y� g�ncelle
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1") && currentAmmo > 0)
        {
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.E)) // "E" tu�una bas�ld���nda
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

            GameObject effectToSpawn = defaultEffect; // Varsay�lan olarak normal efekt

            // Tag kontrol�ne g�re efekt se�
            if (hit.transform.CompareTag("Enemy"))
            {
                Debug.Log("D��mana isabet etti! Hasar veriliyor...");

                // D��mana hasar ver
                Target target = hit.transform.GetComponentInParent<Target>();
                if (target != null)
                {
                    target.TakeDamage(damage);
                    effectToSpawn = bloodEffect; // D��mana �arpt���nda kan efekti
                }
                else
                {
                    Debug.LogWarning("D��man objesinde Target scripti bulunamad�!");
                }
            }
            else
            {
                Debug.Log("Vurulan obje d��man de�il. Tag: " + hit.transform.tag);
            }

            // Se�ilen etkiyi olu�tur
            if (effectToSpawn != null)
            {
                Instantiate(effectToSpawn, hit.point, Quaternion.LookRotation(hit.normal));
            }
            else
            {
                Debug.LogWarning("Efekt prefab� atanmam��!");
            }

            currentAmmo--;
            UpdateAmmoUI(); // Mermi say�s� de�i�ti�inde UI'y� g�ncelle
        }
    }

    private void TryPickupAmmo()
    {
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, pickupRange, pickupLayer))
        {
            if (hit.collider.CompareTag("Clip")) // "Clip" tag'li objeyi kontrol et
            {
                currentAmmo = Mathf.Min(currentAmmo + 5, maxAmmo); // Cephane ekle (maxAmmo'yu a�mas�n)
                UpdateAmmoUI(); // Mermi say�s� de�i�ti�inde UI'y� g�ncelle
                Destroy(hit.collider.gameObject); // Objeyi yok et
            }
        }
    }

    private void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = currentAmmo.ToString(); // UI Text'i g�ncelle
        }
        else
        {
            Debug.LogWarning("Ammo Text bile�eni atanmam��!");
        }
    }
}