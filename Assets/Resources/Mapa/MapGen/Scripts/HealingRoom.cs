using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HealingRoom : MonoBehaviour
{

    private float duration = 5f;
    private float hps;

    private bool entered = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.tag == "player" && !entered){
            hps = ((float)(other.gameObject.GetComponent<Player>().healthMax - other.gameObject.GetComponent<Player>().health))/duration;
            StartCoroutine(Regen(other.gameObject.GetComponent<Player>()));
            entered = true;
        }
    }


    public IEnumerator Regen(Player p){
        int nextSec = (int) duration;
        while(duration > 0f){
            if(nextSec >= duration && Time.deltaTime > 0){
                if(p.health == p.healthMax) yield return null;
                else if(p.health + hps >= p.healthMax) p.health = p.healthMax;
                else p.health += hps;
                nextSec = (int) (duration-Time.deltaTime);
            }
            else{
                duration -= Time.deltaTime;
            }
            yield return null;
        }
    }
}
