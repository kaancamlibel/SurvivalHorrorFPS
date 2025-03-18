using UnityEngine;
using UnityEngine.AI;

public class Target : MonoBehaviour
{
    [SerializeField] private float health = 50f;
    private bool isDead = false;
    private bool isChasingPlayer = false; // Player'ý takip ediyor mu?

    [SerializeField] private GameObject ragdollPrefab; // Ragdoll prefabý
    [SerializeField] private EnemyRagdoll ragdoll; // EnemyRagdoll scripti

    private NavMeshAgent agent;
    private Transform player;
    private Animator animator;

    void Start()
    {
        // Eðer Inspector'dan atanmadýysa, GetComponent ile al
        if (ragdoll == null)
        {
            ragdoll = GetComponent<EnemyRagdoll>();
        }

        // NavMeshAgent bileþenini al
        agent = GetComponent<NavMeshAgent>();
        // Animator bileþenini al
        animator = GetComponent<Animator>();

        // Player'ý bul
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player == null)
        {
            Debug.LogError("Player objesi bulunamadý!");
        }
    }

    void Update()
    {
        // Eðer düþman öldüyse veya player'ý takip etmiyorsa, hiçbir þey yapma
        if (isDead || !isChasingPlayer) return;

        // Player'ý sürekli olarak takip et
        ChasePlayer();
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return; // Eðer düþman zaten öldüyse, tekrar çalýþtýrma

        Debug.Log(gameObject.name + " hasar aldý: " + amount);

        health -= amount;
        Debug.Log("Kalan can: " + health);

        if (health <= 0f)
        {
            Die();
        }
        else
        {
            // Can azaldýðýnda player'ý takip etmeye baþla
            isChasingPlayer = true;
        }
    }

    void ChasePlayer()
    {
        if (agent != null && player != null)
        {
            // Player'ýn konumunu sürekli olarak güncelle
            agent.SetDestination(player.position);

            // Running animasyonunu çaðýr
            if (animator != null)
            {
                animator.SetBool("isRunning", true);
            }
        }
    }

    void Die()
    {
        if (!isDead) // Tekrar çalýþmasýný önlemek için kontrol ekle
        {
            isDead = true;

            // Ragdoll prefabýný oluþtur
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

            // Orijinal düþmaný yok et
            Destroy(gameObject);
        }
    }
}