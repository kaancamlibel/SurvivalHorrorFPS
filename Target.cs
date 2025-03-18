using UnityEngine;
using UnityEngine.AI;

public class Target : MonoBehaviour
{
    [SerializeField] private float health = 50f;
    private bool isDead = false;
    private bool isChasingPlayer = false; // Player'� takip ediyor mu?

    [SerializeField] private GameObject ragdollPrefab; // Ragdoll prefab�
    [SerializeField] private EnemyRagdoll ragdoll; // EnemyRagdoll scripti

    private NavMeshAgent agent;
    private Transform player;
    private Animator animator;

    void Start()
    {
        // E�er Inspector'dan atanmad�ysa, GetComponent ile al
        if (ragdoll == null)
        {
            ragdoll = GetComponent<EnemyRagdoll>();
        }

        // NavMeshAgent bile�enini al
        agent = GetComponent<NavMeshAgent>();
        // Animator bile�enini al
        animator = GetComponent<Animator>();

        // Player'� bul
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player == null)
        {
            Debug.LogError("Player objesi bulunamad�!");
        }
    }

    void Update()
    {
        // E�er d��man �ld�yse veya player'� takip etmiyorsa, hi�bir �ey yapma
        if (isDead || !isChasingPlayer) return;

        // Player'� s�rekli olarak takip et
        ChasePlayer();
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return; // E�er d��man zaten �ld�yse, tekrar �al��t�rma

        Debug.Log(gameObject.name + " hasar ald�: " + amount);

        health -= amount;
        Debug.Log("Kalan can: " + health);

        if (health <= 0f)
        {
            Die();
        }
        else
        {
            // Can azald���nda player'� takip etmeye ba�la
            isChasingPlayer = true;
        }
    }

    void ChasePlayer()
    {
        if (agent != null && player != null)
        {
            // Player'�n konumunu s�rekli olarak g�ncelle
            agent.SetDestination(player.position);

            // Running animasyonunu �a��r
            if (animator != null)
            {
                animator.SetBool("isRunning", true);
            }
        }
    }

    void Die()
    {
        if (!isDead) // Tekrar �al��mas�n� �nlemek i�in kontrol ekle
        {
            isDead = true;

            // Ragdoll prefab�n� olu�tur
            if (ragdollPrefab != null)
            {
                GameObject ragdollInstance = Instantiate(ragdollPrefab, transform.position, transform.rotation);

                // Ragdoll'u aktif et
                EnemyRagdoll ragdollScript = ragdollInstance.GetComponent<EnemyRagdoll>();
                if (ragdollScript != null)
                {
                    ragdollScript.EnableRagdoll();
                }
            }

            // Orijinal d��man� yok et
            Destroy(gameObject);
        }
    }
}