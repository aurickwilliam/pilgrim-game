using UnityEngine;

public class Enemy_Health : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;

    [Header("Health Bar")]
    public EnemyHealthBar healthBar; 


    private void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetHealth(currentHealth, maxHealth);
    }


    public void ChangeHealth(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else if (currentHealth <= 0)
        {

            Debug.Log("Enemy Died");
            EnemyManager.Instance.OnEnemyDied();
            Destroy(gameObject);
            return;
        }

         healthBar.SetHealth(currentHealth, maxHealth);
    }
}
