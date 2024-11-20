using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_1 : Item
{
    //Declaramos stats o cosas que modificará el item

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("player").GetComponent<Player>();
        descriptionItem = "Lanza un rayo cuando atacas a un enemigo";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void grabItem(Player p)
    {
        base.grabItem(p);
 
    }



}
