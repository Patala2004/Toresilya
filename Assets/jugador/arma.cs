using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class arma : MonoBehaviour
{
    public jugador jugador;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(jugador.ang > 90 || jugador.ang < -90)
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
        }
    }
}
