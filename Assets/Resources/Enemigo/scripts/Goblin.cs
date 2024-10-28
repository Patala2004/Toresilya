using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : Enemy
{
	public float  displSpeed;
	public float followDistance, attackDistance;
	public float attackForce;

	public float timer, timerReset = 1;

	[SerializeField]private bool isJumping;


    // Animacion
    Animator ani;

    // Start is called before the first frame update
    new void Start()
    {
		base.Start();
        ani = GetComponent<Animator>();
    }

	void HandleJump()
    {
		HitboxEnemy(transform.position, new(1,1), 0, (player.gameObject.transform.position - gameObject.transform.position).normalized, 0.7f, this.damage, this.knockback);
        timer -= Time.deltaTime;

        if (timer <= 0) {
            isJumping = false;
            rb.velocity = Vector2.zero;
        }
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

        if (isJumping) {
            HandleJump();
            return; 
        }

        if (distanceToPlayer <= attackDistance && timer <= timerReset) {
            Jump();
        } else if (distanceToPlayer <= followDistance) {
            Chase();
        } else {
            Idle();
        }
    }

	void Jump()
    {
        allowAttack = true;
        ani.SetTrigger("Attacking");

        isJumping = true;
        timer = timerReset;

        Vector2 jumpDirection = (player.transform.position - transform.position).normalized;
		rb.velocity = Vector2.zero;
        rb.AddForce(jumpDirection * attackForce, ForceMode2D.Impulse);
    }

	void Chase()
    {
        Vector2 direction = (player.transform.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * displSpeed, direction.y * displSpeed);

        ani.SetBool("Running", true);
        ani.SetBool("Idle", false);
    }

	void Idle() {
		rb.velocity = Vector2.zero;
        ani.SetBool("Running", false);
        ani.SetBool("Idle", true);
    }

}
