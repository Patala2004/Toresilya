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
        player.attackMechanics.Add(lanzarRayo);
 
    }

    public void lanzarRayo(Enemy[] e){
        foreach(Enemy enemy in e){
            // Lanzar rayo al enemigo
            Lightning_effect rayo = Instantiate(Resources.Load<GameObject>("Items/Prefabs efectos/rayo")).GetComponent<Lightning_effect>();
            enemy.TakeDamage(UnityEngine.Random.Range(player.sword.attackDamage[0], player.sword.attackDamage[1]) * multDanoRayo, player.ang, player.sword.attackKnockback * (player.sword.knockbackMultiplicator<0? 0:player.sword.knockbackMultiplicator));
            rayo.transform.position = enemy.gameObject.transform.position;
            rayo.pointA = player.transform;
            rayo.pointB = enemy.transform;
            rayo.setDuration(0.3f);
            // Poner estados
            if(Random.Range(0f,1f) < probCaosRayo){
                // Poner estado caos
            }

            if(Random.Range(0f,1f) < probParalizarRayo){
                // Poner paralizar en el enemigo
            }

            // ...

        }
    }

    public void OnTriggerEnter2D(){
        grabItem(player);
        Destroy(this);
    }

}
