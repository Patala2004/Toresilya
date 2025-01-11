using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class Item_96 : Item
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
        p.criticalAttackMechanics.Add(metodoItem96);


    }
    //ESTE ITEM ESTA OP AS FUCK CON BUEN CRITICO
    public void metodoItem96(Enemy[] eList)
    {
        foreach (Enemy e in eList)
        {
            if (e.multiplicadorDefensa > 0)
            {
                e.multiplicadorDefensa -= 0.2f;
            }
        }
    }

   
}
