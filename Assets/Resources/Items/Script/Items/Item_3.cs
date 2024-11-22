using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Item_3 : Item
{
    //Declaramos stats o cosas que modificará el item

    // Start is called before the first frame update

    // Este item hace que los rayos lanzados por el jugador salten una vez más


    void Start()
    {
        player = GameObject.Find("player").GetComponent<Player>();
        descriptionItem = "Duplica la cantidad de veces que rebotan los rayos entre enemigos";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void grabItem(Player p)
    {
        base.grabItem(p);
        Item.multRebotesRayo = Item.multRebotesRayo + 1;
 
    }
}
