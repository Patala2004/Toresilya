using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Player : MonoBehaviour
{
    //TODO: vida
    public float healthMax = 50;
    public float health = 50;
    //TODO: implementar parry (probar con slime con impulso 20000)
    public float resistanceMax = 50;
    public float resistance = 50;
    [SerializeField] bool resistanceRegen = true; // TRUE: regenera resistencia
    public bool parrying = false;
    public float parryTimerCooldown = 3;
    public float parryTimerCheck = 0.5f; // Cuanto tiempo es valido el parry 
    public float parryTimerClamp = 1; // Para que el parry no pueda espamearse poner esto mayor a parryTimerCheck
    [SerializeField] float parryTime = 0;
    Coroutine coroutineCooldownRes;
 
    public Sword sword;
    public float vel = 5f; //velocidad jugador
    public float ang; // angulo en grados respecto al cursor (0-180,-0-180)
    public bool attacking = false; // cuando estoy atacando
    public bool attackingAnimation = false;
    public bool blocking = false;
    Vector2 velImpulse;
    Vector2 velMovimiento;
    Vector3 mousePos;
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
        rb.velocity = velMovimiento + velImpulse; // la velocidad del movimiento mas la del recoil del sword
    }
    private void Update()
    {
        //movimiento
        moving();
        //vida
        health = Mathf.Clamp(health,0,healthMax); // limita la vida a maxHealth
        //resistencia y parry
        resistance = Mathf.Clamp(resistance, 0, resistanceMax); // limita la vida a maxHealth
        parryTime = Mathf.Clamp(parryTime, 0, parryTimerClamp); // se queda en 1 bajar para hacer mas facil hacer parry
        //esta haciendo parry
        parrying = (parryTime > 0 && parryTime < parryTimerCheck && blocking); // calcula cuando esta haciendo parry
        if (resistanceRegen && !blocking)
        {
            resistance += 5 * Time.deltaTime;
        }
        //atacar
        if (Input.GetMouseButtonDown(0) && !attacking)
        {
            StartCoroutine(attack(sword.attackSpeed));
            StartCoroutine(attackAnimation(sword.attackAnimation));
        }
        //block
        if (Input.GetMouseButton(1) && !attackingAnimation && resistance != 0)
        {
            block();
            parryTime += Time.deltaTime; // añade tiempo al parry time para que no puedas spamear el boton de atacar
        }
        else
        {
            blocking = false;
            parryTime -= Time.deltaTime;
        }
        //direccion en la que esta mirando el personaje
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        ang = calcularAngulo();
        sR.flipX = (ang > 90 || ang < -90);
        //animacion
        if (attackingAnimation)
        {
            //ani.SetFloat("velocity", 0);
        }
        else { 
            ani.SetFloat("velocity", velMovimiento.magnitude); 
        }
    }
    //Funcion que calcula en angulo respecto al cursor
    public float calcularAngulo()
    {
        float adyacente = mousePos.x - transform.position.x;
        float opuesto = mousePos.y - transform.position.y ;
        return Mathf.Rad2Deg*(Mathf.Atan2(opuesto,adyacente));
    }
    //Funcion que devuelve el vector2 del mocvimiento del jugador
    public void moving()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector2 dir = new Vector2(x, y);
        if (dir.magnitude > 1)
        {
            dir.Normalize();
        }
        velMovimiento = dir * vel;
    }
    //Funcion atacar
    IEnumerator attack(float waitseconds) // funcion ataque
    {
        attacking = true;
        sword.attack();
        hitboxsword(transform.position, sword.hitboxSize, 0, mousePos-transform.position , 1, sword.attackDamage, sword.attackKnockback, sword.attackAnimation);
        StartCoroutine(impulse(sword.attackAnimation, ang, sword.recoil)); // calcular el impulso(mejor con el tiempo de la animacion)
        yield return new WaitForSeconds(waitseconds);
        attacking = false;
    }
    IEnumerator attackAnimation(float waitseconds)
    {
        attackingAnimation = true;
        yield return new WaitForSeconds(waitseconds);
        attackingAnimation = false;
    }
    public void hitboxsword(Vector2 position, Vector2 scale, float angle, Vector2 dir, float distance, int[] damage, float knockback, float attackSpeed)
    {
        RaycastHit2D[] boxCast = Physics2D.BoxCastAll(position, scale, angle, dir, distance);
        foreach (RaycastHit2D collider in boxCast)
        {
            if (collider.collider.CompareTag("enemigo"))
            {
                enemigo enemigo = collider.collider.GetComponent<enemigo>();
                if (!enemigo.invulnerable)
                {
                    int dam = Random.Range(sword.attackDamage[0], sword.attackDamage[1] + 1);
                    enemigo.takeDamage(dam,ang,sword.attackKnockback);
                }
            }
        }
    }
    //Funcion block
    public void block()
    {
        sword.block();
        blocking = true;
    }
    //Funcion que añade un impulso en una direccion
    public IEnumerator impulse(float waitseconds,float ang,float recoil) 
    {
        float elapsedTime = 0;
        Vector2 dir = new Vector3(Mathf.Cos(ang * Mathf.Deg2Rad), Mathf.Sin(ang * Mathf.Deg2Rad));
        while (elapsedTime < waitseconds)
        {
            velImpulse = (!parrying ? 1 : 0) * recoil * (dir * (1 - elapsedTime / waitseconds));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        velImpulse = Vector2.zero;
    }
    // Funcion que sirve para recibir daño
    public void takeDamage(int damage,float ang,float knockback,GameObject gObject = null)
    {
        if(resistance > 0 && blocking)
        {
            if (parrying && gObject != null) // se hace el parry a un enemigo
            {
                parryTime = 0; // cuando haces parry reseteas el timer
                StartCoroutine(gObject.GetComponent<enemigo>().impulse(0.1f, ang, 1)); // aplica empuje en la dir que haces el parryçç+
                attacking = false; // cuando haces parry reseteas que el jugador pueda atacar(punish)
            }
            else
            {
                resistance -= damage;
                StartCoroutine(impulse(0.2f, ang, knockback)); // empuje
                // cooldown de parry
                if (coroutineCooldownRes != null)
                {
                    StopCoroutine(coroutineCooldownRes);
                }
                coroutineCooldownRes = StartCoroutine(parryCooldown(parryTimerCooldown));
            }
        }
        else
        {
            health -= damage;
            StartCoroutine(impulse(0.2f, ang, knockback)); // empuje
        }
    }
    // Parry y manejo de resistencia
    IEnumerator parryCooldown(float waitseconds)
    {
        resistanceRegen = false;
        yield return new WaitForSeconds(waitseconds);
        resistanceRegen = true;
    }
}
