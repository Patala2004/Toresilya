using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Item_2 : Item
{
    //Declaramos stats o cosas que modificará el item

    // Start is called before the first frame update

    // Este item hace que los rayos lanzados por el jugador salten una vez más


    void Start()
    {
        player = GameObject.Find("player").GetComponent<Player>();
        descriptionItem = "Aumenta la cantidad de veces que rebota el rayo entre enemigos";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void grabItem(Player p)
    {
        base.grabItem(p);
        Item.nRebotesRayo = Item.nRebotesRayo+1;
 
    }
}
