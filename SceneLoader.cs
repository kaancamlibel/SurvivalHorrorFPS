using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Inspector'dan sahne adýný ayarlayabilirsiniz
    [SerializeField] private string sceneName = "First_Scene"; // Varsayýlan olarak "First_Scene" ayarlandý

    // Bu metod, butonun OnClick event'ine baðlanacak
    public void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            Debug.Log("Sahne yükleniyor: " + sceneName);
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Sahne adý boþ veya atanmamýþ!");
        }
    }
}