using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;

public class slime : enemigo
{
	public float persecutionRadius = 5.0f;

	public float jumpForce = 120f, jumpTimerInit = 1f, jumpTimer;
    public float knockback;
    public int[] damage = new int[2];
    public float attackSpeed = 0.2f;
	private Animator ac;
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
		jumpTimer = jumpTimerInit;
		ac = GetComponent<Animator>();
    }
    new private void FixedUpdate()
    {
		base.FixedUpdate();
		float magnitude = (jugador.gameObject.transform.position - gameObject.transform.position).magnitude;
        if (magnitude <= persecutionRadius) {
			ac.SetBool("detectedPlayer", true);
			jumpTimer -= Time.fixedDeltaTime;
            if (jumpTimer <= 0f) {
				Vector2 dir = (jugador.transform.position - gameObject.transform.position).normalized;
				rb.AddForce(dir*jumpForce);
                jumpTimer = jumpTimerInit;
                allowAttack = true;
			}
			else
			{
                RaycastHit2D[] boxCast = Physics2D.BoxCastAll(transform.position, transform.localScale, 0, (jugador.transform.position - gameObject.transform.position).normalized, 0);
                foreach (RaycastHit2D collider in boxCast)
                {
                    if (collider.collider.CompareTag("jugador") && allowAttack)
                    {
                        Debug.Log("golpeado jugador !!");
                        hitPlayer(Mathf.Rad2Deg * Mathf.Atan2(jugador.transform.position.y-transform.position.y,jugador.transform.position.x-transform.position.x),damage,knockback,jumpTimerInit);
                    }
                }
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
