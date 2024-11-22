using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileGen : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject projectile;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Lanzar_Projectil(Vector3 pos,Vector2 dir,float[] damage,float knockback)
    {
        Projectile temp = Instantiate(projectile,pos,Quaternion.identity).GetComponent<Projectile>();
        temp.direction = dir;
        temp.damage = damage;
        temp.knockback = knockback;
    }
}
