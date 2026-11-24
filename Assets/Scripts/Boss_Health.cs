using UnityEngine;

public class Boss_Health : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;

    [Header("Health Bar")]
    public EnemyHealthBar healthBar;

    private Boss_Movement bossMovement;
    private bool phaseTransitioned = false;

    private void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetHealth(currentHealth, maxHealth);
        bossMovement = GetComponent<Boss_Movement>();
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
            BossManager.Instance.OnBossDied();
            Debug.Log("Boss Died");
            healthBar.SetHealth(0, maxHealth);
            bossMovement?.Die();
            Destroy(gameObject);
            return;
        }

        // Phase two is handled by Boss_Movement checking health every frame
        if (!phaseTransitioned && GetHealthPercent() <= 0.5f)
        {
            phaseTransitioned = true;
        }

        healthBar.SetHealth(currentHealth, maxHealth);
    }

    public float GetHealthPercent() => (float)currentHealth / maxHealth;
}