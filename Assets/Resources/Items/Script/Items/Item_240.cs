using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_240 : Item
{
    //Declaramos stats o cosas que modificara el item


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

        p.sword.attackDamage[0] += 1;
        p.sword.attackDamage[1] += 1;


    }
}
