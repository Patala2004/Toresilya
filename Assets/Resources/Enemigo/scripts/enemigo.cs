using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class enemigo : MonoBehaviour
{
    //TODO: implementar ataque(cuando salte que tenga hitbox)
    public Player player;
    protected Rigidbody2D rb;
    protected Vector2 velImpulse = Vector2.zero;
    public bool invulnerable;
    public bool allowAttack = true; // booleano que te deja atacar
    public float health = 15;
    public float knockback;
    public float knockbackResistance; // 0 sin resistencia 100 con resistencia
    public int[] damage = new int[2];
    public float attackSpeed = 0.2f;

    public Room room;
    // Start is called before the first frame update
    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("player").GetComponent<Player>();
    }
    public void FixedUpdate()
    {
        rb.velocity += velImpulse;
    }
    // Update is called once per frame
    public void Update()
    {
        // vida control
        if (invulnerable) { invulnerable = player.attacking; }
        if(health <= 0)
        {
            toDie();
        }
    }
    public IEnumerator golpeado(float waitseconds,float ang,float attackKnockback)
    { 
        StartCoroutine(impulse(player.sword.attackAnimation,ang,attackKnockback));
        yield return new WaitForSeconds(waitseconds);
        invulnerable = false;
    }
    //funcion impulso
    public IEnumerator impulse(float waitseconds, float ang, float recoil)
    {
        float elapsedTime = 0;
        Vector2 dir = new Vector3(Mathf.Cos(ang * Mathf.Deg2Rad), Mathf.Sin(ang * Mathf.Deg2Rad));
        while (elapsedTime < waitseconds)
        {
            velImpulse = ((100-knockbackResistance)/100) * recoil * (dir * (1 - elapsedTime / waitseconds));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        velImpulse = Vector2.zero;
    }
    //funcion para crear una hitbox que ataque
    public void hitboxEnemigo(Vector2 position,Vector2 scale,float angle,Vector2 dir,float distance,int[] damage,float knockback,float attackSpeed)
    {
        RaycastHit2D[] boxCast = Physics2D.BoxCastAll(position,scale, angle, dir, distance);
        foreach (RaycastHit2D collider in boxCast)
        {
            if (collider.collider.CompareTag("player") && allowAttack)
            {
                hitPlayer(Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x), damage, knockback, attackSpeed);
            }
        }
    }
    //funciones que se llaman para atacar al player
    public void hitPlayer(float ang,int[] damage,float knockback,float attackSpeed)
    {
        allowAttack = false;
        StartCoroutine(esperar(attackSpeed));
        int dm = Random.Range(damage[0], damage[1] + 1);
        player.takeDamage(dm,ang,knockback,gameObject);
    }
    IEnumerator esperar(float waitseconds)
    {
        yield return new WaitForSeconds(waitseconds);
        allowAttack = true;
    }
    // funciones para el manejo de vida
    public virtual void toDie()
    {
        Destroy(this.gameObject);
        if(room != null){
            room.CommunicateEnemyDeath();
        }
    }
    public void takeDamage(int damage,float ang,float attackKnockback)
    {
        health -= damage;
        GetComponent<genParticulaTexto>().comenzar(damage, ang);
        StartCoroutine(golpeado(attackSpeed, ang, attackKnockback));
        invulnerable = true;
    }
}
