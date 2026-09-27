using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultSceneUI : MonoBehaviour
{
    public void Retry()
    {
        SceneManager.LoadScene("Level 1");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
