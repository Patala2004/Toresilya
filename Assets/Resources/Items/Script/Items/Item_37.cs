using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_37 : Item
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
        player.attackMechanics.Add(randomAttackToInmune);
        

    }

    public void randomAttackToInmune(Enemy[] enemylist)
    {
        int random = Random.Range(1, 101);
        if (random == 33) //1% de posibilidades
        {
            player.invulnerable = true;
            StartCoroutine(rutinaInmune());
        }
    }

    // Esto deberia de estar implementado en player
    // invunerable deberia ser un float que va disminuyendo cada FixedUpdate
    // Error posible: Si se llama a rutinaInmune dos veces en dos momentos distintos pero muy cercanos la primera rutinaInmune desactivaria la invulnerabilidad 
    // de la segunda llamada antes de que se tuviera que desactivaar
    // Basicamente, hay una condición de carrera mal llevada

    public IEnumerator rutinaInmune()
    {
        yield return new WaitForSeconds(1.5f);
        player.invulnerable = false;
    }

}
