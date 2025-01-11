using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChestTile : MonoBehaviour
{
    private bool itemHasBeenGrabbed = false;
    private bool entered = false;

    public Item item;

    public TextMeshPro text;
    private bool textEnabled;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(textEnabled && !entered){
            text.enabled = false;
            textEnabled = false;
        }
        else if(!textEnabled && entered){
            text.enabled = true;
            textEnabled = true;
        }

        if(entered && Input.GetKey(KeyCode.E)){
            item.grabItem(GameObject.Find("player").GetComponent<Player>());
            text.enabled = false;
            this.enabled = false; 
        }
    }

    public void OnTriggerEnter2D(Collider2D other){
        entered = true;
    }

    public void OnTriggerExit2D(Collider2D other){
        entered = false;
    }

    public void SetText(String description){
        text.text = "Press [E] to grab" + '\n' +  description;
        
    }
    public void SetTextColor(String rareza)
    {
        text.color=colorSegunRareza(rareza);
    }
    public Color32 colorSegunRareza(string Rareza)
    {
        switch (Rareza)
        {
            case "comun": return new Color32(165, 165, 165, 255); //Gris oscuro -> comun
            case "pocoComun": return new Color32(0, 148, 9, 255); //verde oscuro -> pocoComun
            case "raro": return new Color32(15, 178, 227, 255); //azul -> raro
            case "epico": return new Color32(142, 32, 239, 255); //morado -> epico
            case "legendario": return new Color32(250, 250, 63, 255); //amarillo -> lengendario
        }
        return new Color32(100, 100, 100, 255); //El mismo que el del precio, imposible que llegue
    }
}
