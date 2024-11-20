using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_1 : Item
{
    //Declaramos stats o cosas que modificará el item
    int attackDMG = 10000;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("player").GetComponent<Player>();
        descriptionItem = "Añade 50 de daño por ataque";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void grabItem(Player p)
    {
        base.grabItem(p);
        p.sword.attackDamage[0]+=attackDMG;
        p.sword.attackDamage[1]+=attackDMG;

        p.attackMechanics.Add(printA);
 
    }


    public void printA(Enemy[] enemyList){
        if(enemyList.Length == 0){
            Debug.Log("MECANICA DE ATAAQUE RAHHHHHHHHHHHHH");
        }
        else{
            foreach(Enemy e in enemyList){
                Debug.Log("MECANICA EXtrA AL TONTO DE " + e.gameObject.name);
            }
        }
    }

}
