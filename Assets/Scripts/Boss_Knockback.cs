using System.Collections;
using UnityEngine;

public class Boss_Knockback : MonoBehaviour
{
    private Rigidbody2D rb;
    private Boss_Movement bossMovement;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bossMovement = GetComponent<Boss_Movement>();
    }

    /// <summary>
    /// Called by whatever script handles boss damage (e.g. Boss_Health).
    /// </summary>
    public void Knockback(Transform playerTransform, float knockbackForce, float knockbackTime, float stunTime)
    {
        bossMovement.ChangeState(BossState.Knockback);
        StartCoroutine(StunRoutine(knockbackTime, stunTime));
        Vector2 direction = (transform.position - playerTransform.position).normalized;
        rb.linearVelocity = direction * knockbackForce;
    }

    IEnumerator StunRoutine(float knockbackTime, float stunTime)
    {
        yield return new WaitForSeconds(knockbackTime);
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(stunTime);
        bossMovement.ChangeState(BossState.Idle);
    }
}