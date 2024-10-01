using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class arma : MonoBehaviour
{
    public jugador jugador;
    Animator ani;
    SpriteRenderer sR;
    public float recoil = 300f;
    public float attackKnockback = 1.5f;
    public float attackSpeed = 0.2f;
    public float attackAlter = 90;
	public float[] attackDamage = new float[2];
    // Start is called before the first frame update
    void Start()
    {
        ani = GetComponent<Animator>();
        sR = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (jugador.ang > 90 || jugador.ang < -90)
        {
            if(attackAlter == 90) { sR.flipY = true; }
            else { sR.flipY = false; }
            float x = Mathf.Cos((jugador.ang) * Mathf.Deg2Rad) * 0.3f;
            float y = Mathf.Sin((jugador.ang) * Mathf.Deg2Rad) * 0.3f;
            transform.localPosition = new Vector3(x, y, 1);
            transform.localEulerAngles = new Vector3(0, 0, jugador.ang + attackAlter);
        }
        else
        {
            if (attackAlter != 90) { sR.flipY = true; }
            else { sR.flipY = false; }
            float x = Mathf.Cos((jugador.ang) * Mathf.Deg2Rad) * 0.3f;
            float y = Mathf.Sin((jugador.ang) * Mathf.Deg2Rad) * 0.3f;
            transform.localPosition = new Vector3(x, y, 1);
            transform.localEulerAngles = new Vector3(0, 0, jugador.ang - attackAlter);
        }
    }
    public void comenzar()
    {
        ani.speed = attackSpeed * 5;
        ani.Play("Aespada");
    }
    public void finalizar()
    {
        attackAlter = -attackAlter;
    }

}
