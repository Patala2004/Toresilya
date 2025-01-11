using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class Item_72 : Item
{
    //Declaramos stats o cosas que modificara el item
    public float addDefense = 0.05f; //Siempre es fija salvo un 0.05 extra si enemigo debil (enemigo hace un 5%  menos de dano)
    public bool aplicadoItem72 = false;

    // Start is called before the first frame update
    void Start()
    {
        //Declaracion del item
        player = GameObject.Find("player").GetComponent<Player>();
        descriptionItem = "";
        descripcionRecoger = "";
        nombre = "Item 72";
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
        p.multiplicadorDefensa += addDefense;
        p.takeHealthDamageMechanics.Add(metodoItem72);
    }
    public void metodoItem72(Enemy e)
    {
        if (e.isDebil && !aplicadoItem72)
        {
            e.dmgMultiplicator -= 0.05f;
            aplicadoItem72 = true;
        }
        else if (!e.isDebil && aplicadoItem72)
        {
            e.dmgMultiplicator += 0.05f;
            aplicadoItem72 = false;
        }
    }
}
