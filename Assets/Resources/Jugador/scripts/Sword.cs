using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Sword : MonoBehaviour
{
    public Player player;
    Animator ani;
    SpriteRenderer sR;
    public float recoil = 10f; // recoil del arma(empuje cuando)
    public float attackKnockback = 4f;// lo lejos que envia al enemigo atacando
    public float knockbackMultiplicator = 1f;
    public float attackSpeed = 0.2f; // velocidad a la que el player puede atacar(no es la velocidad de la animacion)
    public float attackAnimation = 0.2f; // cuanto dura la animacion *importante que no sea mucho
    //to do manejar rng
    public float[] attackDamage = new float[2]; // da�o de da�omin a da�omax
    public float dmgMultiplicator = 1f;
    public Vector2 hitboxSize = new(1,2);
    float radioArma = 0.3f;
    float rotacionArma =0;
    // Start is called before the first frame update
    void Start()
    {
        ani = GetComponent<Animator>();
        sR = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
        //bloqueo
        if (player.blocking)
        {
            radioArma = 0.9f;
            rotacionArma = -50;
            sR.sortingLayerName = "dopamina";
        }
        else
        { 
            sR.sortingLayerName = "armas";
            radioArma = 0.3f;
            rotacionArma = 0;
        }
        //anim attack
        ani.SetBool("atacando", player.attackingAnimation);

    }
    public Enemy[][] HitboxPlayer(Vector2 position, Vector2 scale, float angle, Vector2 dir, float distance)
    {
        RaycastHit2D[] boxCast = Physics2D.BoxCastAll(position, scale, angle, dir, distance);
        List<Enemy> hitEnemies = new List<Enemy>();
        List<Enemy> criticallyHitEnemies = new List<Enemy>();
        Enemy[][] resArray = new Enemy[2][];
        foreach (RaycastHit2D collider in boxCast)
        {
            if (collider.collider.CompareTag("enemy"))
            {
                Enemy enemy = collider.collider.GetComponent<Enemy>();
                hitEnemies.Add(enemy);
                float dam = Random.Range(attackDamage[0], attackDamage[1]) * (dmgMultiplicator<0? 0:dmgMultiplicator); // cogemos el daño de la espada base
                bool critical = Random.Range(0f, 1f) < (player.statCriticalChance + (enemy.isDebil? Item.debilCritChance : 0)); // vemos si ha habido critico
                if (critical)
                {
                    dam *= player.statCriticalDamage;
                    criticallyHitEnemies.Add(enemy);
                }

                enemy.TakeDamage(dam, player.ang, attackKnockback * (knockbackMultiplicator<0? 0:knockbackMultiplicator));
            }
            if (collider.collider.CompareTag("golpeable"))
            {
                float dam = Random.Range(attackDamage[0], attackDamage[1]) * (dmgMultiplicator<0? 0:dmgMultiplicator);
                Golpeable golpeable = collider.collider.GetComponent<Golpeable>();
                golpeable.TakeDamage(dam);
            }
        }
        resArray[0] = hitEnemies.ToArray();
        resArray[1] = criticallyHitEnemies.ToArray();
        return resArray;
    }

    public void FixedUpdate(){
        //seguimiento del arma al player �
        if (player.ang > 90 || player.ang < -90)
        {
            sR.flipY = true;
            float x = Mathf.Cos((player.ang - rotacionArma) * Mathf.Deg2Rad) * radioArma;
            float y = Mathf.Sin((player.ang - rotacionArma) * Mathf.Deg2Rad) * radioArma;
            transform.localPosition = new Vector3(x, y-0.2f, 1);
            transform.localEulerAngles = new Vector3(0, 0, player.ang + 90);
        }
        else
        {
            sR.flipY = false;
            float x = Mathf.Cos((player.ang + rotacionArma) * Mathf.Deg2Rad) * radioArma;
            float y = Mathf.Sin((player.ang + rotacionArma) * Mathf.Deg2Rad) * radioArma;
            transform.localPosition = new Vector3(x, y - 0.2f, 1);
            transform.localEulerAngles = new Vector3(0, 0, player.ang - 90);
        } 
    }
    public void Attack()
    {
        
    }
    public void Block()
    {
        
    }
}
