using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.LightAnchor;

public class MiniBoss : Enemy
{
    // Start is called before the first frame update
    
    Animator ani;
    public MiniBossClone miniBossClone;
    public MiniBossSword miniBossSword;
    public MiniBossHitmarker miniBossHitmarker;
    // Patrones de ataque
    public float distanceToPlayer;
    public enum patterns
    {
        idle,
        change,
        follow,
        attack_sword,
        attack_charge,
        attack_shoot_1,
        attack_shoot_2,
        attack_shoot_especial

    }
    public patterns patron = patterns.idle;
    public bool stayOnPattern = false;
    Coroutine TimeToWait = null;
    public bool flipX;
    // Sword
    public float displSpeed = 3.5f;
    public float timer = 0;
    public float timerReset = 1;
    public float countAttacks = 0;
    public Vector2 direction;
    // Luces
    public GameObject luzDisparo;
    public GameObject luzSword;

    // Charge


    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        ani = GetComponent<Animator>();
    }

    new void Update()
    {
        base.Update();
        flipX = DoFlipX(GetComponent<SpriteRenderer>(), (player.transform.position - transform.position).normalized);
    }
    // Update is called once per frame
    new void FixedUpdate()
    {
        base.FixedUpdate();
        distanceToPlayer = Vector2.Distance(transform.position, player.transform.position); // distancia entre player y el boss
        direction = (player.transform.position - transform.position).normalized; // direccion en la que mira el boss
        // Como actua el personaje segun patron
        switch (patron)
        {
            case patterns.idle:
                // No hago nada

                break;
            case patterns.change:
                // Por ahora no hago nada

                break;
            case patterns.follow: // sigue al personaje
                rb.velocity += new Vector2(direction.x * displSpeed * 0.1f, direction.y * displSpeed * 0.1f);
                break;
            case patterns.attack_sword: // saca una espada y ataca
                timer += Time.deltaTime;
                timerReset = 1;
                if(timer > timerReset && distanceToPlayer < 3)
                {
                    Debug.Log("Te ataco!");
                    timer = 0;
                    countAttacks++;
                }
                if(countAttacks > 4)
                {
                    stayOnPattern = false; // Terminamos la fase de la espada
                }
                break;
            case patterns.attack_charge: // carga contra ti saltando en tu direccion
                timer += Time.deltaTime;
                timerReset = 1;
                Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("player"), LayerMask.NameToLayer("miniboss"), true); // para que atraviese al jugador
                EfectoFrames();
                if (timer > timerReset)
                {
                    Debug.Log("Te salto!");
                    GenerateLight(luzDisparo);
                    rb.AddForce(direction * 80f, ForceMode2D.Impulse);
                    timer = 0;
                }
                break;
            case patterns.attack_shoot_1: // dispara rapido
                timer += Time.deltaTime;
                timerReset = 0.5f;
                if (timer > timerReset)
                {
                    GenerateLight(luzDisparo);
                    Debug.Log("Te disparo 1!");
                    float[] tempDam = { 2, 4 };
                    GetComponent<ProjectileGen>().Lanzar_Projectil(miniBossSword.transform.position, direction, tempDam , knockback);
                    timer = 0;
                }
                break;
            case patterns.attack_shoot_2: // dispara largo alcance con mucha velocidad
                timer += Time.deltaTime;
                timerReset = 2f;
                if (timer > timerReset)
                {
                    GenerateLight(luzSword);
                    float[] tempDam = { 4, 12 };
                    GetComponent<ProjectileGen>().Lanzar_Projectil(miniBossSword.transform.position, direction, tempDam, knockback);
                    timer = 0;
                }
                break;
            case patterns.attack_shoot_especial: // dispara en todas las direcciones
                timer += Time.deltaTime;
                timerReset = 2f;
                if (timer > timerReset)
                {
                    GenerateLight(luzSword);
                    float[] tempDam = { 2, 4 };
                    float ang = 0;
                    for (int i = 0; i < 16; i++)
                    {
                        Vector2 dir = new(Mathf.Cos(ang * Mathf.Deg2Rad), Mathf.Sin(ang * Mathf.Deg2Rad));
                        GetComponent<ProjectileGen>().Lanzar_Projectil(miniBossSword.transform.position, dir, tempDam, knockback);
                        ang += 22.5f;
                    }
                    timer = 0;
                }
                break;
        }


        // Que patron escoger para el siguiente
        if (stayOnPattern)
        {
            return;
        }
        else if(distanceToPlayer < 4)
        {
            int rand = Random.Range(0, 2);
            if (rand == 0) { patron = patterns.attack_sword; stayOnPattern = true; }
            else { StartCoroutine(ChangePattern(patterns.attack_charge,6)); }
        }
        else if(distanceToPlayer < 8)
        {
            int rand = Random.Range(0, 4);
            if (rand == 0) { StartCoroutine(ChangePattern(patterns.attack_shoot_1, 4)); }
            else if(rand == 1) { StartCoroutine(ChangePattern(patterns.attack_shoot_especial, 7)); ; }
            else if (rand == 2) {StartCoroutine(ChangePattern(patterns.attack_charge, 6)); }
            else { patron = patterns.attack_sword; stayOnPattern = true; }
        }
        else if(distanceToPlayer < 15)
        {
            int rand = Random.Range(0, 4);
            if (rand == 0) { StartCoroutine(ChangePattern(patterns.attack_shoot_1, 6)); }
            else if (rand == 1) { StartCoroutine(ChangePattern(patterns.follow, 5)); }
            else if (rand == 2) { StartCoroutine(ChangePattern(patterns.attack_shoot_especial, 7)); }
            else { StartCoroutine(ChangePattern(patterns.attack_shoot_2, 6)); }
        }
        else
        {
            StartCoroutine(ChangePattern(patterns.idle, 2));
        }
    }
    IEnumerator Esperar(float waitseconds)
    {
        yield return new WaitForSeconds(waitseconds);
        stayOnPattern = false;
    }
    IEnumerator ChangePattern(patterns patterns,float waitseconds)
    {
        stayOnPattern = true;
        patron = patterns.change;
        yield return new WaitForSeconds(2);

        //timer y contador de ataques
        timer = 0;
        countAttacks = 0;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("player"), LayerMask.NameToLayer("miniboss"), false);

        // cambiar al patron de la llamada a funcion despues de haber esperado
        patron = patterns;
        if(TimeToWait != null)
        {
            StopCoroutine(TimeToWait);
        }
        TimeToWait = StartCoroutine(Esperar(waitseconds));
    }
    void EfectoFrames()
    {
        Instantiate(miniBossClone,transform.position,Quaternion.identity); // Consume muchos recursos pero xd!!
    }
    void GenerateLight(GameObject luz)
    {
        GetComponent<GenLight>().luz = luz; // Cambia la luz a la actual
        GetComponent<GenLight>().GenerateLight(miniBossSword.transform.position);
    }
    IEnumerator ToSwordAttack(float waitseconds)
    {
        GenerateLight(luzSword);
        yield return new WaitForSeconds(waitseconds);
        
    }
}
