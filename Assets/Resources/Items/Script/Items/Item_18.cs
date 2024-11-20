using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_18 : Item
{
    //Declaramos stats o cosas que modificara el item
    public float addDamage = 0.2f;

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
        p.enterRoomMechanics.Add(damageUpWhenEnteringRoom);


    }

    public void damageUpWhenEnteringRoom()
    {
        player.sword.dmgMultiplicator += addDamage;
        StartCoroutine(rutinaEntrar());
    }

    public IEnumerator rutinaEntrar()
    {
        {
            yield return new WaitForSeconds(5f);
            player.sword.dmgMultiplicator -= addDamage;
        }



    }
}
