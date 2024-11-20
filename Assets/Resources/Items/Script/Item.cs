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

    // Duracion estandar de estados (es compartida)
    // Estado debil
    public static float durDebil = 2f;

    // Rayis
    public static float multDanoRayo = 0.5f;
    public static float probParalizarRayo = 0.05f;
    public static float probCaosRayo = 0f;
    public static int nBurst = 1;

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
    

}
