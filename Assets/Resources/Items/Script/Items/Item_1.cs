using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

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
            enemy.TakeDamage(UnityEngine.Random.Range(player.sword.attackDamage[0], player.sword.attackDamage[1]) * multDanoRayo, player.ang, player.sword.attackKnockback * (player.sword.knockbackMultiplicator<0? 0:player.sword.knockbackMultiplicator), new Color(0,0,0.5f,1));
            rayo.transform.position = enemy.gameObject.transform.position;
            rayo.pointA = player.transform;
            rayo.pointB = enemy.transform;
            rayo.setDuration(0.3f);
            
            ponerEstados(enemy);

            reboteRayo(enemy, nRebotesRayo - 1, player.gameObject);

        }
    }

    private void ponerEstados(Enemy enemy){
        // Poner estados
            if(Random.Range(0f,1f) < probCaosRayo){
                // Poner estado caos
            }

            if(Random.Range(0f,1f) < probParalizarRayo){
                // Poner paralizar en el enemigo
                enemy.Paralize(durParalizadoRayo);
            }

            // ...
    }

    private void reboteRayo(Enemy e, int remainingIterations, GameObject origin){
        Debug.Log("ReBOTE");

        if(remainingIterations == 0) return;

        RaycastHit2D[] hit = Physics2D.CircleCastAll(e.transform.position, 5f, new Vector3(0f,0f,0f), 0);
        // hit is ordered by distance of origin -> closes enemy will be first
        // search for first enemy in hit
        for(int i = 0; i < hit.Length; i++){
            if((hit[i].collider.gameObject.tag == "enemy") && (hit[i].collider.gameObject != origin) && (hit[i].collider.gameObject != e.gameObject)){
                Debug.Log(hit[i].collider.gameObject);
                Enemy target = hit[i].collider.gameObject.GetComponent<Enemy>();
                // Do damage and connect lighting to it
                target.TakeDamage(UnityEngine.Random.Range(player.sword.attackDamage[0], player.sword.attackDamage[1]) * multDanoRayo, player.ang, player.sword.attackKnockback * (player.sword.knockbackMultiplicator<0? 0:player.sword.knockbackMultiplicator), Color.yellow);
                
                Lightning_effect rayo = Instantiate(Resources.Load<GameObject>("Items/Prefabs efectos/rayo")).GetComponent<Lightning_effect>();

                rayo.transform.position = target.transform.position;
                rayo.pointA = e.transform;
                rayo.pointB = target.transform;
                rayo.setDuration(0.3f);

                ponerEstados(target);

                reboteRayo(target, remainingIterations - 1, e.gameObject);
                return;
            }
        }
    }


    // Debug
    void OnTriggerEnter2D(){
        grabItem(player);
    }


}
