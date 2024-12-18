using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    // VIDA
    public float healthMax = 50;
    public float health = 50;
    // DEFENSA
    public float defensa = 1; //Fluctua de 1 a 2. para el jugador mejor mostrarle que el % como tal creo 
    public float multiplicadorDefensa = 1; //Si esto llega a 2 no recibe daño
    // PARRY
    public float resistanceMax = 50;
    public float resistance = 50;
    [SerializeField] bool resistanceRegen = true; // TRUE: regenera resistencia
    public bool parrying = false;
    public float parryTimerCooldown = 3;
    public float parryTimerCheck = 0.5f; // Cuanto tiempo es valido el parry 
    public float parryTimerClamp = 1; // Para que el parry no pueda espamearse poner esto mayor a parryTimerCheck
    [SerializeField] float parryTime = 0;
    Coroutine IcooldownRes;
    //arma
    public bool attacking = false; // cuando estoy atacando
    public bool attackingAnimation = false;
    public bool blocking = false;
    //Stats
    public Sword sword;
    public float vel = 5f;
    public float velWalk = 5f; //velocidad jugador
    public float velBlock = 2.5f;
    public float statCriticalChance;
    public float statCriticalDamage = 2;
    public float ang; // angulo en grados respecto al cursor (0-180,-0-180)
    Vector2 velImpulse;
    Vector2 velMovimiento;
    Vector3 mousePos;
    Rigidbody2D rb;
    SpriteRenderer sR;
    Animator ani;

    // Monedas
    public int monedas = 0;

    //Estados
    public bool stuned = false;
    public float stunedTime;
    public bool invulnerable = false;
    float invulnerableTime;
    // Array de funciones de mecánicas 
    public List<Action<Enemy[]>> attackMechanics = new List<Action<Enemy[]>>(); // Ocurren cada ataque (que golpee o no a enemigos, eso lo revisa la función)
    public List<Action<Enemy[]>> criticalAttackMechanics = new List<Action<Enemy[]>>(); // Ocurren cada ataque crítico
    public List<Action<Enemy>> parryMechanics = new List<Action<Enemy>>();
    public List<Action<Enemy>> perfectParryMechanics = new List<Action<Enemy>>();
    public List<Action<Enemy>> takeHealthDamageMechanics = new List<Action<Enemy>>();
    //Array de funciones de items
    public List<Action> stunMechanics = new List<Action>();
    public List<Action> defenseTempAddMechanics = new List<Action>();

    public List<Action> enterRoomMechanics = new List<Action>(); // Funciones que se llaman al entrar a una habitacion
	// Autoataque
	public bool autoataque;
	public int enrango;

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

        // Update efect status timers
        if (invulnerableTime > 0)
        {
            invulnerableTime -= Time.deltaTime;
            invulnerable = true;
        }
        else if (invulnerableTime <= 0)
        {
            invulnerableTime = 0;
            invulnerable = false;
        }

        if (stunedTime > 0)
        {
            stunedTime -= Time.deltaTime;
            stuned = true;
        }
        else if (stunedTime <= 0)
        {
            stunedTime = 0;
            stuned = false;
        }
    }

	private void Attack() {

		StartCoroutine(Attack(sword.attackSpeed));
		StartCoroutine(AttackAnimation(sword.attackAnimation));
        
	}

	private void AutoAttack() {
		if(autoataque && !blocking && enrango > 0 && !attacking && Time.timeScale > 0 && !stuned) {
			Attack();
		}
	}

    private void Update()
    {
        //movimiento
        Moving();
        vel = ((blocking || stuned) ? velBlock : velWalk); // mirar si esta moviendose


        //vida
        health = Mathf.Clamp(health, 0, healthMax); // limita la vida a maxHealth
        if (health <= 0)
        {
            ToDie();
        }


        //resistencia y parry
        resistance = Mathf.Clamp(resistance, 0, resistanceMax); // limita la resis a maxRes
        parryTime = Mathf.Clamp(parryTime, 0, parryTimerClamp); // se queda en 1 bajar para hacer mas facil hacer parry
        //esta haciendo parry
        parrying = (parryTime > 0 && parryTime < parryTimerCheck && blocking); // calcula cuando esta haciendo parry
        if (resistanceRegen && !blocking && resistance < resistanceMax)
        {
            resistance += 5 * Time.deltaTime;
        }
		AutoAttack();
        //atacar
		if (Input.GetMouseButtonDown(0) && !attacking && Time.timeScale > 0 && !stuned) {
        	Attack();
		}
        //block
        if (Input.GetMouseButton(1) && !attackingAnimation && resistance != 0 && Time.timeScale > 0 && !stuned) // cuando bloqueas tenemos en cuenta la animacion de ataque no el attack speed
        {
            Block();
            parryTime += Time.deltaTime; // a�ade tiempo al parry time para que no puedas spamear el boton de atacar
        }
        else if (parryTime > 0)
        {
            blocking = false;
            parryTime -= Time.deltaTime;
        }
        //direccion en la que esta mirando el personaje
        ang = CalcularAngulo();
        if (Time.timeScale > 0) sR.flipX = (ang > 90 || ang < -90); // Que no cambie el sentido del jugador basado en a donde apuntas mientras el juego esta en pausa

        //animacion
        if (attackingAnimation)
        {
            //ani.SetFloat("velocity", 0);
        }
        else
        {
            ani.SetFloat("velocity", velMovimiento.magnitude);
        }
        //Comprobar en cada frame la lista de defenseTempAddMechanics 
        foreach (Action mechanic in defenseTempAddMechanics)
        {
            mechanic.Invoke();
        }

    }
    //Funcion que calcula en angulo respecto al cursor
    public float CalcularAngulo()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float adyacente = mousePos.x - transform.position.x;
        float opuesto = mousePos.y - transform.position.y;
        return Mathf.Rad2Deg * (Mathf.Atan2(opuesto, adyacente));
    }
    //Funcion que devuelve el vector2 del mocvimiento del jugador
    public void Moving()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector2 dir = new Vector2(x, y);
        if (dir.magnitude > 1)
        {
            dir.Normalize();
        }
        velMovimiento = !stuned ? dir * vel : new(0, 0);
    }
    //Funcion atacar
    IEnumerator Attack(float waitseconds) // funcion ataque
    {
        attacking = true;
        sword.Attack();
        Enemy[][] hitData = sword.HitboxPlayer(transform.position, sword.hitboxSize, ang, (mousePos - transform.position).normalized, 1f);
        StartCoroutine(Impulse(sword.attackAnimation, ang, sword.recoil)); // calcular el impulso(mejor con el tiempo de la animacion)
        // Llamar a mecánicas de items de ataque
        foreach (Action<Enemy[]> mechanic in attackMechanics)
        {
            mechanic.Invoke(hitData[0]);
        }
        // Mecanicas de ataques críticos
        foreach (Action<Enemy[]> mechanic in criticalAttackMechanics)
        {
            mechanic.Invoke(hitData[1]);
        }
        yield return new WaitForSeconds(waitseconds);
        attacking = false;
    }
    IEnumerator AttackAnimation(float waitseconds)
    {
        attackingAnimation = true;
        yield return new WaitForSeconds(waitseconds);
        attackingAnimation = false;
    }
    //Funcion block
    public void Block()
    {
        sword.Block();
        blocking = true;
    }
    //Funcion que a�ade un impulso en una direccion
    public IEnumerator Impulse(float waitseconds, float ang, float recoil)
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
    // Funcion que sirve para recibir da�o
    public void TakeDamage(float damage, float ang, float knockback, GameObject gObject = null)
    {
        if (invulnerable) //Si es invulnerable no recibe daño
        {
            return;
        }

        if (resistance > 0 && blocking)
        {
            // Parry perfecto
            if (parrying && gObject != null) // se hace el parry a un Enemy
            {
                // Animaciones
                GetComponent<GenLight>().GenerateLight(sword.transform.position);
                // El resto
                parryTime = 0; // cuando haces parry reseteas el timer
                if (gObject.GetComponent<Enemy>() != null)
                {
                    StartCoroutine(gObject.GetComponent<Enemy>().Impulse(0.15f, (ang + 180), 1)); // aplica empuje en la dir que haces el parry
                    foreach (Action<Enemy> mechanic in perfectParryMechanics) // Ejecutamos mecanicas de parry
                    {
                        mechanic.Invoke(gObject.GetComponent<Enemy>());
                    }
                    foreach (Action<Enemy> mechanic in parryMechanics) // Ejecutamos mecanicas de bloqueo
                    {
                        mechanic.Invoke(gObject.GetComponent<Enemy>());
                    }
                }
                else if (gObject.GetComponent<Projectile>() != null)
                {
                    gObject.GetComponent<Projectile>().ToGetParried();
                }
                attacking = false; // cuando haces parry reseteas que el jugador pueda atacar(punish)

            }
            // Parry normal
            else if (gObject != null)
            {
                resistance -= damage;
                StartCoroutine(Impulse(0.2f, ang, knockback)); // empuje
                // cooldown de parry
                if (IcooldownRes != null)
                {
                    StopCoroutine(IcooldownRes);
                }
                IcooldownRes = StartCoroutine(ParryCooldown(parryTimerCooldown));

                if (gObject.GetComponent<Enemy>() != null)
                {
                    foreach (Action<Enemy> mechanic in parryMechanics) // Ejecutamos mecanicas de bloqueo
                    {
                        mechanic.Invoke(gObject.GetComponent<Enemy>());
                    }
                }
            }

        }
        else
        {   //Comentado todo lo de defensa por si si
            if (gObject.GetComponent<Enemy>() != null)
            {
                foreach (Action<Enemy> mechanic in takeHealthDamageMechanics) // Ejecutamos mecanicas de bloqueo
                {
                    mechanic.Invoke(gObject.GetComponent<Enemy>());
                }
            }
            health -= (damage/defensa); //Preguntar a Alvaro si se tiene dudas de la ecuacion
            StartCoroutine(Impulse(0.2f, ang, knockback)); // empuje
        }

    }
    // Parry y manejo de resistencia
    IEnumerator ParryCooldown(float waitseconds)
    {
        resistanceRegen = false;
        yield return new WaitForSeconds(waitseconds);
        resistanceRegen = true;
    }
    // Funcion que se llama cuando el jugador muere
    private void ToDie()
    {
        SceneManager.LoadScene("SampleScene");
    }

    //Metodo para aplicar stun al jugador durante 1,5 seg
    public void GetStuned(float waitseconds)
    {
        stunedTime = waitseconds;
    }
    //Metodo para invulnerabilidad
    public void GetInvulnerable(float waitseconds)
    {
        invulnerableTime = waitseconds;
    }
    public void OnRoomEnter()
    { // Funcion llamada desde la habitacion al entrar
        foreach (Action mechanic in enterRoomMechanics)
        {
            mechanic.Invoke();
        }
    }
}
