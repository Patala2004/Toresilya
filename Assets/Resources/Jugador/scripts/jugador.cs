using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class jugador : MonoBehaviour
{
    //TODO: implementar parry (probar con slime con impulso 20000)
    //TODO: implementar dash (que no suba mucho la velocidad de movimiento(pequeño impulso+animacion))
    public arma arma;
    public hitbox hitbox;
    public float vel = 5f; //velocidad jugador
    public float ang; // angulo en grados respecto al cursor (0-180,-0-180)
    public bool attacking = false; // cuando estoy atacando
    public bool blocking = false;
    Vector2 velImpulse;
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
        rb.velocity = moverse() + velImpulse; // la velocidad del movimiento mas la del recoil del arma
    }
    private void Update()
    {
        //atacar
        if (Input.GetMouseButtonDown(0) && !attacking)
        {
            StartCoroutine(attack(arma.attackSpeed));
        }
        //block
        if (Input.GetMouseButton(1) && !attacking)
        {
            block();
        }
        else
        {
            blocking = false;
        }
        //direccion en la que esta mirando el personaje
        ang = angulo();
        if (ang > 90 || ang < -90)
        {
            sR.flipX = true;
        }
        else
        {
            sR.flipX = false;
        }
        //animacion
        if (attacking)
        {
            ani.SetFloat("velocity", 0);
        }
        else { 
            ani.SetFloat("velocity", moverse().magnitude); 
        }
    }
    //Funcion que calcula en angulo respecto al cursor
    public float angulo()
    {
        Vector2 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float adyacente = MousePos.x - transform.position.x;
        float opuesto = MousePos.y - transform.position.y ;
        return Mathf.Rad2Deg*(Mathf.Atan2(opuesto,adyacente));
    }
    //Funcion que devuelve el vector2 del mocvimiento del jugador
    public Vector2 moverse()
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
    //Funcion atacar
    IEnumerator attack(float waitseconds) // funcion ataque
    {
        attacking = true;
        arma.attack();
        hitbox.comenzar();
        StartCoroutine(impulse(arma.attackAnimation, ang, arma.recoil)); // calcular el impulso(mejor con el tiempo de la animacion)
        yield return new WaitForSeconds(waitseconds);
        attacking = false;
    }
    //Funcion block
    public void block()
    {
        arma.block();
        blocking = true;
    }
    //Funcion que añade un impulso en una direccion
    IEnumerator impulse(float waitseconds,float ang,float recoil) // funcion que calcula el impulso
    {
        float elapsedTime = 0;
        Vector2 dir = new Vector3(Mathf.Cos(ang * Mathf.Deg2Rad), Mathf.Sin(ang * Mathf.Deg2Rad));
        while (elapsedTime < waitseconds)
        {
            velImpulse = recoil * (dir * (1 - elapsedTime / waitseconds));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        velImpulse = Vector2.zero;
    }
}
