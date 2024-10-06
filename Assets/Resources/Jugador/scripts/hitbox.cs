using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class hitbox : MonoBehaviour
{
    // Start is called before the first frame update
    public jugador jugador;
    public float radioHitbox = 1f;// lo lejos que sale la hitbox del personaje(1 default)
    void Start()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void comenzar()
    {
        gameObject.SetActive(true);
        float x = Mathf.Cos(jugador.ang * Mathf.Deg2Rad) * radioHitbox;
        float z = Mathf.Sin(jugador.ang * Mathf.Deg2Rad) * radioHitbox;
        transform.localPosition = new Vector3(x, z, 1);
        transform.localEulerAngles = new Vector3(0, 0, jugador.ang);
        StartCoroutine(finalizar(jugador.arma.attackAnimation));
    }
    public IEnumerator finalizar(float waitseconds)
    {
        yield return new WaitForSeconds(waitseconds);
        gameObject.SetActive(false);
    }
}
