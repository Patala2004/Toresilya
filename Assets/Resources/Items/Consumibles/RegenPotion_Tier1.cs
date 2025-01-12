using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegenPotion_Tier1 : RegenPotion
{


    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        hps = 1;
        duration = 20f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
