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
    public float[] damage = new float[2];
    public Room room;
    
    //Defensa enemigo funciona como player
    public float defensa = 1; //Fluctua de 1 a 2. para el jugador mejor mostrarle que el % como tal creo 
    public float multiplicadorDefensa = 1; //Si esto llega a 2 no recibe da�o, si es menor que 1 recibe mas dano

    //Cosas que se han anadido por los items
    public float dmgMultiplicator = 1f;
    
    AStar astar;

    //Lista de metodos temporales segun estados


    // Efectos de estado
        // Paralizado -> puede atacar pero no moverse
    public float paralizedTime = 0f;
    public bool isParalized = false;

        // Caos -> no puede ni moverse ni atacar
    public float caosTime = 0f;
    public bool isCaos = false;

        //Debil -> Su defensa disminue un 20% durante durDebil en item.cs
    public float debilTime = 0f;
    public bool isDebil = false;
    // Animaciones
    Coroutine coroutineIndicatorColor;

    // Start is called before the first frame update
    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("player").GetComponent<Player>();
    }

	public void EnableAStar() {
		astar = new AStar(room.nodeMatrix, player);
		Debug.Log("Enabled a star");
		if(room.nodeMatrix == null) {
			Debug.LogError("Matriz de nodos nula");
		}
		if(player == null) {
			Debug.LogError("Jugador");
		}
	}

    public void FixedUpdate()
    {
        rb.velocity += velImpulse;
        if(isParalized || isCaos) rb.velocity = new Vector2(0f,0f);

        // Update efect status timers
        if(paralizedTime > 0){
            paralizedTime -= Time.deltaTime;
        }
        else if(paralizedTime <= 0){
            paralizedTime = 0;
            isParalized = false;
        }

        if(caosTime > 0){
            caosTime -= Time.deltaTime;
        }
        else if(caosTime <= 0){
            caosTime = 0;
            isCaos = false;
        }

        if (debilTime > 0)
        {
            debilTime -= Time.deltaTime;
        }
        else if (debilTime <= 0)
        {
            debilTime = 0;
            isDebil = false;
        }
        
      

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
    public void HitboxEnemy(Vector2 position,Vector2 scale,float angle,Vector2 dir,float distance,float[] damage,float knockback)
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
                float dam = Random.Range(damage[0], damage[1]);
                Golpeable golpeable = collider.collider.GetComponent<Golpeable>();
                golpeable.TakeDamage(dam);
            }
        }
    }
    //funciones que se llaman para atacar al player
    public void HitPlayer(float ang,float[] damage,float knockback)
    {

        if(isCaos) return; // El efecto de estado de caos no deja atacar

        allowAttack = false;

        float dm = (Random.Range(damage[0], damage[1])) *dmgMultiplicator;
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

	public void ReduceHealth(float damage) {
		health -= damage;
	}
    public void TakeDamage(float damage,float ang,float attackKnockback, Color color = default)
    {
        float tempDamage = damage / (defensa * (isDebil? Item.debilDefReductionMult : 1)) ;
        ReduceHealth(tempDamage);
        StartCoroutine(Impulse(player.sword.attackAnimation, ang, attackKnockback));
        //animaciones
        if(coroutineIndicatorColor != null)
        {
            StopCoroutine(coroutineIndicatorColor);
        }
        coroutineIndicatorColor = StartCoroutine(IColorDamage(0.4f,color == default ? Color.red:color));
        GetComponent<GenParticulaTexto>().comenzar(tempDamage, ang, color);
    }
    // Manejo sprites
    public void DoFlipX(SpriteRenderer sR,Vector2 dir) // girar sprite depende de donde mire
    {
        float ang = Mathf.Rad2Deg * (Mathf.Atan2(dir.y, dir.x));
        sR.flipX = (ang > 90 || ang < -90);
    }

    IEnumerator IColorDamage(float waitseconds,Color color)
    {
        SpriteRenderer sR = GetComponent<SpriteRenderer>();
        float elapsedTime = 0;
        sR.color = color;
        while(elapsedTime < waitseconds)
        {
            sR.color = new Color(color.r + (1 - color.r) * (elapsedTime/waitseconds), color.g + (1 - color.g) * (elapsedTime / waitseconds), color.b + (1 - color.b) * (elapsedTime / waitseconds), 1);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        sR.color = Color.white;
    }


	public List<Node> pathToPlayer() {
		Node origin = astar.getNearestNode(this.gameObject);
		Node end = astar.getNearestNode(player.gameObject);
		return astar.FindPath(origin, end);
	}


    // Funciones para efectos de estado
    public void Paralize(float duration){
        isParalized = true;
        if(duration > paralizedTime){ // Si ya esta paralizado solo queremos aumentar a la nueva duracion si es mayor a la restante
            paralizedTime = duration;
        }
    }
    public void Caos(float duration){
        isCaos = true;
        if(duration > caosTime){ // Si ya esta paralizado solo queremos aumentar a la nueva duracion si es mayor a la restante
            caosTime = duration;
        }
    }
    public void Debil(float duration)
    {
        isDebil = true;
        if (duration > debilTime)
        { // Si ya esta paralizado solo queremos aumentar a la nueva duracion si es mayor a la restante
            debilTime = duration;
        }
    }

}
