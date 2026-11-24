using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
    public float cutsceneDuration = 83f;
    public bool isCredits = false; 

    void Start()
    {
        StartCoroutine(PlayCutscene());
    }

    IEnumerator PlayCutscene()
    {
        yield return new WaitForSeconds(cutsceneDuration);
        LoadNext();
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            StopAllCoroutines();
            LoadNext();
        }
    }

    void LoadNext()
    {
        if (isCredits)
        {
            SceneManager.LoadScene("MainMenu"); 
        }
        else
        {
            int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
            SceneManager.LoadScene(nextIndex);
        }
    }
}