using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class Lobo : enemigo
{
	public float  displSpeed;
	public float followDistance, attackDistance;
	public float attackForce;

	public float timer, timerReset = 1;
    // Start is called before the first frame update
    new void Start()
    {
		base.Start();
    }

	void updateTimer() {
		timer -= Time.fixedDeltaTime;
	}

	void Moverse() {
		float distance = (player.transform.position - gameObject.transform.position).magnitude;
		if(distance <= attackDistance) {
			Vector2 direccion = (player.transform.position - gameObject.transform.position).normalized;
			if(timer <= 0) {
				rb.AddForce(direccion * attackForce);
				timer = timerReset;
			} else updateTimer();
		} else if(distance <= followDistance) {
			Vector2 direccion = (player.transform.position - gameObject.transform.position).normalized;
			if(timer == timerReset)
				transform.Translate(displSpeed * Time.fixedDeltaTime * direccion);
			else updateTimer();
				
		} else {
			rb.velocity = Vector2.zero;
		}
	}

    // Update is called once per frame
    new void FixedUpdate()
    {
		base.FixedUpdate();
		Moverse();
    }
}
