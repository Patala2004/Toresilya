using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class Item_56 : Item
{
    //Declaramos stats o cosas que modificara el item
    public float disminuirVelocidad = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        //Declaracion del item
        player = GameObject.Find("player").GetComponent<Player>();
       
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void grabItem(Player p)
    {
        base.grabItem(p);
        //ANADIMOS LAS STATS O LO QUE HAGA EL ITEM
        p.parryMechanics.Add(metodoDisminuirVelocidad);

    }

    public void metodoDisminuirVelocidad(Enemy e)
    {
        e.multiplicadorVelocidad -= disminuirVelocidad;
        if (e.multiplicadorVelocidad< 0){
            e.multiplicadorVelocidad = 0;
        }
    }

}
