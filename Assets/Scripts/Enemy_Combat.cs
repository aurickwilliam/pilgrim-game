using UnityEngine;

public class Enemy_Combat : MonoBehaviour
{
    public int damage = 10;
    public Transform attackPoint;
    public float weaponRange;
    public float knockbackForce;
    public float stunTime;
    public LayerMask playerLayer;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip attackSFX;
    public AudioClip hitSFX;

    public void Attack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, weaponRange, playerLayer);
        audioSource.PlayOneShot(attackSFX);
        if (hits.Length > 0)
        {
            hits[0].GetComponent<PlayerHealth>().ChangeHealth(-damage);
            audioSource.PlayOneShot(hitSFX);
            hits[0].GetComponent<PlayerMovement>().Knockback(transform, knockbackForce, stunTime);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, weaponRange);
    }
}