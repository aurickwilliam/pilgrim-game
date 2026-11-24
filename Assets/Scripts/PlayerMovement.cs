using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5;
    public int facingDirection = 1;
    public Rigidbody2D rb;
    public Animator anim;
    public Player_Combat player_Combat;

    [Header("Dash")]
    public float dashSpeed = 18f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 1f;

    private bool isKnockback;
    private bool isDashing;
    private float dashCooldownTimer;
    private Vector2 lastMoveDirection = Vector2.right;

    private void Update()
    {
        if (Input.GetButtonDown("Slash"))
            player_Combat.Attack();

        if (dashCooldownTimer > 0)
            dashCooldownTimer -= Time.deltaTime;

        if (Input.GetButtonDown("Dash") && dashCooldownTimer <= 0 && !isDashing)
            StartCoroutine(Dash());
    }

    void FixedUpdate()
    {
        if (isDashing || isKnockback)
            return;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector2 moveInput = new Vector2(horizontal, vertical);
        if (moveInput.sqrMagnitude > 0.01f)
            lastMoveDirection = moveInput.normalized;

        if (horizontal > 0 && transform.localScale.x < 0 ||
            horizontal < 0 && transform.localScale.x > 0)
            Flip();

        anim.SetFloat("horizontal", Mathf.Abs(horizontal));
        anim.SetFloat("vertical", Mathf.Abs(vertical));
        rb.linearVelocity = moveInput * speed;
    }

    IEnumerator Dash()
    {
        isDashing = true;
        dashCooldownTimer = dashCooldown;
        anim.SetBool("isDashing", true); // add this
        rb.linearVelocity = lastMoveDirection * dashSpeed;
        yield return new WaitForSeconds(dashDuration);
        rb.linearVelocity = Vector2.zero;
        isDashing = false;
        anim.SetBool("isDashing", false); // add this
    }

    public void DashButton()
    {
        if (dashCooldownTimer <= 0 && !isDashing)
            StartCoroutine(Dash());
    }

    void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(
            transform.localScale.x * -1,
            transform.localScale.y,
            transform.localScale.z);
    }

    public void Knockback(Transform enemy, float force, float stunTime)
    {
        isKnockback = true;
        Vector2 direction = (transform.position - enemy.position).normalized;
        rb.linearVelocity = direction * force;
        StartCoroutine(KnockbackCounter(stunTime));
    }

    IEnumerator KnockbackCounter(float stunTime)
    {
        yield return new WaitForSeconds(stunTime);
        rb.linearVelocity = Vector2.zero;
        isKnockback = false;
    }
}