using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemigoProjectile : Enemy
{
    public float displSpeed;
    public float followDistance, attackDistance, jumpForce = 120f, jumpTimerInit = 1f, jumpTimer;
    public float attackForce;

    public float timer, timerReset = 1;
    // Start is called before the first frame update
    new void Start()
    {
        jumpTimer = jumpTimerInit;
        base.Start();
    }

    new void Update()
    {
        base.Update();
        DoFlipX(GetComponent<SpriteRenderer>(), (player.transform.position - transform.position).normalized);
    }
    // Update is called once per frame
    new void FixedUpdate()
    {
        base.FixedUpdate();
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= attackDistance)
        {
            Attacking();
        }
        else if (distanceToPlayer <= followDistance)
        {
            timer = timerReset;
            Chase();
        }
        else
        {
            Idle();
        }
    }

    void Attacking()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            GetComponent<ProjectileGen>().Lanzar_Projectil(transform.position, (player.gameObject.transform.position - gameObject.transform.position).normalized, damage, knockback);
            timer = timerReset;
        }
    }

    void Chase()
    {
        jumpTimer -= Time.deltaTime;
        if (jumpTimer <= 0f)
        {
            Vector2 dir = (player.transform.position - gameObject.transform.position).normalized;
            rb.AddForce(dir * jumpForce);
            jumpTimer = jumpTimerInit;
            allowAttack = true;
        }
    }

    void Idle()
    {
        rb.velocity = Vector2.zero;
    }
}
