using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//SCRIPT USADO PARA HACER OBJETOS MAS RAPIDOS CON UNA ESTRUCTURA BASE, NO SE USA EN EL JUEGO!!!
//SCRIPT USADO PARA HACER OBJETOS MAS RAPIDOS CON UNA ESTRUCTURA BASE, NO SE USA EN EL JUEGO!!!
//SCRIPT USADO PARA HACER OBJETOS MAS RAPIDOS CON UNA ESTRUCTURA BASE, NO SE USA EN EL JUEGO!!!
//SCRIPT USADO PARA HACER OBJETOS MAS RAPIDOS CON UNA ESTRUCTURA BASE, NO SE USA EN EL JUEGO!!!
//SCRIPT USADO PARA HACER OBJETOS MAS RAPIDOS CON UNA ESTRUCTURA BASE, NO SE USA EN EL JUEGO!!!
public class Item_Base_Structure : Item
{
    //Declaramos stats o cosas que modificara el item


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

    //FUNCIONES QUE NECESITE EL OBJETO EXTRA


    //RECORDATORIO PARA ALVARO (O QUIEN SEA) ELIMINAR COMENTARIOS INECESARIOS COMO ESTE
    //RECORDATORIO PARA ALVARO (O QUIEN SEA) ELIMINAR COMENTARIOS INECESARIOS COMO ESTE
    //RECORDATORIO PARA ALVARO (O QUIEN SEA) ELIMINAR COMENTARIOS INECESARIOS COMO ESTE
    //RECORDATORIO PARA ALVARO (O QUIEN SEA) ELIMINAR COMENTARIOS INECESARIOS COMO ESTE
    //RECORDATORIO PARA ALVARO (O QUIEN SEA) ELIMINAR COMENTARIOS INECESARIOS COMO ESTE
    //RECORDATORIO PARA ALVARO (O QUIEN SEA) ELIMINAR COMENTARIOS INECESARIOS COMO ESTE
}

