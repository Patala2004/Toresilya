using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_219 : Item
{
    //Declaramos stats o cosas que modificara el item
    float masVelMov = 0.10f; //25%

    // Start is called before the first frame update
    void Start()
    {
        //Declaracion del item
        player = GameObject.Find("player").GetComponent<Player>();
        descriptionItem = "";
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

        p.velWalk = p.velWalk + (masVelMov * p.velWalk);


    }

}
