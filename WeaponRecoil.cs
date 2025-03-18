using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    [SerializeField] private float recoilDistance = 0.1f; // Silahýn geri çekilme mesafesi
    [SerializeField] private float recoilSpeed = 20f; // Geri çekilme hýzý
    [SerializeField] private float returnSpeed = 10f; // Geri dönüþ hýzý
    [SerializeField] private Gun gun; // Gun scriptine referans

    private Vector3 originalPosition; // Silahýn orijinal pozisyonu
    [SerializeField] private bool isFiring; // Ateþ etme durumu

    public bool IsFiring => isFiring; // Dýþarýdan sadece okunabilir

    void Start()
    {
        originalPosition = transform.localPosition;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && gun.currentAmmo > 0) // Burada getter kullandýk
        {
            if (!isFiring)
            {
                isFiring = true;
            }
            Recoil();
        }
        else
        {
            if (isFiring)
            {
                isFiring = false;
            }
            ReturnToOriginalPosition();
        }
    }

    void Recoil()
    {
        Vector3 targetPosition = originalPosition - Vector3.forward * recoilDistance;
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, recoilSpeed * Time.deltaTime);
    }

    void ReturnToOriginalPosition()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, returnSpeed * Time.deltaTime);
    }
}
