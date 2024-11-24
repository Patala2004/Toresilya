using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_162 : Item
{
    //Declaramos stats o cosas que modificara el item
    public bool aplicadoItem162=false;
    // Start is called before the first frame update
    void Start()
    {
        //Declaracion del item
        player = GameObject.Find("player").GetComponent<Player>();
        descriptionItem = "1223";
        descripcionRecoger = "";
        nombre = "Item 162";
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
        p.defenseTempAddMechanics.Add(metodoItem162);

    }

    public void metodoItem162()
    {
        if (player.health <= 0.4f * player.healthMax && !aplicadoItem162)
        {
            player.multiplicadorDefensa += 0.1f;
            aplicadoItem162 = true;
        }
        else if (player.health > 0.4f * player.healthMax &&aplicadoItem162 )
        {
            player.multiplicadorDefensa -= 0.1f;
            aplicadoItem162 = false;
        }
    }

   

}
