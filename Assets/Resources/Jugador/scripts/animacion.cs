using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class animacion : MonoBehaviour
{
    // Start is called before the first frame update
    public jugador jugador;
    SpriteRenderer sR;
    void Start()
    {
        sR = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void comenzar()
    {
        float x = Mathf.Cos(jugador.ang * Mathf.Deg2Rad) * 1.5f;
        float z = Mathf.Sin(jugador.ang * Mathf.Deg2Rad) * 1.5f;
        transform.localPosition = new Vector3(x, z, 0);
        transform.localEulerAngles = new Vector3(0, 0, jugador.ang);
    }
    public void finalizar()
    {
        transform.localPosition = new Vector3(0, 1000, 0);
    }
}
