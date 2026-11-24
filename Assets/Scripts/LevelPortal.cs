using UnityEngine;

public class LevelPortal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (EnemyManager.Instance.allEnemiesDefeated)
            {
                Debug.Log("Portal activated! Loading next level...");
                LevelManager.Instance.LoadNextLevel();
            }
            else
            {
                Debug.Log("Defeat all enemies first!");
            }
        }
    }
}
