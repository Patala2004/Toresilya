using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_163 : Item
{
    //Declaramos stats o cosas que modificara el item

    // Start is called before the first frame update
    void Start()
    {
        //Declaracion del item
        player = GameObject.Find("player").GetComponent<Player>();
        descriptionItem = "";
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
        p.item_163 = true;
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

    //Metodo que mira cuando el objeto haya sido tocado por el jugador
    public void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica si el objeto que ha tocado el �tem tiene la etiqueta "player"
        if (collision.CompareTag("player"))
        {
            grabItem(player); // Llamamos a la funcion de recoger el item
        }
    }
}
