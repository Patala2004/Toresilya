using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Rendering.Universal;
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

    public int roomX; // x coordinates but not based on real coordinates but on room index on the room matrix
    public int roomY;

    public int roomType;


    public Node[,] nodeMatrix;


    private MapGen mapManager;

    public bool spawned = false;
    public int aliveEnemies = 0;

    private GameObject minimapCamera;
    private GameObject minimapCameraWide;

    public BoxCollider2D roomCollider;

    private Player player;
    // Start is called before the first frame update
    void Start()
    {
        mapManager = GameObject.Find("MapGenerator").GetComponent<MapGen>();
        minimapCamera = GameObject.Find("MiniMapCamera");
        minimapCameraWide = GameObject.Find("MiniMapCameraWide");
        player = GameObject.Find("player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.name != "player"){
            return;
        }
        // If collision was with player and the room is an EnemyRoom
        if( roomType > 4 && !spawned){
            spawned = true;
            CloseCorridors();            
        }   
        // Crear habitacion de minimapa
        createMiniMapRoom();     

        // Llamar a funciones de entrada a habitacion de player
        other.gameObject.GetComponent<Player>().OnRoomEnter();
    }
    public void CommunicateEnemyDeath(){
        aliveEnemies--;
        if(aliveEnemies == 0){
            RoomCleared();
        }
    }

    private void RoomCleared(){
        OpenCorridors();
        int totalCoinVal = UnityEngine.Random.Range(10,30);
        GameObject coinPrefab = Resources.Load<GameObject>("Mapa/MapGen/Prefabs/Coin");

        // Divide val into gold coins (20), silver coins (5) and bronze coins (1)
        int goldC = 0;
        int silvC = 0;
        int bronzeC = 0;
        while(totalCoinVal - 20 >= 0){
            goldC++;
            totalCoinVal -= 20;
        }
        while(totalCoinVal - 5 >= 0){
            silvC++;
            totalCoinVal-=5;
        }
        bronzeC = totalCoinVal;

        GameObject coin;
        for(;goldC > 0; goldC--){
            // Spawn gold coins (coin with value 20)
            coin = Instantiate(coinPrefab);
            coin.transform.parent = gameObject.transform;
            coin.transform.localPosition = new Vector2(0,0) + new Vector2(UnityEngine.Random.Range(0f,1f),UnityEngine.Random.Range(0f,1f));
            coin.GetComponent<Coin>().value = 20;
            coin.GetComponent<Light2D>().color = new Color32(255,223,0,255);
        }
        for(;silvC > 0; silvC--){
            // Spawn silver coins (coin with value 5)
            coin = Instantiate(coinPrefab);
            coin.transform.parent = gameObject.transform;
            coin.transform.localPosition = new Vector2(0,0) + new Vector2(UnityEngine.Random.Range(0f,1f),UnityEngine.Random.Range(0f,1f));
            coin.GetComponent<Coin>().value = 5;
            coin.GetComponent<Light2D>().color = new Color32(192,192,192,255);
        }
        for(;bronzeC > 0; bronzeC--){
            // Spawn bronze coins (coin with value 1)
            coin = Instantiate(coinPrefab);
            coin.transform.parent = gameObject.transform;
            coin.transform.localPosition = new Vector2(0,0) + new Vector2(UnityEngine.Random.Range(0f,1f),UnityEngine.Random.Range(0f,1f));
            coin.GetComponent<Coin>().value = 1;
            coin.GetComponent<Light2D>().color = new Color32(205,127,50,255);
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

     private void OpenCorridors(){
        int arrSize = 0;
        if(north) arrSize+= RoomType.CORRIDOR_WIDTH;
        if(east) arrSize+= RoomType.CORRIDOR_WIDTH;
        if(south) arrSize+= RoomType.CORRIDOR_WIDTH;
        if(west) arrSize+= RoomType.CORRIDOR_WIDTH;
        Tile doorTile = null;
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

    private void createMiniMapRoom(){
        GameObject miniMapFolder = GameObject.Find("MiniMap");
        GameObject minimapRoom = Instantiate(Resources.Load<GameObject>("Mapa/Minimap/MiniMap_Room"));
        minimapRoom.transform.position = new Vector2(roomX*3 - 1000,roomY*3 - 1000);
        // hide not existing corridors
        minimapRoom.transform.GetChild(1).gameObject.SetActive(north); // north
        minimapRoom.transform.GetChild(2).gameObject.SetActive(east); // east
        minimapRoom.transform.GetChild(3).gameObject.SetActive(south); // south
        minimapRoom.transform.GetChild(4).gameObject.SetActive(west); // west

        minimapRoom.transform.parent = miniMapFolder.transform;

        minimapCamera.transform.position = new Vector3(roomX*3 - 1000,roomY*3 - 1000, -10);
        minimapCameraWide.transform.position = new Vector3(roomX*3 - 1000,roomY*3 - 1000, -40);

        // Add symbols
        if(roomType == 0){
            minimapRoom.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Mapa/Minimap/Symbol_Sprites/casita");
        }
        // Add symbols for neightboring rooms
        if(north){
            // Find northern room script
            Room otherRoom = GameObject.Find("room " + roomX + "," + (roomY + 1)).GetComponent<Room>();
            Sprite sprite = null;
            if(otherRoom.roomType == 2 || otherRoom.roomType == 3 || otherRoom.roomType == 4){
                sprite = Resources.Load<Sprite>("Mapa/Minimap/Symbol_Sprites/cajita");
                minimapRoom.transform.GetChild(1).GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
            }
            else if(otherRoom.roomType == 1){
                sprite = Resources.Load<Sprite>("Mapa/Minimap/Symbol_Sprites/portalito");
                minimapRoom.transform.GetChild(1).GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
            }
        }
        if(east){
            Room otherRoom = GameObject.Find("room " + (roomX+1) + "," + roomY).GetComponent<Room>();
            Sprite sprite = null;
            if(otherRoom.roomType == 2 || otherRoom.roomType == 3 || otherRoom.roomType == 4){
                sprite = Resources.Load<Sprite>("Mapa/Minimap/Symbol_Sprites/cajita");
                minimapRoom.transform.GetChild(2).GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
            }
            else if(otherRoom.roomType == 1){
                sprite = Resources.Load<Sprite>("Mapa/Minimap/Symbol_Sprites/portalito");
                minimapRoom.transform.GetChild(2).GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
            }
        }
        if(south){
            Room otherRoom = GameObject.Find("room " + roomX + "," + (roomY - 1)).GetComponent<Room>();
            Sprite sprite = null;
            if(otherRoom.roomType == 2 || otherRoom.roomType == 3 || otherRoom.roomType == 4){
                sprite = Resources.Load<Sprite>("Mapa/Minimap/Symbol_Sprites/cajita");
                minimapRoom.transform.GetChild(3).GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
            }
            else if(otherRoom.roomType == 1){
                sprite = Resources.Load<Sprite>("Mapa/Minimap/Symbol_Sprites/portalito");
                minimapRoom.transform.GetChild(3).GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
            }
        }
        if(west){
            Room otherRoom = GameObject.Find("room " + (roomX-1) + "," + roomY).GetComponent<Room>();
            Sprite sprite = null;
            if(otherRoom.roomType == 2 || otherRoom.roomType == 3 || otherRoom.roomType == 4){
                sprite = Resources.Load<Sprite>("Mapa/Minimap/Symbol_Sprites/cajita");
                minimapRoom.transform.GetChild(4).GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
            }
            else if(otherRoom.roomType == 1){
                sprite = Resources.Load<Sprite>("Mapa/Minimap/Symbol_Sprites/portalito");
                minimapRoom.transform.GetChild(4).GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
            }
        }
    }


    public void createMatrix(int width, int lenght, GameObject roomPrefab){
        if(roomPrefab == null){
            return;
        }

        nodeMatrix = new Node[width,lenght];
        for(int i = 0; i < width; i++){
            for(int j = 0; j < length; j++){
                Node newNode = new Node(i,j);
                nodeMatrix[i,j] = newNode;
                if(i > 0){
                    nodeMatrix[i-1,j].east = newNode;
                    newNode.west = nodeMatrix[i-1,j];
                }
                if(j > 0){
                    nodeMatrix[i,j-1].south = newNode;
                    newNode.north = nodeMatrix[i,j-1];
                }
                if(i > 0 && j > 0){
                    nodeMatrix[i-1,j-1].southEast = newNode;
                    newNode.northWest = nodeMatrix[i-1,j-1];
                }
                if(i > 0 && j < length-1){
                    nodeMatrix[i-1,j+1].southWest = newNode;
                    newNode.northEast = nodeMatrix[i-1,j+1];
                }
            }
        }

        foreach(Transform obstacle in roomPrefab.transform){
           invalidateChildren(obstacle.gameObject);
        }

        /*
        String a = "";
        for(int i = 0; i < width; i++){
            for(int j = 0; j < length; j++){
                a += (isInvalidated(nodeMatrix[i,j], nodeMatrix)? 8:0) + " ";
            }
            a += '\n';
        }
        */
        
        
    }

    private void invalidateChildren(GameObject go){
        foreach(Transform child in go.transform){
            if(child.transform.childCount > 0){
                invalidateChildren(child.gameObject);
            }
            else{
                int x = (int)(child.position.x - 0.5f - this.x);
                int y =  (int)(child.position.y - 0.5f - this.y);
                nodeMatrix[x,y].isWalkable = false;
                invalidateNodeNeightboors(nodeMatrix[x,y], nodeMatrix);
            }
        }
    }

    public static void invalidateNodeNeightboors(Node node, Node[,] matrix){
        int i = node.x;
        int j = node.y;
        int width = matrix.GetLength(0);
        int length = matrix.GetLength(1);
        if(i > 0){ // west
            matrix[i-1,j].east = null;
        }
        if(j > 0){ // north
            matrix[i,j-1].south = null;
        }
        if(i > 0 && j > 0){ // northwest
            matrix[i-1,j-1].southEast = null;
        }
        if(j > 0 && i < width-1){ // northeast
            matrix[i+1,j-1].southWest = null;
        }
        if(i > 0 && j < length-1){ // southwest
            matrix[i-1,j+1].northEast = null;
        }
        if(i < width-1){ // east
            matrix[i+1,j].west = null;
        }
        if(j < length-1){ // south
            matrix[i,j+1].north = null;
        }
        if(i < width-1 && j < length-1){ // southeast
            matrix[i+1,j+1].northWest = null;
        }
    }

    private static bool isInvalidated(Node node, Node[,] matrix){
        // Used only for debugging for correct node invalidation / validation
        int i = node.x;
        int j = node.y;
        int width = matrix.GetLength(0);
        int length = matrix.GetLength(1);
        bool result = true;
        if(i > 0){ // west
            result = result && matrix[i-1,j].east == null;
        }
        if(j > 0){ // north
            result = result && matrix[i,j-1].south == null;
        }
        if(i > 0 && j > 0){ // northwest
            result = result && matrix[i-1,j-1].southEast == null;
        }
        if(j > 0 && i < width-1){ // northeast
            result = result && matrix[i+1,j-1].southWest == null;
        }
        if(i > 0 && j < length-1){ // southwest
            result = result && matrix[i-1,j+1].northEast == null;
        }
        if(i < width-1){ // east
            result = result && matrix[i+1,j].west == null;
        }
        if(j < length-1){ // south
            result = result && matrix[i,j+1].north == null;
        }
        if(i < width-1 && j < length-1){ // southeast
            result = result && matrix[i+1,j+1].northWest == null;
        }
        return result;
    }

    public static void validateNodeNeightboors(Node node, Node[,] matrix){
        // This function will be called to update the Node of a recently broken obstacle / a now walkable tile
        int i = node.x;
        int j = node.y;
        matrix[i,j].isWalkable = true;
        int width = matrix.GetLength(0);
        int length = matrix.Length;
        if(i > 0){ // west
            matrix[i-1,j].east = matrix[i,j];
        }
        if(j > 0){ // north
            matrix[i,j-1].south = matrix[i,j];
        }
        if(i > 0 && j > 0){ // northwest
            matrix[i-1,j-1].southEast = matrix[i,j];
        }
        if(j > 0 && i < width){ // northeast
            matrix[i+1,j-1].southWest = matrix[i,j];
        }
        if(i > 0 && j < length){ // southwest
            matrix[i-1,j+1].northEast = matrix[i,j];
        }
        if(i < width){ // east
            matrix[i+1,j].west = matrix[i,j];
        }
        if(j < length){ // south
            matrix[i,j+1].north = matrix[i,j];
        }
        if(i < width && j < length){ // southeast
            matrix[i+1,j+1].northWest = matrix[i,j];
        }
    }
}


public class Node{
    private float root2 = (float) Math.Sqrt(2);
    public bool isWalkable = true;
    public int x,y;
    public Node north;
    public Node northEast;
    public Node east;
    public Node southEast;
    public Node south;
    public Node southWest;
    public Node west;
    public Node northWest;

    public Node(int x, int y){
        this.x = x;
        this.y = y;
        north = null;
        northEast = null;
        east = null;
        southEast = null;
        south = null;
        southWest = null;
        west = null;
        northWest = null;
    }

    public float getCost(Node node){
        if(node == north || node == south || node == east || node == west){
            return 1;
        }
        else if(node == northWest || node == southWest || node == northEast || node == southEast){
            return root2;
        }
        else{
            throw new Exception("node is not a neightboor. Cannot calculate direct walking cost");
        }
    }

    public float getHeuristic(float playerX, float playerY){
        // Pythagoras
        return (float) Math.Sqrt(Math.Pow(playerX-x,2) + Math.Pow(playerY-y,2));
    }
}
