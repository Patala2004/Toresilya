using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegenPotion : Item
{

    protected int hps; // healing per seccond
    protected float duration; // Duration of regen buff

    // Start is called before the first frame update
    public virtual void Start()
    {
        descriptionItem = "Cura " + hps + " vida por segundo durante " + duration + " segundos";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void grabItem(Player p)
    {
        Debug.Log(p.gameObject);
        StartCoroutine(Regen(p));
    }

    public IEnumerator Regen(Player p){
        int nextSec = (int) duration;
        while(duration > 0f){
            if(nextSec >= duration && Time.deltaTime > 0){
                p.health += hps;
                nextSec = (int) (duration-Time.deltaTime);
            }
            else{
                duration -= Time.deltaTime;
            }
            yield return null;
        }
    }
}
