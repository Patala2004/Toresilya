using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_231 : Item
{
    //Declaramos stats o cosas que modificara el item

    //+200% del base
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
            p.sword.attackSpeed = 0.1f;
        
    }

}
