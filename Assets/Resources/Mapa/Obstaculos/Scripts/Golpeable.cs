using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golpeable : MonoBehaviour
{
    public float health = 6;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public virtual void TakeDamage(float damage)
    {
        health -= damage;
        if(health <= 0)
        {
            ToDie();
        }
    }
    public void ToDie()
    {
        Destroy(this.gameObject);
    }
}