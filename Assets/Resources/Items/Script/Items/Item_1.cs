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

    public void lanzarRayo(Enemy[] e){
        foreach(Enemy enemy in e){
            // Lanzar rayo al enemigo
            
            enemy.TakeDamage(UnityEngine.Random.Range(player.sword.attackDamage[0], player.sword.attackDamage[1]), player.ang, player.sword.attackKnockback * (player.sword.knockbackMultiplicator<0? 0:player.sword.knockbackMultiplicator));
        }
    }

}
