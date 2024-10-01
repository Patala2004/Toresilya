using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class genParticulaTexto : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject particula;
    public float particulaVel;
    public float particulaDispersion;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void comenzar(int damage,float ang,float attackKnockback)
    {
        //Calculamos dispersion de la particula
        ang = Random.Range(ang - particulaDispersion, ang + particulaDispersion);
        //Calculamos vector dirección
        Vector2 dir = new Vector3(Mathf.Cos(ang * Mathf.Deg2Rad), Mathf.Sin(ang * Mathf.Deg2Rad));
        //Instanciamos la particula
        particulaTexto temp = Instantiate(particula, new Vector3(transform.position.x, transform.position.y, 0), transform.rotation).GetComponent<particulaTexto>();
        temp.vel = dir * particulaVel * attackKnockback;
        temp.damage = damage;
       
    }
}
