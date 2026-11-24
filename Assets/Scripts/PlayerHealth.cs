using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerHealth : MonoBehaviour
{
   public int maxHealth;
   public int currentHealth;

    public HealthBar healthBar;



    public void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    public void ChangeHealth(int amount)
    {
        currentHealth += amount;


        if (currentHealth <= 0)
        {
            PlayerPrefs.SetInt("lastLevel", SceneManager.GetActiveScene().buildIndex); // 👈
            gameObject.SetActive(false);
            SceneManager.LoadScene("DeathScreen");
        }
        
        healthBar.SetHealth(currentHealth);

    }
}
