using UnityEngine;

public class KeyItem : MonoBehaviour
{
    [SerializeField] private float interactionDistance = 5f; // Etkile�im mesafesi
    [SerializeField] private string keyItemTag = "KeyItem"; // Key item'lar�n tag'i

    private EscapeGameManager escapeGameManager;

    private void Start()
    {
        escapeGameManager = FindObjectOfType<EscapeGameManager>();
        if (escapeGameManager == null)
        {
            Debug.LogError("EscapeGameManager bulunamad�!");
        }
    }

    void Update()
    {
        // E tu�una bas�ld���nda
        if (Input.GetKeyDown(KeyCode.E))
        {
            CheckForKeyItem();
        }
    }

    // Raycast ile key item'� kontrol et
    private void CheckForKeyItem()
    {
        if (Camera.main == null)
        {
            Debug.LogError("Main Camera bulunamad�!");
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

    // Key item'� topla
    private void CollectKeyItem(GameObject keyItem)
    {
        if (keyItem == null)
        {
            Debug.LogError("Key Item bulunamad�!");
            return;
        }

        // Key item'� yok et
        Destroy(keyItem);
        Debug.Log("Key Item Topland�!");

        // EscapeGameManager scriptindeki CollectKey metodunu �a��r
        if (escapeGameManager != null)
        {
            escapeGameManager.CollectKey(keyItem);
        }
        else
        {
            Debug.LogError("EscapeGameManager bulunamad�!");
        }
    }
}