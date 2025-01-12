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
            if (33<= random && random <38) //5% de posibilidades
            {
                found = true;
                player.GetInvulnerable(5f);
            }
        }
    }

    
    

}
