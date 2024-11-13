using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour
{
    //TODO arreglo corrutina attackaLLOW
    public Player player;
    protected Rigidbody2D rb;
    protected Vector2 velImpulse = Vector2.zero;
    public bool allowAttack = true; // booleano que te deja atacar
    public float health = 15;
    public float knockback;
    public float knockbackResistance; // 0 sin resistencia 100 con resistencia
    public int[] damage = new int[2];
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
        if(health <= 0)
        {
            ToDie();
        }
    }
    //funcion impulso
    public IEnumerator Impulse(float waitseconds, float ang, float recoil)
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
    public void HitboxEnemy(Vector2 position,Vector2 scale,float angle,Vector2 dir,float distance,int[] damage,float knockback)
    {
        RaycastHit2D[] boxCast = Physics2D.BoxCastAll(position,scale, angle, dir, distance);
        foreach (RaycastHit2D collider in boxCast)
        {
            if (collider.collider.CompareTag("player") && allowAttack)
            {
                HitPlayer(Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x), damage, knockback);
            }
            if (collider.collider.CompareTag("golpeable"))
            {
                int dam = Random.Range(damage[0], damage[1] + 1);
                Golpeable golpeable = collider.collider.GetComponent<Golpeable>();
                golpeable.TakeDamage(dam);
            }
        }
    }
    //funciones que se llaman para atacar al player
    public void HitPlayer(float ang,int[] damage,float knockback)
    {
        allowAttack = false;

        int dm = Random.Range(damage[0], damage[1] + 1);
        player.TakeDamage(dm,ang,knockback,gameObject);
    }
    // funciones para el manejo de vida
    public virtual void ToDie()
    {
        Destroy(this.gameObject); // morir

        if(room != null){
            room.CommunicateEnemyDeath();
        }
    }

	public void ReduceHealth(int damage) {
		health -= damage;
	}
    public void TakeDamage(int damage,float ang,float attackKnockback)
    {
        ReduceHealth(damage);
        StartCoroutine(Impulse(player.sword.attackAnimation, ang, attackKnockback));
        GetComponent<GenParticulaTexto>().comenzar(damage, ang);
    }
    // Manejo sprites
    public void DoFlipX(SpriteRenderer sR,Vector2 dir) // girar sprite depende de donde mire
    {
        float ang = Mathf.Rad2Deg * (Mathf.Atan2(dir.y, dir.x));
        sR.flipX = (ang > 90 || ang < -90);
    }
}
