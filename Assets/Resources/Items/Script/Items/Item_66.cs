using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_66 : Item
{
    //Declaramos stats o cosas que modificara el item
    float addDamage = 0.15f;
    float addProbDebil = 30f;

    // Start is called before the first frame update
    void Start()
    {
        //Declaracion del item
        player = GameObject.Find("player").GetComponent<Player>();
       
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void grabItem(Player p)
    {
        base.grabItem(p);
        ItemGenerator.activarDebil();
        //ANADIMOS LAS STATS O LO QUE HAGA EL ITEM
        p.sword.dmgMultiplicator += addDamage;
        if (Item.probDebil == 0)
        {
            p.attackMechanics.Add(anadirDebil);
        }
        Item.probDebil += addProbDebil;
    }

    public void anadirDebil(Enemy[] enemyList) {
        for (int i = 0; i < enemyList.Length; i++)
        {
            int random = Random.Range(1, 101);
            if(random < Item.probDebil)
            {
                Enemy enemyAct = enemyList[i];
                enemyAct.Debil(Item.durDebil);
            }
        }
    }

}

