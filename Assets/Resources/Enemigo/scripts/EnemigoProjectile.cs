using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemigoProjectile : Enemy
{
    public float displSpeed;
    public float followDistance, attackDistance;
    public float attackForce;

    public float timer, timerReset = 1;
    // Start is called before the first frame update
    new void Start()
    {
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

        timer -= Time.deltaTime;
        if (distanceToPlayer <= attackDistance)
        {
            Attacking();
        }
        else if (distanceToPlayer <= followDistance)
        {
            Chase();
        }
        else
        {
            Idle();
        }
    }

    void Attacking()
    {
        if(timer <= 0)
        {
            GetComponent<ProjectileGen>().Lanzar_Projectil(transform.position, (player.gameObject.transform.position - gameObject.transform.position).normalized, damage, knockback);
            timer = timerReset;
        }
    }

    void Chase()
    {
        Vector2 direction = (player.transform.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * displSpeed, direction.y * displSpeed);
    }

    void Idle()
    {
        rb.velocity = Vector2.zero;
    }
}
