using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public Player player;
    public String descriptionItem = "";
    public String descripcionRecoger = "";
    public String nombre = "";
    public bool unique = false;

    public String rarity = "common";
    public int precio;

    // Start is called before the first frame update
    void Start()
    {
     
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void grabItem(Player p){
        // Add stats

        Debug.Log("El metodo grabItem ha sido llamado para el item " + nombre);

    }
    
    public virtual IEnumerator putOnCooldown(float seconds, bool cc)
    {
        yield return new WaitForSeconds(seconds);
        cc = true;
    }

}
