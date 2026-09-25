using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSoul : MonoBehaviour
{
    [Header("Soul")]
    public int currentSouls = 0;
    public int requiredSouls = 10;

    private bool loadingNextLevel = false;

    public void AddSouls(int amount)
    {
        currentSouls += amount;

        Debug.Log("Souls: " + currentSouls + "/" + requiredSouls);

        if (currentSouls >= requiredSouls)
        {
            GoToNextLevel();
        }
    }

    private void GoToNextLevel()
    {
        if (loadingNextLevel)
            return;

        loadingNextLevel = true;

        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        SceneManager.LoadScene(nextSceneIndex);
    }
}
