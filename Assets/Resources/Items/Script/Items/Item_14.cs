using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_14 : Item
{
    //Declaramos stats o cosas que modificar� el item
    float attackDMGMult = 0.1f; //Aumentaremos el 10%
    float attackKNC = 0.05f; //Aumentaremos el 15%

    // Start is called before the first frame update
    void Start()
    {
        //Declaracion del item
        player = GameObject.Find("player").GetComponent<Player>();
        descriptionItem = "Tus ataques hacen mas daño y empujan hacia atras un poco mas a los enemigos";
        descripcionRecoger = "RAW";
        nombre = "ITEM 14";
        unique = false;
        rarity = "common";
        precio = 9999999;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void grabItem(Player p)
    {
        base.grabItem(p);
        //ANADIMOS LAS STATS O LO QUE HAGA EL ITEM
        p.sword.knockbackMultiplicator +=attackKNC; //AUMENTO EL KNOCKBACK
        p.sword.dmgMultiplicator += attackDMGMult;

    }
}

