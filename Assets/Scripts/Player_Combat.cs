using UnityEngine;

public class Player_Combat : MonoBehaviour
{
    public Transform attackPoint;
    public float weaponRange = 1;
    public float knockbackForce = 50;
    public float knockbackTime = .15f;
    public float stunTime = .3f;
    public LayerMask enemyLayer;
    public int damage = 1;
    public Animator anim;
    public float cooldown = 1;
    private float timer;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip attackSFX;
    public AudioClip hitSFX;

    public void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
    }

    public void Attack()
    {
        if (timer <= 0)
        {
            anim.SetBool("isAttacking", true);
            audioSource.PlayOneShot(attackSFX);
            timer = cooldown;
        }
    }

    public void DealDamage()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPoint.position, weaponRange, enemyLayer);
        if (enemies.Length > 0)
        {
            // Health — works on both regular enemies and the boss
            Enemy_Health enemyHealth = enemies[0].GetComponent<Enemy_Health>();
            Boss_Health bossHealth = enemies[0].GetComponent<Boss_Health>();

            if (enemyHealth != null) enemyHealth.ChangeHealth(-damage);
            else if (bossHealth != null) bossHealth.ChangeHealth(-damage);

            // Knockback — works on both regular enemies and the boss
            Enemy_Knockback enemyKnockback = enemies[0].GetComponent<Enemy_Knockback>();
            Boss_Knockback bossKnockback = enemies[0].GetComponent<Boss_Knockback>();

            if (enemyKnockback != null) enemyKnockback.Knockback(transform, knockbackForce, knockbackTime, stunTime);
            else if (bossKnockback != null) bossKnockback.Knockback(transform, knockbackForce, knockbackTime, stunTime);

            if (hitSFX != null)
                audioSource.PlayOneShot(hitSFX);
        }
    }

    public void FinishAttacking()
    {
        anim.SetBool("isAttacking", false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, weaponRange);
    }
}