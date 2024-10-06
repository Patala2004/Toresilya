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
    public bool allowAttack = true; // booleano que te deja atacar
    public int health = 15;
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
    public void Update()
    {
        if (invulnerable) { invulnerable = jugador.attacking; }
        if(health <= 0)
        {
            toDie();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("hitboxAmigo") && !invulnerable)
        {
            int damage = Random.Range(jugador.arma.attackDamage[0], jugador.arma.attackDamage[1] + 1);
            health -= damage;
            GetComponent<genParticulaTexto>().comenzar(damage, jugador.ang);
            StartCoroutine(golpeado(jugador.arma.attackSpeed,jugador.ang,jugador.arma.attackKnockback));
            invulnerable = true;
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
    //funciones que se llaman para atacar al jugador
    public void hitPlayer(float ang,int[] damage,float knockback,float attackSpeed)
    {
        allowAttack = false;
        StartCoroutine(esperar(attackSpeed));
        int dm = Random.Range(damage[0], damage[1] + 1);
        jugador.health -= dm;
        StartCoroutine(jugador.impulse(0.2f, ang, knockback));
    }
    IEnumerator esperar(float waitseconds)
    {
        yield return new WaitForSeconds(waitseconds);
        allowAttack = true;
    }
    // funcion muerte
    public virtual void toDie()
    {
        Destroy(this.gameObject);
    }
}
