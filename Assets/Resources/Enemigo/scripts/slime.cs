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
    }
    private void FixedUpdate()
    {
		float magnitude = (jugador.gameObject.transform.position - gameObject.transform.position).magnitude;
        if(magnitude <= persecutionRadius) {
			ac.SetBool("detectedPlayer", true);
			jumpTimer -= Time.fixedDeltaTime;
			
			if(jumpTimer <= 0f) {
				Vector2 dir = (jugador.transform.position - gameObject.transform.position).normalized;
				rb.AddForce(dir*jumpForce);

				jumpTimer = jumpTimerInit;
			}
		} else {
			ac.SetBool("detectedPlayer", false);
			jumpTimer = jumpTimerInit;
		}
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    /*private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("hitboxAmigo"))
        {
            Debug.Log("golpeado!!");
            StartCoroutine(golpeado(jugador.arma.attackSpeed,jugador.ang, jugador.arma.recoil,jugador.arma.attackKnockback));
        }
    }
    IEnumerator golpeado(float waitseconds,float ang,float recoil,float attackKnockback)
    {
        float elapsedTime = 0;
        Vector2 dir = new Vector3(Mathf.Cos(ang * Mathf.Deg2Rad), Mathf.Sin(ang * Mathf.Deg2Rad));
        while (elapsedTime < waitseconds)
        {
            vel2 = attackKnockback * recoil * (dir * (1 - elapsedTime / waitseconds));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        vel2 = Vector2.zero;
    }*/
}
