using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;

public class slime : enemigo
{
    public float persecutionRadius = 5.0f;
	public float jumpForce = 120f, jumpTimerInit = 1f, jumpTimer;
	private Animator ac;
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
		jumpTimer = jumpTimerInit;
		ac = GetComponent<Animator>();
        attackSpeed = jumpTimerInit;
    }
    new private void FixedUpdate()
    {
		base.FixedUpdate();
		float magnitude = (player.gameObject.transform.position - gameObject.transform.position).magnitude;
        if (magnitude <= persecutionRadius) {
			ac.SetBool("detectedPlayer", true);
			jumpTimer -= Time.fixedDeltaTime;
            if (jumpTimer <= 0f) {
				Vector2 dir = (player.transform.position - gameObject.transform.position).normalized;
				rb.AddForce(dir*jumpForce);
                jumpTimer = jumpTimerInit;
                allowAttack = true;
			}
			else // empieza a saltar
			{
                hitboxEnemigo(transform.position, transform.localScale, 0, (player.gameObject.transform.position - gameObject.transform.position), 0, this.damage, this.knockback, this.attackSpeed);
            }
		} else {
			ac.SetBool("detectedPlayer", false);
			jumpTimer = jumpTimerInit;
		}
    }
    // Update is called once per frame
    void Update()
    {
		base.Update();
    }
}
