using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreenManager : MonoBehaviour
{
    public void RetryLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(PlayerPrefs.GetInt("lastLevel")); // loads the level the player died in
    }

    public void ExitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
