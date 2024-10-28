using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_1 : Item
{

    int attackDMG = 50;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        description = "Añade 50 de daño por ataque";
    }

    public override void grabItem(Player p)
    {
        base.grabItem(p);
        p.sword.attackDamage[0]+=attackDMG;
        p.sword.attackDamage[1]+=attackDMG;
    }

}
