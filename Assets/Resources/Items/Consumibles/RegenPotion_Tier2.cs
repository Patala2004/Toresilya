using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegenPotion_Tier2 : RegenPotion
{


    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        hps = 5;
        duration = 90f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
