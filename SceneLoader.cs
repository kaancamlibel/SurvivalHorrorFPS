using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Inspector'dan sahne ad�n� ayarlayabilirsiniz
    [SerializeField] private string sceneName = "First_Scene"; // Varsay�lan olarak "First_Scene" ayarland�

    // Bu metod, butonun OnClick event'ine ba�lanacak
    public void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            Debug.Log("Sahne y�kleniyor: " + sceneName);
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Sahne ad� bo� veya atanmam��!");
        }
    }
}