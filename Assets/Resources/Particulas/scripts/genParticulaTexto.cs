using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class genParticulaTexto : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject particula;
    public float particulaVel = 2;
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
    public void comenzar(int damage,float ang)
    {
        //Calculamos dispersion de la particula
        ang = Random.Range(ang - particulaDispersion, ang + particulaDispersion);
        //Calculamos vector dirección
        Vector2 dir = new Vector3(Mathf.Cos(ang * Mathf.Deg2Rad), Mathf.Sin(ang * Mathf.Deg2Rad));
        dir += Vector2.up * particulaUpRig; 
        //Instanciamos la particula
        particulaTexto temp = Instantiate(particula, new Vector3(transform.position.x, transform.position.y, 0), transform.rotation).GetComponent<particulaTexto>();
        temp.vel = dir * particulaVel;
        temp.damage = damage;
       
    }
}
