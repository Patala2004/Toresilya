using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : Enemy
{
	public float  displSpeed;
    public float displTemp;
	public float followDistance, attackDistance;
	public float attackForce;

	public float timer, timerReset = 1;



    // Animacion
    Animator ani;

    // Start is called before the first frame update
    new void Start()
    {
		base.Start();
        ani = GetComponent<Animator>();
        allowAttack = true;
        displTemp = displSpeed;
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
        //Timers
        timer -= Time.deltaTime;
        if(displTemp < displSpeed)
        {
            displTemp += Time.deltaTime * displSpeed/2;
        }

        if (distanceToPlayer <= attackDistance) {
            Jump();
        } else if (distanceToPlayer <= followDistance) {
            Chase();
        } else {
            Idle();
        }
    }

	void Jump()
    {
        Vector2 dir = (player.gameObject.transform.position - gameObject.transform.position).normalized;
        rb.velocity = new Vector2(dir.x * (displTemp/2), dir.y * (displTemp / 2));
        if (timer <= 0)
        {
            ani.SetTrigger("Attacking");
            allowAttack = true;
            HitboxEnemy(transform.position, new(1, 2), Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x), dir, 1, this.damage, this.knockback);
            rb.AddForce(dir * attackForce, ForceMode2D.Impulse);
            timer = timerReset;
            displTemp = 0;
        }
    }

	void Chase()
    {
        Vector2 direction = (player.transform.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * displTemp, direction.y * displTemp);
        ani.SetBool("Running", true);
        ani.SetBool("Idle", false);
    }

	void Idle() {
		rb.velocity = Vector2.zero;
        ani.SetBool("Running", false);
        ani.SetBool("Idle", true);
    }

}
