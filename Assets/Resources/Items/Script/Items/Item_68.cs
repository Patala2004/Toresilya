using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_68 : Item
{
    // Start is called before the first frame update
    void Start()
    {
        //Declaracion del item
        player = GameObject.Find("player").GetComponent<Player>();
        descriptionItem = "1223";
        descripcionRecoger = "";
        nombre = "";
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
        p.perfectParryMechanics.Add(perfectParryToDebil);
        

    }

    public void perfectParryToDebil(Enemy e)
    {
        e.Debil(Item.durDebil);
    }
    
}
