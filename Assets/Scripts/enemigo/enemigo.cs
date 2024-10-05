using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemigo : MonoBehaviour
{
    //TODO: implementar ataque(cuando salte que tenga hitbox)
    public jugador jugador;
    protected Rigidbody2D rb;
    protected Vector2 velImpulse = Vector2.zero;
    public bool invulnerable;
    // Start is called before the first frame update
    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        jugador = GameObject.Find("player").GetComponent<jugador>();
    }
    public void FixedUpdate()
    {
        rb.velocity += velImpulse;
    }
    // Update is called once per frame
    void Update()
    {
        if (invulnerable) { invulnerable = jugador.attacking; }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("hitboxAmigo") && !invulnerable)
        {
            invulnerable = true;
            StartCoroutine(golpeado(jugador.arma.attackSpeed,jugador.ang,jugador.arma.attackKnockback));
			int damage = (int)Random.Range(jugador.arma.attackDamage[0], jugador.arma.attackDamage[1] + 1);
            GetComponent<genParticulaTexto>().comenzar(damage,jugador.ang);
        }
    }
    IEnumerator golpeado(float waitseconds,float ang,float attackKnockback)
    { 
        StartCoroutine(impulse(jugador.arma.attackAnimation,ang,attackKnockback));
        yield return new WaitForSeconds(waitseconds);
        invulnerable = false;
    }
    IEnumerator impulse(float waitseconds, float ang, float recoil)
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
