using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class Item_94 : Item
{
    //Declaramos stats o cosas que modificara el item
    public float masCriticalDamage = 0.15f;
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
        p.statCriticalDamage += (p.statCriticalDamage* masCriticalDamage);

    }

}
