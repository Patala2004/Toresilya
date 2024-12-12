using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_67 : Item
{
    //Declaramos stats o cosas que modificara el item
    float addProbDebil = 40f;

    // Start is called before the first frame update
    void Start()
    {
        //Declaracion del item
        player = GameObject.Find("player").GetComponent<Player>();
        descriptionItem = "1223";
        descripcionRecoger = "";
        nombre = "Item 66";
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
        if (Item.probDebil == 0)
        {
            p.attackMechanics.Add(anadirDebil);
        }
        Item.probDebil += addProbDebil;

    }

    public void anadirDebil(Enemy[] enemyList)
    {
        for (int i = 0; i < enemyList.Length; i++)
        {
            int random = Random.Range(1, 101);
            if (random < Item.probDebil)
            {
                Enemy enemyAct = enemyList[i];
                enemyAct.Debil(Item.durDebil);
            }
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
