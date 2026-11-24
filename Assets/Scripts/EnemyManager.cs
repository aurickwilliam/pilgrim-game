using System.Collections;
using UnityEngine;
using TMPro;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    private int totalEnemies;
    private int enemiesAlive;
    public bool allEnemiesDefeated = false; 
    [Header("Objective UI")]
    public TextMeshProUGUI objectiveText;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        enemiesAlive = GameObject.FindGameObjectsWithTag("Enemy").Length;
        totalEnemies = enemiesAlive;
        UpdateObjectiveText();
    }

    public void OnEnemyDied()
    {
        enemiesAlive--;
        UpdateObjectiveText();

        if (enemiesAlive == 0)
        {
            allEnemiesDefeated = true;
            Debug.Log("All enemies defeated!");

        }
    }

    private void UpdateObjectiveText()
    {
        if (objectiveText != null)
        {
            int killed = totalEnemies - enemiesAlive;
            objectiveText.text = $"Enemies: {killed} / {totalEnemies}";
        }
    }
}