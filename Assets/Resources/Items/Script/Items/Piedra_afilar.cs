using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Piedra_afilar : Item
{
    int attackDMG = 1;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("player").GetComponent<Player>();
        descriptionItem = "A�ade 1 de ataque";
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void grabItem(Player p)
    {
        base.grabItem(p);
        p.sword.attackDamage[0] += 1;
        p.sword.attackDamage[1] += 1;
    }

}
