using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using TMPro;
using UnityEngine;
using UnityEngineInternal;

public class GenParticulaTexto : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject particula;
    public float particulaVel = 8;
    public float particulaDispersion = 40;
    public float particulaUpRig = 0.4f;
    void Start()
    {
        particula = Resources.Load<GameObject>("Particulas/particulaTexto");
    }

    // Update is called once per frame
    void Update()
    {
   
    }
    public void comenzar(float damage,float ang, Color color = default)
    {

        if(color == default) color = Color.red;

        //Calculamos dispersion de la particula
        ang = Random.Range(ang - particulaDispersion, ang + particulaDispersion);
        //Calculamos vector direcci�n
        Vector2 dir = new Vector3(Mathf.Cos(ang * Mathf.Deg2Rad), Mathf.Sin(ang * Mathf.Deg2Rad));
        dir += Vector2.up * particulaUpRig; 
        //Instanciamos la particula
        ParticulaTexto temp = Instantiate(particula, new Vector3(transform.position.x, transform.position.y, 0), transform.rotation).GetComponent<ParticulaTexto>();
        temp.gameObject.GetComponent<TextMeshPro>().color = color;
        temp.vel = dir * particulaVel;
        temp.damage = damage;
       
    }
}
