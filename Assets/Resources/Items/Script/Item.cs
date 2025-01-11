using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public Player player;
    public string descriptionItem = "";
    public string descripcionRecoger = "ESTO ES UNA DESCRIPCION BUENA";
    public string nombre = "PRUEBA HOLIWI";
    public bool unique = false;
    public string rarity = "";
    // Duracion estandar de estados (es compartida)
    // Estado debil
    public static float durDebil = 2f;
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

        Debug.Log("El metodo grabItem ha sido llamado para el item " + nombre);
        transform.position = new Vector3(-10000, 10000, 0); // Mover a a tomar por culo para no tener que destruir 
        Barra hud = FindObjectOfType<Barra>();
        if (hud != null)
        {
            hud.ShowItemText(nombre, descripcionRecoger);
        }
        else
        {
            Debug.LogWarning("No se encontró el script Barra en la escena.");
        }

    }

    public virtual void grabItem2(Player p, string nombre, string descripcion)
    {
        // Add stats

        Debug.Log("El metodo grabItem ha sido llamado para el item " + nombre);
        transform.position = new Vector3(-10000, 10000, 0); // Mover a a tomar por culo para no tener que destruir 
        Barra hud = FindObjectOfType<Barra>();
        if (hud != null)
        {
            hud.ShowItemText(nombre, descripcion);
        }
        else
        {
            Debug.LogWarning("No se encontró el script Barra en la escena.");
        }

    }


}
