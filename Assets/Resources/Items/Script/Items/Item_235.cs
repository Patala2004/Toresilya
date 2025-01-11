using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_235 : Item
{
    //Declaramos stats o cosas que modificara el item
    float addDamage = 0.1f; 
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
        //ANADIMOS LAS STATS O LO QUE HAGA EL ITEM
        if (p.statCriticalChance >= 1 || p.statCriticalChance + 0.1 > 1)
        {
            p.statCriticalChance = 1;
        }
        else
        {
            p.statCriticalChance += 0.1f;
        }
        p.sword.dmgMultiplicator += addDamage;



    }

    

}
