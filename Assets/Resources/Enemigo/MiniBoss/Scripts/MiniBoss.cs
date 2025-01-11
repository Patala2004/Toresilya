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
    public float maxHealth;
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
    bool cambioSword = false;
    int lastAttack = 6;
    public bool strongAttack = false;
    // Luces
    public GameObject luzDisparo;
    public GameObject luzSword;
    public GameObject luzSwordMaximo;
    // Charge
    bool saltado = false;
    float[] tempDamage = new float[2];
    // Shoot
    int ShootMovement = 0;
    bool MoverDer = true;
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        ani = GetComponent<Animator>();
        health = maxHealth;
        // carga
        tempDamage[0] = damage[0] / 2f;
        tempDamage[1] = damage[1] / 2f;
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
                rb.velocity += new Vector2(direction.x * displSpeed * 0.1f, direction.y * displSpeed * 0.1f);
                strongAttack = countAttacks == lastAttack;
                if(timer > timerReset && distanceToPlayer < 3 && countAttacks <= 6)
                {
                    timerReset = strongAttack ? 2.5f : 0.7f;
                    StartCoroutine(ToSwordAttack(0.3f));
                    timer = 0;
                    countAttacks++;
                }
                if(countAttacks > 6 && !cambioSword)
                {
                    StartCoroutine(WaitToChangePattern(0.6f)); // Terminamos la fase de la espada
                }
                break;
            case patterns.attack_charge: // carga contra ti saltando en tu direccion
                timer += Time.deltaTime;
                timerReset = 1;
                // Visuales
                Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("player"), LayerMask.NameToLayer("miniboss"), true); // para que atraviese al jugador
                EfectoFrames();
                // Ataque
                HitboxEnemy(transform.position, new(1.3f, 1.3f), 0, direction, 0, tempDamage, 15);
                if (timer > timerReset)
                {
                    Debug.Log("Te salto!");
                    allowAttack = true;
                    GenerateLight(luzDisparo);
                    rb.AddForce(direction * 80f, ForceMode2D.Impulse);
                    timer = 0;
                }
                break;
            case patterns.attack_shoot_1: // dispara rapido
                timer += Time.deltaTime;
                timerReset = 0.5f;
                if(ShootMovement % 6 == 0)
                {
                    MoverDer = !MoverDer;
                }
                if (MoverDer)
                {
                    rb.velocity += new Vector2(direction.y * displSpeed * 0.1f, -direction.x * displSpeed * 0.1f);
                }
                else
                {
                    rb.velocity += new Vector2(-direction.y * displSpeed * 0.1f, direction.x * displSpeed * 0.1f);
                }
                if (timer > timerReset)
                {
                    GenerateLight(luzDisparo);
                    Debug.Log("Te disparo 1!");
                    float[] tempDam = { 2, 4 };
                    GetComponent<ProjectileGen>().Lanzar_Projectil(miniBossSword.transform.position, direction, tempDam , knockback);
                    timer = 0;
                    ShootMovement += 1;
                }
                break;
            case patterns.attack_shoot_2: // dispara largo alcance con mucha velocidad
                timer += Time.deltaTime;
                timerReset = 2f;
                if (ShootMovement % 6 == 0)
                {
                    MoverDer = !MoverDer;
                }
                if (MoverDer)
                {
                    rb.velocity += new Vector2(direction.y * displSpeed * 0.1f, -direction.x * displSpeed * 0.1f);
                }
                else
                {
                    rb.velocity += new Vector2(-direction.y * displSpeed * 0.1f, direction.x * displSpeed * 0.1f);
                }
                if (timer > timerReset)
                {
                    GenerateLight(luzSword);
                    float[] tempDam = { 4, 12 };
                    GetComponent<ProjectileGen>().Lanzar_Projectil(miniBossSword.transform.position, direction, tempDam, knockback);
                    timer = 0;
                    ShootMovement += 1;
                }
                break;
            case patterns.attack_shoot_especial: // dispara en todas las direcciones
                timer += Time.deltaTime;
                timerReset = 2f;
                Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("player"), LayerMask.NameToLayer("miniboss"), true); // para que atraviese al jugador
                EfectoFrames();
                if (timer > 1 && !saltado)
                {
                    float ramdomDir;
                    if (Random.Range(0,2) == 0)
                    {
                        ramdomDir = Random.Range(-0.25f,-0.75f);
                    }
                    else
                    {
                        ramdomDir = Random.Range(0.25f, 0.75f);
                    }
                    rb.AddForce(new Vector2(direction.x + ramdomDir,direction.y + ramdomDir).normalized * 60f, ForceMode2D.Impulse);
                    saltado = true;
                }
                if (timer > timerReset)
                {
                    // visuales
                    camara.shakeAllCameras(0.1f);
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
                    saltado = false;
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
            if (rand == 0) { patron = patterns.attack_sword; stayOnPattern = true; timerReset = 3; cambioSword = false; }
            else { StartCoroutine(ChangePattern(patterns.attack_charge,6)); }
        }
        else if(distanceToPlayer < 10)
        {
            int rand = Random.Range(0, 4);
            if (rand == 0) { StartCoroutine(ChangePattern(patterns.attack_shoot_1, 4)); }
            else if(rand == 1) { StartCoroutine(ChangePattern(patterns.attack_shoot_especial, 7)); ; }
            else if (rand == 2) {StartCoroutine(ChangePattern(patterns.attack_charge, 6)); }
            else { patron = patterns.attack_sword; stayOnPattern = true; timerReset = 3; cambioSword = false; }
        }
        else if(distanceToPlayer < 15)
        {
            int rand = Random.Range(0, 4);
            if (rand == 0) { StartCoroutine(ChangePattern(patterns.attack_shoot_1, 6)); }
            else if (rand == 1) { StartCoroutine(ChangePattern(patterns.follow, 2)); }
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
        patron = patterns.change; // cambiamos de patron
        yield return new WaitForSeconds(2);

        //timer y contador de ataques
        ShootMovement = 0; // Shoot
        allowAttack = false; // para casi todas las fases
        timer = 0; // para casi todas las fases
        countAttacks = 0; // sword
        saltado = false; // attack shoot especial
        cambioSword = false; // sword
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("player"), LayerMask.NameToLayer("miniboss"), false); // charge

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
        if (strongAttack)
        {
            GenerateLight(luzSwordMaximo);
        }
        else{ GenerateLight(luzSword); }
        yield return new WaitForSeconds(waitseconds);
        allowAttack = true;
        if (strongAttack) { 
            HitboxEnemy(transform.position, new(2f, 2f), Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x), direction, 1.5f, damage, knockback);
            camara.shakeAllCameras(1);
        }
        else { 
            HitboxEnemy(transform.position, new(2f, 2f), Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x), direction, 1.5f, tempDamage, 15);
            camara.shakeAllCameras(0.1f);
        }
        miniBossHitmarker.Comenzar();
        miniBossSword.Comenzar();
    }
    IEnumerator WaitToChangePattern(float waitseconds) // solo para sword
    {
        cambioSword = true;
        yield return new WaitForSeconds(waitseconds);
        stayOnPattern = false;
    }
}
