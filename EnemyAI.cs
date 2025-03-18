using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Transform player; // Player'�n transformu
    [SerializeField] private float sightRange = 10f; // G�r�� mesafesi
    [SerializeField] private float sightAngle = 90f; // G�r�� a��s� (derece cinsinden)
    [SerializeField] private LayerMask obstacleMask; // Engelleri belirten layer mask
    [SerializeField] private Transform[] patrolPoints; // Hedef noktalar� tutacak dizi

    [SerializeField] private GameObject deathPanel; // Siyah ekran paneli
    [SerializeField] private Text deathText; // "Died" yaz�s�
    [SerializeField] private float deathDelay = 3f; // Ana men�ye d�nme s�resi

    private NavMeshAgent agent;
    private Animator animator;
    private bool isPlayerInSight = false;
    private int currentPatrolIndex = 0; // �u anki hedef nokta indeksi
    private bool isPlayerDead = false; // Player'�n �l�p �lmedi�ini kontrol eder

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip chaseSound;

    [SerializeField] private GameObject backgroundSound; // Background m�zik objesi

    private bool isChasing = false; // Takip sesinin oynat�l�p oynat�lmad���n� kontrol eder

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Null kontrolleri
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent bile�eni bulunamad�!");
        }

        if (animator == null)
        {
            Debug.LogError("Animator bile�eni bulunamad�!");
        }

        if (player == null)
        {
            Debug.LogError("Player objesi atanmam��!");
        }

        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            Debug.LogError("Hedef noktalar� (patrolPoints) atanmam�� veya bo�!");
        }
        else
        {
            // �lk hedef noktaya git
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }

        // Ba�lang��ta �l�m ekran�n� gizle
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
            // Player'� takip et
            agent.SetDestination(player.position);

            // E�er ses daha �nce �almad�ysa, �almaya ba�la
            if (!isChasing)
            {
                audioSource.PlayOneShot(chaseSound);
                isChasing = true;
            }

            // H�z� g�ncelle
            float speed = agent.velocity.magnitude;
            animator.SetFloat("speed", speed);

            // Ko�ma durumunu kontrol et
            animator.SetBool("isRunning", speed > 0.01f);
        }
        else
        {
            // Player'� g�rm�yorsa, patrol noktalar� aras�nda hareket et
            Patrol();

            // Takip bitti�inde boolean s�f�rlan�r, b�ylece tekrar ba�lad���nda ses yeniden �al�nabilir
            isChasing = false;
        }

        // Player'a de�di�inde �l�m ekran�n� g�ster (sadece bir kez tetikle)
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

        // Hedef noktaya ula��ld� m�?
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            // Bir sonraki hedef noktaya ge�
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }

        // H�z� g�ncelle
        float speed = agent.velocity.magnitude;
        animator.SetFloat("speed", speed);

        // Patrol s�ras�nda sadece walk animasyonu oynat
        animator.SetBool("isRunning", false); // Walk animasyonu
    }

    void CheckPlayerInSight()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        // Player g�r�� mesafesi i�inde mi?
        if (distanceToPlayer <= sightRange)
        {
            // Player g�r�� a��s� i�inde mi?
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
            if (angleToPlayer <= sightAngle / 2f)
            {
                // Raycast ile engel kontrol�
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

        // �l�m ekran�n� g�ster
        if (deathPanel != null)
        {
            deathPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("Death Panel atanmam��!");
        }

        if (deathText != null)
        {
            deathText.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("Death Text atanmam��!");
        }

        // Player'�n hareketini durdur
        if (player != null)
        {
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.enabled = false;
            }
            else
            {
                Debug.LogError("PlayerMovement scripti bulunamad�!");
            }
        }
        else
        {
            Debug.LogError("Player objesi atanmam��!");
        }

        // Ana men�ye ge�i� yap
        Invoke("LoadMainMenu", deathDelay);
    }

    void LoadMainMenu()
    {
        // Mouse'u g�r�n�r ve serbest b�rak
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Ana men� sahnesine ge�i� yap
        SceneManager.LoadScene("Menu");
    }

    // G�r�� mesafesini ve a��s�n� g�steren debug �izgileri (Opsiyonel)
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