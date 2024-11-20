using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_16 : Item
{
    //Declaramos stats o cosas que modificara el item
    float attackKNC = 0.15f;

    // Start is called before the first frame update
    void Start()
    {
        //Declaracion del item
        player = GameObject.Find("player").GetComponent<Player>();
        descriptionItem = "Duplica el knockback a enemigos";
        descripcionRecoger = "COGIO EL ITEM 16";
        nombre = "ITEM 16";
        unique = false;
        rarity = "";
        precio = 1;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void grabItem(Player p)
    {
        base.grabItem(p);
        //ANADIMOS LAS STATS O LO QUE HAGA EL ITEM
        p.sword.knockbackMultiplicator += attackKNC; 
    }   
}
