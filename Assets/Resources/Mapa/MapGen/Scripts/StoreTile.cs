using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StoreTile : MonoBehaviour
{
    private bool itemHasBeenGrabbed = false;


    private int precio;

    private Item item;

    public TextMeshPro priceTag;
    public TextMeshPro itemDescription;

    public Player player;

    private bool entered = false;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame

    void FixedUpdate(){
        if(precio > player.monedas){
            priceTag.color = new Color32(200,0,0,255);
        }
        else{
            priceTag.color = new Color32(255,255,255,255);
        }
    }
    void Update()
    {
        if(entered && Input.GetKey(KeyCode.E) && precio <= player.monedas){
            item.grabItem(player);
            itemHasBeenGrabbed = true;
            //Destroy(item); // Ahora el grabitem los mueve a lejos y no hace falta destruir

            priceTag.text = "<s>  " + priceTag.text + "  </s>";
            priceTag.color = new Color32(100,100,100,255); // Grey out the price
            itemDescription.enabled = false;
            this.enabled = false;
        }
    }

    public void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.name != "player" || itemHasBeenGrabbed) return;

        itemDescription.enabled = true;
        entered = true;        
    }

    public void OnTriggerExit2D(Collider2D other){
        if(other.gameObject.name != "player" || itemHasBeenGrabbed) return;

        itemDescription.enabled = false;
        entered = false;
    }

    public void SetItem(GameObject newItem) {
        newItem.transform.SetParent(transform, false);
        newItem.transform.localPosition = new Vector2(0,0);
        item = newItem.GetComponent<Item>();
        precio = UnityEngine.Random.Range(20,45);
        priceTag.text = precio.ToString();
        itemDescription.text = "Press [E] to grab" + '\n' +  item.descriptionItem;
    }
}
