using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBoss : Enemy
{
    // Start is called before the first frame update
    
    Animator ani;
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


    // Shoot


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
        distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        // Como actua el personaje segun patron
        switch (patron)
        {
            case patterns.idle:
                

                break;
            case patterns.change:
               
                break;
            case patterns.follow:
                
                break;
            case patterns.attack_sword:
                
                break;
            case patterns.attack_charge:
                
                break;
            case patterns.attack_shoot_1:
                
                break;
            case patterns.attack_shoot_2:
                
                break;
            case patterns.attack_shoot_especial:
                
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
            else if(rand == 1) { StartCoroutine(ChangePattern(patterns.attack_shoot_especial, 3)); ; }
            else if (rand == 2) {StartCoroutine(ChangePattern(patterns.follow, 3)); }
            else {StartCoroutine(ChangePattern(patterns.attack_charge, 6)); }
        }
        else if(distanceToPlayer < 12)
        {
            int rand = Random.Range(0, 3);
            if (rand == 0) { StartCoroutine(ChangePattern(patterns.attack_shoot_1, 6)); }
            else if (rand == 1) { StartCoroutine(ChangePattern(patterns.follow, 5)); }
            else { StartCoroutine(ChangePattern(patterns.attack_shoot_2, 4)); }
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
        patron = patterns;
        if(TimeToWait != null)
        {
            StopCoroutine(TimeToWait);
        }
        TimeToWait = StartCoroutine(Esperar(waitseconds));
    }
}
