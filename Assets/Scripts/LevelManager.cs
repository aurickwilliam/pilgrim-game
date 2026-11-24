using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public CanvasGroup fadePanel;
    public float fadeDuration = 1f;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        fadePanel.alpha = 0f;
        fadePanel.blocksRaycasts = false;
    }

    public void LoadNextLevel()
    {
        StartCoroutine(LoadNextLevelRoutine());
    }

    IEnumerator LoadNextLevelRoutine()
    {
        yield return StartCoroutine(FadeToBlack());

        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextIndex);
        }
        else
        {
            SceneManager.LoadScene("WinScreen");
        }
    }

    IEnumerator FadeToBlack()
    {
        fadePanel.blocksRaycasts = true;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadePanel.alpha = Mathf.Clamp01(timer / fadeDuration);
            yield return null;
        }

        fadePanel.alpha = 1f;
    }
}