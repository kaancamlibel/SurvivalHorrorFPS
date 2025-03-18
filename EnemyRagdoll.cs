using UnityEngine;

public class EnemyRagdoll : MonoBehaviour
{
    private Rigidbody[] ragdollRigidbodies; // Tüm ragdoll rigidbody bileþenleri
    private bool isDead = false; // Düþmanýn ölüp ölmediðini kontrol eder

    void Start()
    {
        // Rigidbody ve Collider bileþenlerini bul
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();

        DisableRagdoll(); // Baþlangýçta ragdoll’u kapat
    }

    // Ragdoll’u devre dýþý býrak (Düþman hayattayken)
    void DisableRagdoll()
    {
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = true; // Rigidbody’yi devre dýþý býrak
        }
    }

    // Ragdoll’u aktif et (Öldüðünde)
    internal void EnableRagdoll()
    {
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = false; // Rigidbody’yi aktif et
        }
    }

    // Düþman öldüðünde çaðrýlacak fonksiyon
    internal void Die()
    {
        if (!isDead) // Öldüyse tekrar çalýþtýrma
        {
            isDead = true;
            EnableRagdoll(); // Ragdoll’u etkinleþtir
            Debug.Log("Düþman öldü, ragdoll aktif edildi!");
        }
    }
}
