using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreTile : MonoBehaviour
{
    private bool itemHasBeenGrabbed = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.name != "player" || itemHasBeenGrabbed) return;
        GameObject item = transform.GetChild(0).gameObject;
        item.GetComponent<Item>().grabItem(other.gameObject.GetComponent<Player>());
        Destroy(item);
        itemHasBeenGrabbed = true;
    }
}
