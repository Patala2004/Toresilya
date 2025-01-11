using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class Item_89 : Item
{
    //Declaramos stats o cosas que modificara el item
    public float masCritDamage = 0.15f;

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
        base.grabItem2(p, nombre, descripcionRecoger);
        //ANADIMOS LAS STATS O LO QUE HAGA EL ITEM
        Item.debilCritDamage += masCritDamage;

    }


}
