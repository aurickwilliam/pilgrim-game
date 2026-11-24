using UnityEngine;

public class BossManager : MonoBehaviour
{
    public static BossManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public void OnBossDied()
    {
        Debug.Log("Boss defeated! Loading next level...");
        LevelManager.Instance.LoadNextLevel();
    }
}
