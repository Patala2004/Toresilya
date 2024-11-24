using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goop : Projectile
{
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }
    new void FixedUpdate()
    {
        base.FixedUpdate();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public new void ToDie()
    {
        Destroy(gameObject);
    }
}
