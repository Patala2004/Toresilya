using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class jugador : MonoBehaviour
{
    public arma arma;
    public hitbox hitbox;
    public float vel = 5f; //velocidad jugador
    public float ang; // angulo en grados respecto al cursor (0-180,-0-180)
    public bool attacking = false; // cuando estoy atacando
    Vector2 vel2;
    Rigidbody2D rb;
    SpriteRenderer sR;
    Animator ani;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sR = rb.GetComponent<SpriteRenderer>();
        ani = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.velocity = moverse() + vel2; // la velocidad del movimiento mas la del recoil del arma
    }
    private void Update()
    {
        //atacar
        if (Input.GetMouseButtonDown(0) && !attacking)
        {
            StartCoroutine(atacar(arma.attackSpeed));
        }
        ang = angulo();
        //direccion en la que esta mirando el personaje
        if (ang > 90 || ang < -90)
        {
            sR.flipX = true;
        }
        else
        {
            sR.flipX = false;
        }
        if (attacking)
        {
            ani.SetFloat("velocity", 0);
        }
        else { 
            ani.SetFloat("velocity", moverse().magnitude); 
        }
    }
    public float angulo() // calcular angulo respecto al cursor
    {
        Vector2 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float adyacente = MousePos.x - transform.position.x;
        float opuesto = MousePos.y - transform.position.y ;
        return Mathf.Rad2Deg*(Mathf.Atan2(opuesto,adyacente));
    }
    public Vector2 moverse() // calcular el movimiento
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector2 dir = new Vector2(x, y);
        if (dir.magnitude > 1)
        {
            dir.Normalize();
        }
        return (dir * vel);
    }
    IEnumerator atacar(float waitseconds) // funcion ataque
    {
        attacking = true;
        arma.comenzar();
        hitbox.comenzar();
        float elapsedTime = 0;
        Vector2 dir = new Vector3(Mathf.Cos(ang * Mathf.Deg2Rad), Mathf.Sin(ang * Mathf.Deg2Rad));
        while (elapsedTime < waitseconds)
        {
            vel2 = arma.recoil * (dir * (1 - elapsedTime / waitseconds));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        arma.finalizar();
        hitbox.finalizar();
        vel2 = Vector2.zero;
        attacking = false;
    }
    
}
