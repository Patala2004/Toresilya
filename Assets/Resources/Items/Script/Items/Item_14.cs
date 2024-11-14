using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_14 : Item
{
    //Declaramos stats o cosas que modificará el item
    float attackDMG = 1.1f; //Aumentaremos el 10%
    float attackKNC = 1.05f; //Aumentaremos el 15%

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
        p.sword.attackKnockback*=attackKNC; //AUMENTO EL KNOCKBACK
        p.sword.attackDamage[0] *= attackDMG;
        p.sword.attackDamage[1] *= attackDMG;
        p.attackMechanics.Add(printA);

    }
    public void printA(Enemy[] enemyList)
    {
        if (enemyList.Length == 0)
        {
            Debug.Log("MECANICA DE ATAAQUE RAHHHHHHHHHHHHH");
        }
        else
        {
            foreach (Enemy e in enemyList)
            {
                Debug.Log("MECANICA EXtrA AL TONTO DE " + e.gameObject.name);
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica si el objeto que ha tocado el ítem tiene la etiqueta "player"
        if (collision.CompareTag("player"))
        {
            grabItem(player); // Llamamos a la funcion de recoger el item
        }
    }

   

}

