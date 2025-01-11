using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_233 : Item
{
    //Declaramos stats o cosas que modificara el item


    // Start is called before the first frame update
    void Start()
    {
        //Declaracion del item
        player = GameObject.Find("player").GetComponent<Player>();
        descriptionItem = "";
        descripcionRecoger = "";
        nombre = "Item 233";
        unique = false;
        rarity = "";
        precio = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void grabItem(Player p)
    {
        base.grabItem(p);
        //ANADIMOS LAS STATS O LO QUE HAGA EL ITEM
        if (p.statCriticalChance >= 1 || p.statCriticalChance+0.2 > 1)
        {
            p.statCriticalChance = 1;
        }
        else
        {
            p.statCriticalChance += 0.2f;
        }
        
      


    }


}
