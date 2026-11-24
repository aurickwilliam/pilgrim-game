using UnityEngine;

public class Boss_Combat : MonoBehaviour
{
    [Header("Normal Attack")]
    public int damage = 20;
    public Transform attackPoint;
    public float weaponRange = 1.5f;
    public float knockbackForce = 8f;
    public float stunTime = 0.4f;

    [Header("AOE Slam (Phase Two only)")]
    public int slamDamage = 35;
    public Transform slamPoint;
    public float slamRadius = 3f;
    public float slamKnockbackForce = 14f;
    public float slamStunTime = 0.8f;
    public float slamCooldown = 6f;

    [Header("Dash Contact Damage")]
    public bool dashDealsContact = true;
    public int dashDamage = 15;
    public float dashKnockbackForce = 10f;
    public float dashStunTime = 0.3f;

    [Header("Layer")]
    public LayerMask playerLayer;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip attackSFX;
    public AudioClip hitSFX;
    public AudioClip slamSFX;
    public AudioClip dashHitSFX;

    private float slamCooldownTimer;
    private Boss_Movement bossMovement;

    void Start()
    {
        bossMovement = GetComponent<Boss_Movement>();
        if (bossMovement == null)
            Debug.LogError($"{name}: Boss_Combat requires Boss_Movement on the same GameObject.");
    }

    void Update()
    {
        if (slamCooldownTimer > 0) slamCooldownTimer -= Time.deltaTime;
    }

    public void Attack()
    {
        PlaySFX(attackSFX);

        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, weaponRange, playerLayer);
        if (hits.Length > 0)
        {
            DamagePlayer(hits[0], damage, knockbackForce, stunTime);
            PlaySFX(hitSFX);
        }

          // bossMovement.ChangeState(BossState.Idle);
    }

    public void SlamAttack()
    {
        if (bossMovement.GetPhase() != BossPhase.PhaseTwo) return;
        if (slamCooldownTimer > 0) return;

        slamCooldownTimer = slamCooldown;
        PlaySFX(slamSFX);

        Collider2D[] hits = Physics2D.OverlapCircleAll(slamPoint.position, slamRadius, playerLayer);
        foreach (Collider2D hit in hits)
            DamagePlayer(hit, slamDamage, slamKnockbackForce, slamStunTime);

        // bossMovement.ChangeState(BossState.Idle);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!dashDealsContact) return;

        // Bug 1 fix: use the dedicated IsDashing property instead of animator bool
        if (bossMovement == null || !bossMovement.IsDashing) return;

        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            DamagePlayer(other, dashDamage, dashKnockbackForce, dashStunTime);
            PlaySFX(dashHitSFX);
        }
    }

    void DamagePlayer(Collider2D playerCollider, int dmg, float kbForce, float stun)
    {
        playerCollider.GetComponent<PlayerHealth>()?.ChangeHealth(-dmg);
        playerCollider.GetComponent<PlayerMovement>()?.Knockback(transform, kbForce, stun);
    }

    void PlaySFX(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, weaponRange);
        }
        if (slamPoint != null)
        {
            Gizmos.color = new Color(1f, 0.5f, 0f);
            Gizmos.DrawWireSphere(slamPoint.position, slamRadius);
        }
    }
}