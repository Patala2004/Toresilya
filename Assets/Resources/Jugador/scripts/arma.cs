using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class arma : MonoBehaviour
{
    public jugador jugador;
    Animator ani;
    // Start is called before the first frame update
    void Start()
    {
        ani = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
            float x = Mathf.Cos((jugador.ang) * Mathf.Deg2Rad) * 0.3f;
            float z = Mathf.Sin((jugador.ang) * Mathf.Deg2Rad) * 0.3f;
            transform.localPosition = new Vector3(x, z, 0);
            transform.localEulerAngles = new Vector3(0, 0, jugador.ang - 120);
        /* if(jugador.ang > 90 || jugador.ang < -90)
         {
             GetComponent<SpriteRenderer>().flipX = true;
             GetComponent<SpriteRenderer>().flipY = true;
             transform.localPosition = new Vector3(-0.65f, 0.2f, 0);
             transform.eulerAngles = new Vector3(0, 0, jugador.ang + 40);
         }
         else
         {
             GetComponent<SpriteRenderer>().flipX = false;
             GetComponent<SpriteRenderer>().flipY = false;
             transform.localPosition = new Vector3(0.65f, 0.2f, 0);
             transform.eulerAngles = new Vector3(0, 0, jugador.ang - 40);
         }*/
    }
    public void atacar()
    {
        ani.speed = jugador.attackSpeed * 5;
        ani.Play("Aespada");
    }

}
