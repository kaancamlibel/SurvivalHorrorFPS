using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Transform player; // Player'ýn transformu
    [SerializeField] private float sightRange = 10f; // Görüþ mesafesi
    [SerializeField] private float sightAngle = 90f; // Görüþ açýsý (derece cinsinden)
    [SerializeField] private LayerMask obstacleMask; // Engelleri belirten layer mask
    [SerializeField] private Transform[] patrolPoints; // Hedef noktalarý tutacak dizi

    [SerializeField] private GameObject deathPanel; // Siyah ekran paneli
    [SerializeField] private Text deathText; // "Died" yazýsý
    [SerializeField] private float deathDelay = 3f; // Ana menüye dönme süresi

    private NavMeshAgent agent;
    private Animator animator;
    private bool isPlayerInSight = false;
    private int currentPatrolIndex = 0; // Þu anki hedef nokta indeksi
    private bool isPlayerDead = false; // Player'ýn ölüp ölmediðini kontrol eder

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip chaseSound;

    [SerializeField] private GameObject backgroundSound; // Background müzik objesi

    private bool isChasing = false; // Takip sesinin oynatýlýp oynatýlmadýðýný kontrol eder

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Null kontrolleri
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent bileþeni bulunamadý!");
        }

        if (animator == null)
        {
            Debug.LogError("Animator bileþeni bulunamadý!");
        }

        if (player == null)
        {
            Debug.LogError("Player objesi atanmamýþ!");
        }

        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            Debug.LogError("Hedef noktalarý (patrolPoints) atanmamýþ veya boþ!");
        }
        else
        {
            // Ýlk hedef noktaya git
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }

        // Baþlangýçta ölüm ekranýný gizle
        if (deathPanel != null)
        {
            deathPanel.SetActive(false);
        }

        if (deathText != null)
        {
            deathText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        CheckPlayerInSight();

        if (isPlayerInSight)
        {
            // Player'ý takip et
            agent.SetDestination(player.position);

            // Eðer ses daha önce çalmadýysa, çalmaya baþla
            if (!isChasing)
            {
                audioSource.PlayOneShot(chaseSound);
                isChasing = true;
            }

            // Hýzý güncelle
            float speed = agent.velocity.magnitude;
            animator.SetFloat("speed", speed);

            // Koþma durumunu kontrol et
            animator.SetBool("isRunning", speed > 0.01f);
        }
        else
        {
            // Player'ý görmüyorsa, patrol noktalarý arasýnda hareket et
            Patrol();

            // Takip bittiðinde boolean sýfýrlanýr, böylece tekrar baþladýðýnda ses yeniden çalýnabilir
            isChasing = false;
        }

        // Player'a deðdiðinde ölüm ekranýný göster (sadece bir kez tetikle)
        if (!isPlayerDead && Vector3.Distance(transform.position, player.position) < 1.5f)
        {
            isPlayerDead = true;
            audioSource.Stop();
            OnPlayerDeath();
        }
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0)
        {
            return;
        }

        // Hedef noktaya ulaþýldý mý?
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            // Bir sonraki hedef noktaya geç
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }

        // Hýzý güncelle
        float speed = agent.velocity.magnitude;
        animator.SetFloat("speed", speed);

        // Patrol sýrasýnda sadece walk animasyonu oynat
        animator.SetBool("isRunning", false); // Walk animasyonu
    }

    void CheckPlayerInSight()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        // Player görüþ mesafesi içinde mi?
        if (distanceToPlayer <= sightRange)
        {
            // Player görüþ açýsý içinde mi?
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
            if (angleToPlayer <= sightAngle / 2f)
            {
                // Raycast ile engel kontrolü
                if (!Physics.Raycast(transform.position, directionToPlayer.normalized, distanceToPlayer, obstacleMask))
                {
                    isPlayerInSight = true;
                    return;
                }
            }
        }

        isPlayerInSight = false;
    }

    void OnPlayerDeath()
    {
        backgroundSound.SetActive(false);

        // Ölüm ekranýný göster
        if (deathPanel != null)
        {
            deathPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("Death Panel atanmamýþ!");
        }

        if (deathText != null)
        {
            deathText.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("Death Text atanmamýþ!");
        }

        // Player'ýn hareketini durdur
        if (player != null)
        {
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.enabled = false;
            }
            else
            {
                Debug.LogError("PlayerMovement scripti bulunamadý!");
            }
        }
        else
        {
            Debug.LogError("Player objesi atanmamýþ!");
        }

        // Ana menüye geçiþ yap
        Invoke("LoadMainMenu", deathDelay);
    }

    void LoadMainMenu()
    {
        // Mouse'u görünür ve serbest býrak
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Ana menü sahnesine geçiþ yap
        SceneManager.LoadScene("Menu");
    }

    // Görüþ mesafesini ve açýsýný gösteren debug çizgileri (Opsiyonel)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Vector3 leftBoundary = Quaternion.Euler(0, -sightAngle / 2f, 0) * transform.forward * sightRange;
        Vector3 rightBoundary = Quaternion.Euler(0, sightAngle / 2f, 0) * transform.forward * sightRange;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
    }
}