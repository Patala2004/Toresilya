using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{

    private MapGen mapgen;


    // Start is called before the first frame update
    void Start()
    {
        mapgen = GameObject.Find("MapGenerator").GetComponent<MapGen>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(){
        // Clean TileSet
        mapgen.clearMap();
        mapgen.generateMap();
    }
}
