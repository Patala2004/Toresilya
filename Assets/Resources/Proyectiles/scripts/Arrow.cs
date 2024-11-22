using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : Projectile
{
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        Destroy(gameObject,5);
    }

    // Update is called once per frame
    private new void FixedUpdate()
    {
        base.FixedUpdate();
    }
    void Update()
    {
        
    }
}
