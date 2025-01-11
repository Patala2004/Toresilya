using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_73 : Item
{
    //Declaramos stats o cosas que modificara el item
    public bool aplicadoItem73=false;

    // Start is called before the first frame update
    void Start()
    {
        //Declaracion del item
        player = GameObject.Find("player").GetComponent<Player>();
        descriptionItem = "";
        descripcionRecoger = "";
        nombre = "Item 73";
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
        p.takeHealthDamageMechanics.Add(lessDamageWhenDebil);

    }

    public void lessDamageWhenDebil(Enemy e)
    {
        if (e.isDebil && !aplicadoItem73)
        {
            e.dmgMultiplicator -= 0.5f;
            aplicadoItem73 = true;
        } 
        else if(!e.isDebil && aplicadoItem73)
        {
            e.dmgMultiplicator += 0.5f;
            aplicadoItem73 = false;
        }
        
    }

}
