using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingPotion_Tier1 : HealingPotion
{


    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        healingAmm = 10;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void grabItem(Player p)
    {
        p.health += healingAmm;
    }
}
