using UnityEngine;

public class EnemyRagdoll : MonoBehaviour
{
    private Rigidbody[] ragdollRigidbodies; // T�m ragdoll rigidbody bile�enleri
    private bool isDead = false; // D��man�n �l�p �lmedi�ini kontrol eder

    void Start()
    {
        // Rigidbody ve Collider bile�enlerini bul
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();

        DisableRagdoll(); // Ba�lang��ta ragdoll�u kapat
    }

    // Ragdoll�u devre d��� b�rak (D��man hayattayken)
    void DisableRagdoll()
    {
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = true; // Rigidbody�yi devre d��� b�rak
        }
    }

    // Ragdoll�u aktif et (�ld���nde)
    internal void EnableRagdoll()
    {
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = false; // Rigidbody�yi aktif et
        }
    }

    // D��man �ld���nde �a�r�lacak fonksiyon
    internal void Die()
    {
        if (!isDead) // �ld�yse tekrar �al��t�rma
        {
            isDead = true;
            EnableRagdoll(); // Ragdoll�u etkinle�tir
            Debug.Log("D��man �ld�, ragdoll aktif edildi!");
        }
    }
}
