using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

// Script for parent-room-class. All room classes will inherit this one
// Script should contain basic attributes that every room should contain
// Specific rooms will then handle specific activities (Spawn enemies, spawn cover, spawn a chest, a merchant, a healing well etc)
public class Room : MonoBehaviour
{
    // Boolean variables to check if it has a corridor in that direction
    private bool north;
    private bool east;
    private bool south;
    private bool west;

    public int width; // x size
    public int length; // y size

    public int x;
    public int y; // Bottom Left Corner Coordinates

    public int roomType;


    private MapGen mapManager;
    private EnemySpawner enemySpawner;

    public BoxCollider2D roomCollider;
    // Start is called before the first frame update
    void Start()
    {
        mapManager = GameObject.Find("MapGenerator").GetComponent<MapGen>();
        enemySpawner = GameObject.Find("EnemySpawner").GetComponent<EnemySpawner>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other){
        Debug.Log("ENTERED");
        // If collision was with player and the room is an EnemyRoom
        if(other.gameObject.name == "player" && roomType > 4){
            CloseCorridors();
            enemySpawner.SpawnEnemy(x + width/2, y + length/2);
        }
    }

    private void CloseCorridors(){
        int arrSize = 0;
        if(north) arrSize+= RoomType.CORRIDOR_WIDTH;
        if(east) arrSize+= RoomType.CORRIDOR_WIDTH;
        if(south) arrSize+= RoomType.CORRIDOR_WIDTH;
        if(west) arrSize+= RoomType.CORRIDOR_WIDTH;
        Tile doorTile = mapManager.doorTile;
        Tilemap wallMap = mapManager.wallMap;

        // Create arrays
        Vector3Int[] vectors = new Vector3Int[arrSize];
        Tile[] tiles = new Tile[arrSize];
        int i = 0; // arrayIndex

        int halfWidth = (width - RoomType.CORRIDOR_WIDTH)/2; // So it doesnt have to do many divisions
        int halfLength = (length - RoomType.CORRIDOR_WIDTH)/2;

        

        if(north){
            for(int j = 0; j < RoomType.CORRIDOR_WIDTH; j++){
                vectors[i] = new Vector3Int(x + halfWidth + j, y + length);
                tiles[i] = doorTile;
                i++;
            }
        }
        if(south){
            for(int j = 0; j < RoomType.CORRIDOR_WIDTH; j++){
                vectors[i] = new Vector3Int(x + halfWidth + j, y -1);
                tiles[i] = doorTile;
                i++;
            }
        }
        if(east){
            for(int j = 0; j < RoomType.CORRIDOR_WIDTH; j++){
                vectors[i] = new Vector3Int(x + width, y + halfLength + j);
                tiles[i] = doorTile;
                i++;
            }
        }
        if(west){
            for(int j = 0; j < RoomType.CORRIDOR_WIDTH; j++){
                vectors[i] = new Vector3Int(x - 1, y + halfLength + j);
                tiles[i] = doorTile;
                i++;
            }
        }

        // Draw tiles
        wallMap.SetTiles(vectors, tiles);
    }


    public void passCorridorBooleans(bool north, bool east, bool south, bool west){
        this.north = north;
        this.east = east;
        this.south = south;
        this.west = west;
    }
}
