using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Script for parent-room-class. All room classes will inherit this one
// Script should contain basic attributes that every room should contain
// Specific rooms will then handle specific activities (Spawn enemies, spawn cover, spawn a chest, a merchant, a healing well etc)
public class Room : MonoBehaviour
{
    // Boolean variables to check if it has a corridor in that direction
    public bool north;
    public bool east;
    public bool south;
    public bool west;

    public int width; // x size
    public int length; // y size
    public int corridor_width;
    public int corridor_length_v; // Length for corridors that go north / south
    public int corridor_length_h; // Length for corridors that go west / east

    public int x;
    public int y; // Coordinates

    public String roomType;

    public BoxCollider2D collider;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void createCollider(){
        // Have to add parameters (size, position) first -> cant add it in start
        collider = this.gameObject.AddComponent<BoxCollider2D>();
        collider.transform.position = new Vector3Int(x + width/2,y + length/2,0);
        collider.isTrigger = true;
        collider.size = new Vector2(width, length);
    }
}
