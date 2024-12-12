using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingPotion : Item
{

    protected int healingAmm;

    // Start is called before the first frame update
    public virtual void Start()
    {
        descriptionItem = "Cura " + healingAmm + " vida";
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
