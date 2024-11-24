using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{

    public int value = 0;

    private const float floatDistance = 6f;
    private const float grabDistance = 1f;
    public GameObject player;

    private bool changedLayerMask = false;

    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("player");
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate(){
        float dist = (transform.position - player.transform.position).magnitude;
        if(dist < grabDistance){
            player.GetComponent<Player>().monedas += value;
            Debug.Log("DINERO += " + value);
            Destroy(gameObject);
        }
        else if(dist < floatDistance){
            if(!changedLayerMask){
                changedLayerMask = true;

                // Hacer que cuando se acerque al jugador que ignore todos los obstaculos
                rb.includeLayers = rb.includeLayers & ~(1 << LayerMask.NameToLayer("Default")); // Quitar default del include
                rb.excludeLayers = rb.excludeLayers | (1 << LayerMask.NameToLayer("Default")); // Añadir default al exclude
            }
            rb.velocity = (Vector2) (player.transform.position - transform.position).normalized * (dist / floatDistance + 1) * 10;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
