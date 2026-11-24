using System;
using System.Collections;

using UnityEngine;

public class Enemy_Knockback : MonoBehaviour
{
    Rigidbody2D rb;
    private Enemy_Movement enemy_Movement;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemy_Movement = GetComponent<Enemy_Movement>();
    }

    public void Knockback(Transform playerTransform, float knocbackForce, float knockbackTime, float stunTime)
    {

        enemy_Movement.ChangeState(EnemyState.Knockback);
        StartCoroutine(StunTime(knockbackTime, stunTime));
        Vector2 direction = (transform.position - playerTransform.position).normalized;
        rb.linearVelocity = direction * knocbackForce;

    }

    IEnumerator StunTime(float knockbackTime, float stunTime)
    {

        yield return new WaitForSeconds(knockbackTime);
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(stunTime);
        enemy_Movement.ChangeState(EnemyState.Idle);  
    }


}
