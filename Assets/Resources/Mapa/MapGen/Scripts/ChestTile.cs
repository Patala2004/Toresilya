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
}
