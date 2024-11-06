using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{

    private MapGen mapgen;
    private GameObject minimap;


    // Start is called before the first frame update
    void Start()
    {
        mapgen = GameObject.Find("MapGenerator").GetComponent<MapGen>();
        minimap = GameObject.Find("MiniMap");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.name != "player") return;
        other.gameObject.transform.position = new Vector3(-1,-1,0); // Antes de limpiar minimapa y generar nada nuevo para que no haya problemas con habitaciones generadas donde esta el jugador esperando
        // Clean TileSet
        mapgen.clearMap();
        mapgen.generateMap();
        foreach (Transform child in minimap.transform){
            Destroy(child.gameObject);
        }
    }
}
