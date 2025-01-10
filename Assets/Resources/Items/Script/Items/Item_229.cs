using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_229 : Item
{
    //Declaramos stats o cosas que modificara el item

    float masVelAtq = 0.1f; //+50%
    // Start is called before the first frame update
    void Start()
    {
        //Declaracion del item
        player = GameObject.Find("player").GetComponent<Player>();
        descriptionItem = "Item 229";
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
        if (p.sword.attackSpeed <= 0.2f || p.sword.attackSpeed-0.1f < 0.2f)
        {
            p.sword.attackSpeed = 0.2f;
        }
        else
        {
            p.sword.attackSpeed -= masVelAtq;
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
