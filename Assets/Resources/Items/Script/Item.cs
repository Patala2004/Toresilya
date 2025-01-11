using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{

    public Player player;
    public string descriptionItem = "";
    public string descripcionRecoger = "";
    public string nombre = "";
    public bool unique = false;
    public string rarity = "";
    // Duracion estandar de estados (es compartida)
    // Estado debil
    public static float durDebil = 4f;
    public static float probDebil = 0f;
    public static float debilDefReductionMult = 0.8f;
    public static float debilCritChance = 0f;
    public static float debilCritDamage = 0f;
    // Rayis
    public static float multDanoRayo = 0.5f;
    public static float probParalizarRayo = 0.05f;
    public static float probCaosRayo = 0f;
    public static float durParalizadoRayo = 2f;
    public static float durCaosRayo = 2f;
    public static int nBurst = 1;
    public static int nRebotesRayo = 1;
    public static int multRebotesRayo = 1;

    
    public int precio;
    public int minPrecio;
    public int maxPrecio;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
   
    }

    public virtual void grabItem(Player p)
    {
        // Add stats
        GameObject listElement = Instantiate(Resources.Load<GameObject>("Hud/Prefabs/ItemListElement"));
        listElement.GetComponent<Image>().sprite = this.gameObject.GetComponent<SpriteRenderer>().sprite;
        listElement.GetComponent<ItemListElement>().itemName = this.name;
        listElement.GetComponent<ItemListElement>().itemDescription = this.descriptionItem;
        listElement.transform.position = new Vector3(67000,-69420);
        ItemList.itemList.Add(listElement);
        //ItemList.instance.PrepareItemList();

        Debug.Log("El metodo grabItem ha sido llamado para el item " + nombre);
        transform.position = new Vector3(-10000, 10000, 0); // Mover a a tomar por culo para no tener que destruir 
        Barra hud = FindObjectOfType<Barra>();
        if (hud != null)
        {
            hud.ShowItemText(nombre, rarity);
        }
        else
        {
            Debug.LogWarning("No se encontr� el script Barra en la escena.");
        }

    }

    // public virtual void grabItem2(Player p, string nombre, string descripcion)
    // {
    //     // Add stats

    //     Debug.Log("El metodo grabItem ha sido llamado para el item " + nombre);
    //     transform.position = new Vector3(-10000, 10000, 0); // Mover a a tomar por culo para no tener que destruir 
    //     Barra hud = FindObjectOfType<Barra>();
    //     if (hud != null)
    //     {
    //         hud.ShowItemText(nombre, descripcion);
    //     }
    //     else
    //     {
    //         Debug.LogWarning("No se encontr� el script Barra en la escena.");
    //     }

    // }


}