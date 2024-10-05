using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class arma : MonoBehaviour
{
    public jugador jugador;
    Animator ani;
    SpriteRenderer sR;
    public float recoil = 10f; // recoil del arma(empuje cuando)
    public float attackKnockback = 4f;// lo lejos que envia al enemigo atacando
    public float attackSpeed = 0.2f; // velocidad a la que el jugador puede atacar(no es la velocidad de la animacion)
    public float attackAnimation = 0.2f; // cuanto dura la animacion *importante que no sea mucho
    //to do manejar rng
    public float[] attackDamage = new float[2]; // daño de dañomin a dañomax
    float radioArma = 0.3f;
    float rotacionArma = 90;
    // Start is called before the first frame update
    void Start()
    {
        ani = GetComponent<Animator>();
        sR = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

        //seguimiento del arma al jugador ç
        if (jugador.ang > 90 || jugador.ang < -90)
        {
            sR.flipY = true;
            float x = Mathf.Cos((jugador.ang - rotacionArma) * Mathf.Deg2Rad) * radioArma;
            float y = Mathf.Sin((jugador.ang - rotacionArma) * Mathf.Deg2Rad) * radioArma;
            transform.localPosition = new Vector3(x, y, 1);
            transform.localEulerAngles = new Vector3(0, 0, jugador.ang + 90);
        }
        else
        {
            sR.flipY = false;
            float x = Mathf.Cos((jugador.ang + rotacionArma) * Mathf.Deg2Rad) * radioArma;
            float y = Mathf.Sin((jugador.ang + rotacionArma) * Mathf.Deg2Rad) * radioArma;
            transform.localPosition = new Vector3(x, y, 1);
            transform.localEulerAngles = new Vector3(0, 0, jugador.ang - 90);
        }
        //bloqueo
        if (jugador.blocking)
        {
            radioArma = 0.9f;
            rotacionArma = -50;
        }
        else
        {
            radioArma = 0.3f;
            rotacionArma = 0;
        }
        //anim attack
        ani.SetBool("atacando", jugador.attacking);
    }
    public void attack()
    {
        
    }
    public void block()
    {
        
    }
}
