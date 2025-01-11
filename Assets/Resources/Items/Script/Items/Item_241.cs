using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_241 : Item
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
        if (p.statCriticalChance >= 1 || (p.statCriticalChance + 0.05f) > 1)
        {
            p.statCriticalChance = 1;
        }
        else
        {
            p.statCriticalChance += 0.05f;
        }

        if (p.sword.attackSpeed <= 0.2f || (p.sword.attackSpeed - 0.05f) < 0.2f)
        {
            p.sword.attackSpeed = 0.2f;
        }
        else
        {
            p.sword.attackSpeed -= 0.05f;
        }

        p.sword.dmgMultiplicator += 0.05f;


    }
}
