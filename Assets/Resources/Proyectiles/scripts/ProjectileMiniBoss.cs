using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMiniBoss : Projectile
{
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }
    new void FixedUpdate()
    {
        base.FixedUpdate();
        GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x , GetComponent<Rigidbody2D>().velocity.y) + (damage[1] / 10) * direction;
    }
    // Update is called once per frame
    void Update()
    {
  
    }
    public override void ToDie()
    {
        Destroy(gameObject);
    }
}
