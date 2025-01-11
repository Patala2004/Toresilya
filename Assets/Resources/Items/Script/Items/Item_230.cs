using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_230 : Item
{
    //Declaramos stats o cosas que modificara el item

    float masVelAtq = 0.2f; //+100%
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
        if (p.sword.attackSpeed <= 0.2f || p.sword.attackSpeed-masVelAtq <0.2f)
        {
            p.sword.attackSpeed = 0.2f;
        }
        else
        {
            p.sword.attackSpeed -= masVelAtq;
        }
    }

}
