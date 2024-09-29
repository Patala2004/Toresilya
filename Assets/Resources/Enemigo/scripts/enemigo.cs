using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemigo : MonoBehaviour
{
    public jugador jugador;
    Rigidbody2D rb;
    Vector2 vel2 = Vector2.zero;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        jugador = GameObject.Find("player").GetComponent<jugador>();
    }
    private void FixedUpdate()
    {
        rb.velocity = vel2;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("hitboxAmigo"))
        {
            Debug.Log("golpeado!!");
            StartCoroutine(golpeado(jugador.arma.attackSpeed,jugador.ang, jugador.arma.recoil,jugador.arma.attackKnockback));
        }
    }
    IEnumerator golpeado(float waitseconds,float ang,float recoil,float attackKnockback)
    {
        float elapsedTime = 0;
        Vector2 dir = new Vector3(Mathf.Cos(ang * Mathf.Deg2Rad), Mathf.Sin(ang * Mathf.Deg2Rad));
        while (elapsedTime < waitseconds)
        {
            vel2 = attackKnockback * recoil * (dir * (1 - elapsedTime / waitseconds));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        vel2 = Vector2.zero;
    }
}
