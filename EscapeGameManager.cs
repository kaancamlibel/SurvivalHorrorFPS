using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class EscapeGameManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> keyItems;
    [SerializeField] private GameObject theEndUI;
    [SerializeField] private GameObject blackBackgroundPanel;
    [SerializeField] private string escapeObjectTag = "EscapeObject";
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string menuSceneName = "Menu";

    [SerializeField] private GameObject backgroundSound;


    private Coroutine loadMenuCoroutine;

    public static EscapeGameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (theEndUI == null || blackBackgroundPanel == null)
        {
            Debug.LogError("UI öðeleri atanmamýþ! Lütfen Inspector'dan atayýn.");
        }
        else
        {
            theEndUI.SetActive(false);
            blackBackgroundPanel.SetActive(false);
        }
    }

    public void CollectKey(GameObject keyItem)
    {
        if (keyItems.Contains(keyItem))
        {
            keyItems.Remove(keyItem);
            Debug.Log($"Key Item Toplandý! Kalan Key Sayýsý: {keyItems.Count}");

            if (keyItems.Count == 0)
            {
                Debug.Log("Tüm Key'ler Toplandý!");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            if (keyItems.Count == 0 && loadMenuCoroutine == null)
            {
                backgroundSound.SetActive(false);
                loadMenuCoroutine = StartCoroutine(LoadMenuAfterDelay(10f));
            }
        }
    }

    private IEnumerator LoadMenuAfterDelay(float delay)
    {
        if (theEndUI != null && blackBackgroundPanel != null)
        {
            theEndUI.SetActive(true);
            blackBackgroundPanel.SetActive(true);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            yield return new WaitForSeconds(delay);
            SceneManager.LoadScene(menuSceneName);
        }
        else
        {
            Debug.LogError("UI öðeleri atanmamýþ!");
        }
    }
}