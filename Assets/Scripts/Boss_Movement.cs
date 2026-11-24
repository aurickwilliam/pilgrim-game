using UnityEngine;

public enum BossState
{
    Idle,
    Chasing,
    Attacking,
    Dashing,
    Knockback,
    Dead
}

public enum BossPhase
{
    PhaseOne,
    PhaseTwo,
}

public class Boss_Movement : MonoBehaviour
{
    [Header("General")]
    public float speed = 3f;
    public float attackRange = 2f;
    public float attackCooldown = 2f;
    public float playerDetectRange = 8f;
    public Transform detectionPoint;
    public LayerMask playerLayer;

    [Header("Dash")]
    public float dashSpeed = 14f;
    public float dashDuration = 0.25f;
    public float dashCooldown = 4f;
    public float dashTriggerRange = 6f;

    [Header("Phase Two Multipliers")]
    public float phaseTwoSpeedMult = 1.4f;
    public float phaseTwoAttackCooldownMult = 0.6f;
    public float phaseTwoDashCooldownMult = 0.6f;
    public float phaseTwoDamageMult = 1.5f;

    // State
    private BossState bossState;
    private BossPhase bossPhase = BossPhase.PhaseOne;

    private float attackCooldownTimer;
    private float dashCooldownTimer;
    private float dashTimer;

    private int facingDirection = -1;
    private Vector2 dashDirection;

    private bool isDashing = false;
    public bool IsDashing => isDashing;

    // Components
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private Animator anim;
    private Transform player;

    private Boss_Health bossHealth;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        bossHealth = GetComponent<Boss_Health>();

        if (rb == null) Debug.LogError($"{name}: Missing Rigidbody2D.");
        if (anim == null) Debug.LogError($"{name}: Missing Animator.");
        if (bossHealth == null) Debug.LogError($"{name}: Missing Boss_Health.");

        ChangeState(BossState.Idle);
    }

    void Update()
    {
        if (bossState == BossState.Dead) return;
        if (bossState == BossState.Knockback) return;

        // Check phase two every frame directly
        if (bossPhase == BossPhase.PhaseOne && bossHealth != null && bossHealth.GetHealthPercent() <= 0.5f)
        {
            EnterPhaseTwo();
        }

        if (attackCooldownTimer > 0) attackCooldownTimer -= Time.deltaTime;
        if (dashCooldownTimer > 0) dashCooldownTimer -= Time.deltaTime;

        switch (bossState)
        {
            case BossState.Idle:
            case BossState.Chasing:
                CheckForPlayer();
                break;

            case BossState.Dashing:
                HandleDash();
                break;

            case BossState.Attacking:
                // Stay completely still and let the animation finish.
                // Only the animation event (Attack()) can exit this state.
                rb.linearVelocity = Vector2.zero;
                return;
        }
    }

    void CheckForPlayer()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(detectionPoint.position, playerDetectRange, playerLayer);

        if (hits.Length == 0)
        {
            rb.linearVelocity = Vector2.zero;
            ChangeState(BossState.Idle);
            return;
        }

        player = hits[0].transform;
        float dist = Vector2.Distance(transform.position, player.position);

        // Normal attack
        if (dist <= attackRange && attackCooldownTimer <= 0)
        {
            attackCooldownTimer = attackCooldown;
            ChangeState(BossState.Attacking);
            return;
        }

        // Dash
        if (dist > attackRange && dist <= dashTriggerRange && dashCooldownTimer <= 0)
        {
            dashCooldownTimer = dashCooldown;
            StartDash();
            return;
        }

        // Chase
        if (dist > attackRange && bossState != BossState.Attacking)
        {
            ChangeState(BossState.Chasing);
            Chase();
        }
    }

    void Chase()
    {
        if (player == null) return;
        FacePlayer();
        rb.linearVelocity = (player.position - transform.position).normalized * speed;
    }

    void StartDash()
    {
        if (player == null) return;
        FacePlayer();

        // Aim just outside attack range so the boss stops with a gap
        Vector2 dirToPlayer = (player.position - transform.position).normalized;
        Vector2 dashTarget = (Vector2)player.position - dirToPlayer * (attackRange * 0.9f);
        dashDirection = (dashTarget - (Vector2)transform.position).normalized;

        dashTimer = dashDuration;
        ChangeState(BossState.Dashing);
    }

    void HandleDash()
    {
        dashTimer -= Time.deltaTime;
        rb.linearVelocity = dashDirection * dashSpeed;

        if (dashTimer <= 0)
        {
            isDashing = false;
            rb.linearVelocity = Vector2.zero;
            ChangeState(BossState.Idle);
        }
    }

    public void EnterPhaseTwo()
    {
        if (bossPhase == BossPhase.PhaseTwo) return;
        bossPhase = BossPhase.PhaseTwo;
        speed *= phaseTwoSpeedMult;
        attackCooldown *= phaseTwoAttackCooldownMult;
        dashCooldown *= phaseTwoDashCooldownMult;

        // Apply damage multiplier to Boss_Combat
        Boss_Combat bossCombat = GetComponent<Boss_Combat>();
        if (bossCombat != null)
        {
            bossCombat.damage = Mathf.RoundToInt(bossCombat.damage * phaseTwoDamageMult);
            bossCombat.slamDamage = Mathf.RoundToInt(bossCombat.slamDamage * phaseTwoDamageMult);
            bossCombat.dashDamage = Mathf.RoundToInt(bossCombat.dashDamage * phaseTwoDamageMult);
        }

        Debug.Log($"{name}: Entered Phase Two!");
    }

    void FacePlayer()
    {
        if (player == null) return;
        bool shouldFlip = (player.position.x > transform.position.x && facingDirection == -1) ||
                          (player.position.x < transform.position.x && facingDirection == 1);
        if (shouldFlip) Flip();
    }

    void Flip()
    {
        facingDirection *= -1;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void ChangeState(BossState newState)
    {
        // Exit current animation
        if (bossState == BossState.Idle) anim.SetBool("isIdle", false);
        else if (bossState == BossState.Chasing) anim.SetBool("isChasing", false);
        else if (bossState == BossState.Attacking) anim.SetBool("isAttacking", false);
        else if (bossState == BossState.Dashing) anim.SetBool("isChasing", false); // reuses run anim

        bossState = newState;
        isDashing = (bossState == BossState.Dashing);

        // Enter new animation
        if (bossState == BossState.Idle) anim.SetBool("isIdle", true);
        else if (bossState == BossState.Chasing) anim.SetBool("isChasing", true);
        else if (bossState == BossState.Attacking) anim.SetBool("isAttacking", true);
        else if (bossState == BossState.Dashing) anim.SetBool("isChasing", true); // reuses run anim
    }

    public BossPhase GetPhase() => bossPhase;

    public void Die()
    {
        isDashing = false;
        rb.linearVelocity = Vector2.zero;
        ChangeState(BossState.Dead);
    }

    private void OnDrawGizmosSelected()
    {
        if (detectionPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(detectionPoint.position, playerDetectRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(detectionPoint.position, dashTriggerRange);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}