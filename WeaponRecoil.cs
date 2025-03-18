using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    [SerializeField] private float recoilDistance = 0.1f; // Silah�n geri �ekilme mesafesi
    [SerializeField] private float recoilSpeed = 20f; // Geri �ekilme h�z�
    [SerializeField] private float returnSpeed = 10f; // Geri d�n�� h�z�
    [SerializeField] private Gun gun; // Gun scriptine referans

    private Vector3 originalPosition; // Silah�n orijinal pozisyonu
    [SerializeField] private bool isFiring; // Ate� etme durumu

    public bool IsFiring => isFiring; // D��ar�dan sadece okunabilir

    void Start()
    {
        originalPosition = transform.localPosition;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && gun.currentAmmo > 0) // Burada getter kulland�k
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
