using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_80 : Item
{
    //Declaramos stats o cosas que modificara el item
    public float aumentoDurDebil = 2f;

    // Start is called before the first frame update
    void Start()
    {
        //Declaracion del item
        player = GameObject.Find("player").GetComponent<Player>();
        descriptionItem = "";
        descripcionRecoger = "";
        nombre = "Item 80";
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
        p.perfectParryMechanics.Add(perfectBlocktoDebil);
    }

    public void perfectBlocktoDebil(Enemy enemy)
    {
            enemy.isDebil = true;
            enemy.debilTime = Item.durDebil;
            
       
    }
}
