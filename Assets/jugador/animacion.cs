using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class animacion : MonoBehaviour
{
    // Start is called before the first frame update
    public jugador jugador;
    SpriteRenderer sR;
    public float Alpha;
    void Start()
    {
        sR = GetComponent<SpriteRenderer>();
        Alpha = 0;
    }

    // Update is called once per frame
    void Update()
    {
        sR.color = new Color(1, 1, 1, Alpha);
        if(Alpha > 0)
        {
            Alpha -= 3f * Time.deltaTime;
        }
        else
        {
            transform.position = new Vector3 (0, 100000, 0);
        }
    }
    public void comenzar()
    {
        float x = Mathf.Cos(jugador.ang * Mathf.Deg2Rad) * 1f;
        float z = Mathf.Sin(jugador.ang * Mathf.Deg2Rad) * 1f;
        transform.localPosition = new Vector3(x, z, 0);
        transform.localEulerAngles = new Vector3(0, 0, jugador.ang - 90);
        Alpha = 1;
    }
}
