using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

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
            priceTag.color = new Color32(255,0,0,255);
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
            player.monedas -= precio;
            //Destroy(item); // Ahora el grabitem los mueve a lejos y no hace falta destruir

            priceTag.text = "";//"<s>  " + priceTag.text + "  </s>";
            priceTag.color = new Color32(100,100,100,255); // Grey out the price
            itemDescription.enabled = false;
            this.enabled = false;
            gameObject.GetComponent<Light2D>().enabled = false;
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
        precio = genRandomPrecio(item.minPrecio, item.maxPrecio);
        priceTag.text = precio.ToString();
        itemDescription.color = colorSegunRareza(item.rarity);
        itemDescription.text = "Press [E] to grab" + '\n' +  item.descriptionItem;
    }

    public Color32 colorSegunRareza(string Rareza)
    {
        switch (Rareza) {
            case "comun": return new Color32(165, 165, 165, 255); //Gris oscuro -> comun
            case "pocoComun": return new Color32(0, 148, 9, 255); //verde oscuro -> pocoComun
            case "raro": return new Color32(15, 178, 227, 255); //azul -> raro
            case "epico": return new Color32(142, 32, 239, 255); //morado -> epico
            case "legendario": return new Color32(250, 250, 63, 255); //amarillo -> lengendario
        }
        return new Color32(100, 100, 100, 255); //El mismo que el del precio, imposible que llegue
    }

    public int genRandomPrecio(int min, int max)
    {
        int random = Random.Range(min, max);
        return random;
    }
}
