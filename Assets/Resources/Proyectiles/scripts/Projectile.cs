using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody2D rb;
    public Vector2 direction = Vector2.left;
    public Vector2 hitboxSixe = new(1,1);
    public float[] damage = {2,3};
    public float knockback = 30;
    public float vel = 2;
    bool allowAttack = true;
    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = direction * vel;
        transform.localEulerAngles = new Vector3(0,0, (Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x))+180);
    }
    public void FixedUpdate()
    {
        HitboxEnemy(transform.position, hitboxSixe, Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x), direction, 0, damage, knockback);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void HitboxEnemy(Vector2 position, Vector2 scale, float angle, Vector2 dir, float distance, float[] damage, float knockback)
    {
        RaycastHit2D[] boxCast = Physics2D.BoxCastAll(position, scale, angle, dir, distance);
        foreach (RaycastHit2D collider in boxCast)
        {
            if (collider.collider.CompareTag("player") && allowAttack)
            {
                allowAttack = false;
                float dm = Random.Range(damage[0], damage[1]);
                collider.collider.GetComponent<Player>().TakeDamage(dm, Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x), knockback, gameObject);
                ToDie();
            }
            if (collider.collider.CompareTag("golpeable"))
            {
                float dam = Random.Range(damage[0], damage[1]);
                Golpeable golpeable = collider.collider.GetComponent<Golpeable>();
                golpeable.TakeDamage(dam);
                ToDie();
            }
        }
    }
    public virtual void ToDie()
    {
        rb.velocity = Vector2.zero;
        Destroy(gameObject, 5);
    }
    public virtual void ToGetParried()
    {
        Destroy(gameObject);
    }
}
