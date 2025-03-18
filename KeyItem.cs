using UnityEngine;

public class KeyItem : MonoBehaviour
{
    [SerializeField] private float interactionDistance = 5f; // Etkileþim mesafesi
    [SerializeField] private string keyItemTag = "KeyItem"; // Key item'larýn tag'i

    private EscapeGameManager escapeGameManager;

    private void Start()
    {
        escapeGameManager = FindObjectOfType<EscapeGameManager>();
        if (escapeGameManager == null)
        {
            Debug.LogError("EscapeGameManager bulunamadý!");
        }
    }

    void Update()
    {
        // E tuþuna basýldýðýnda
        if (Input.GetKeyDown(KeyCode.E))
        {
            CheckForKeyItem();
        }
    }

    // Raycast ile key item'ý kontrol et
    private void CheckForKeyItem()
    {
        if (Camera.main == null)
        {
            Debug.LogError("Main Camera bulunamadý!");
            return;
        }

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (hit.collider.CompareTag(keyItemTag))
            {
                CollectKeyItem(hit.collider.gameObject);
            }
        }
    }

    // Key item'ý topla
    private void CollectKeyItem(GameObject keyItem)
    {
        if (keyItem == null)
        {
            Debug.LogError("Key Item bulunamadý!");
            return;
        }

        // Key item'ý yok et
        Destroy(keyItem);
        Debug.Log("Key Item Toplandý!");

        // EscapeGameManager scriptindeki CollectKey metodunu çaðýr
        if (escapeGameManager != null)
        {
            escapeGameManager.CollectKey(keyItem);
        }
        else
        {
            Debug.LogError("EscapeGameManager bulunamadý!");
        }
    }
}