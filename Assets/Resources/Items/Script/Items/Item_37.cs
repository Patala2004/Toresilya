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
        bool found = false;
        for(int i = 0; i < enemylist.Length && !found; i++){
            int random = Random.Range(1, 101);
            if (random == 33) //1% de posibilidades
            {
                found = true;
                player.GetInvulnerable(1.5f);
            }
        }
    }

    // Esto deberia de estar implementado en player
    // invunerable deberia ser un float que va disminuyendo cada FixedUpdate
    // Error posible: Si se llama a rutinaInmune dos veces en dos momentos distintos pero muy cercanos la primera rutinaInmune desactivaria la invulnerabilidad 
    // de la segunda llamada antes de que se tuviera que desactivaar
    // Basicamente, hay una condición de carrera mal llevada
    
    public void OnTriggerEnter2D(Collider2D other)
    {
        // Verificar si el objeto con el que colisionamos es el jugador
        if (other.gameObject.CompareTag("player"))
        {
            grabItem(player);
            Debug.Log("TUS MUERTOS");
            transform.position = new Vector3(10000, 100000, transform.position.z);// Destruir el objeto después de recogerlo
        }
    }

}
