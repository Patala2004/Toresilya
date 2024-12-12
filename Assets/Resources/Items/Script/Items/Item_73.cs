using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_79 : Item
{
    //Declaramos stats o cosas que modificara el item
    public bool aplicadoItem79=false;

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

        


    }

    public void lessDamageWhenDebil(Enemy e)
    {
        if (e.isDebil && !aplicadoItem79)
        {
            e.dmgMultiplicator = 0.5f;
            aplicadoItem79 = true;
        } 
        else if(!e.isDebil && aplicadoItem79)
        {
            e.dmgMultiplicator = 1f;
            aplicadoItem79 = false;
        }
        
    }

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
