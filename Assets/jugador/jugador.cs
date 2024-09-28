using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class jugador : MonoBehaviour
{
    public animacion animacion;
    public arma arma;
    Rigidbody2D rb;
    public float vel = 5f;
    public float recoil = 300f;
    public float ang;
    public bool attacking = false;
    Vector2 vel2;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.velocity = moverse() + vel2;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !attacking)
        {
            StartCoroutine(atacar(0.2f));
        }
        ang = angulo();
    }
    public float angulo()
    {
        Vector2 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float adyacente = MousePos.x - transform.position.x;
        float opuesto = MousePos.y - transform.position.y ;
        return Mathf.Rad2Deg*(Mathf.Atan2(opuesto,adyacente));
    }
    public Vector2 moverse()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector2 dir = new Vector2(x, y);
        if (dir.magnitude > 1)
        {
            dir.Normalize();
        }
        return (dir * vel);
    }
    IEnumerator atacar(float waitseconds)
    {
        float elapsedTime = 0;
        attacking = true;
        animacion.comenzar();
        Vector2 dir = new Vector3(Mathf.Cos(ang * Mathf.Deg2Rad), Mathf.Sin(ang * Mathf.Deg2Rad));
        while (elapsedTime < waitseconds)
        {
            vel2 = recoil * (dir * (1 - elapsedTime / waitseconds));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        vel2 = Vector2.zero;
        attacking = false;
    }
}
