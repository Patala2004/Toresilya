using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_163 : Item
{
    //Declaramos stats o cosas que modificara el item
    public bool aplicadoItem163 = false;
    // Start is called before the first frame update
    void Start()
    {
        //Declaracion del item
        player = GameObject.Find("player").GetComponent<Player>();
        descriptionItem = "1223";
        descripcionRecoger = "";
        nombre = "Item 163";
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
        p.defenseTempAddMechanics.Add(metodoItem163);

    }

    public void metodoItem163()
    {
        if (player.health <= 0.2f * player.healthMax && !aplicadoItem163)
        {
            player.multiplicadorDefensa += 0.2f;
            aplicadoItem163 = true;
        }
        else if (player.health > 0.2f * player.healthMax && aplicadoItem163)
        {
            player.multiplicadorDefensa -= 0.2f;
            aplicadoItem163 = false;

        }
    }
    
}
