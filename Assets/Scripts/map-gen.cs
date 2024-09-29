using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;
using Unity.VisualScripting;
using Unity.Collections;
using UnityEngineInternal;
using UnityEngine.Tilemaps;



public class MapGen: MonoBehaviour{

    public Tile floorTile;
    public Tile corridorTile;
    public Tile wallTile;
    public Tilemap floorMap;
    public Tilemap wallMap;

    public GameObject rooms;

    public AStar mapGenerator;
    void Start(){
        rooms = new GameObject();
        rooms.name = "Rooms";
        rooms.transform.parent = transform;
        mapGenerator = new AStar(10,10,14);
        MapPainter renderer = new MapPainter(this);
        mapGenerator.createMap();
        renderer.paintNode(mapGenerator.allNodes, mapGenerator.offset[0], mapGenerator.offset[1]);

        // TEMP -> draw other collor at start and endnode
        floorMap.SetTile(new Vector3Int((mapGenerator.startNode.x - mapGenerator.offset[0]) * 40 + 10, (mapGenerator.startNode.y - mapGenerator.offset[1]) * 40 + 10, 0),corridorTile);
        floorMap.SetTile(new Vector3Int((mapGenerator.endNode.x - mapGenerator.offset[0]) * 40 + 10, (mapGenerator.endNode.y - mapGenerator.offset[1]) * 40 + 10, 0),corridorTile);

        GameObject.Find("player").transform.position = new Vector3((mapGenerator.startNode.x - mapGenerator.offset[0]) * 40 + 10 + 10, (mapGenerator.startNode.y - mapGenerator.offset[1]) * 40 + 10 + 10, 0);

    }
}

public class Node
{

    /*
        North = 0
        East = 1
        South = 2
        West = 3  

          0
        3<^>1
          2

    */

    public int x, y; // node coordinates
    public int cost;  // distance from start node / cost
    public int heuristic;  // node heuristic (will be random)
    public Node parent;  // pointer to parent node
    public List<Node> children; // pointer to all children (so each room can handle their own part of the corridor)

    public bool childrenHaveBeenCreated = false;
    public bool isWalkable; // Check if a node has already been designed as not Walkable during the execution of A* algorithm

    public int funcCost => cost + heuristic; // functional cost

    public Node(int x, int y, bool isWalkable)
    {
        this.x = x;
        this.y = y;
        this.isWalkable = isWalkable;
        children = new List<Node>();
        
    }

    private static int[] getDir(int dir){
        int[] res = new int[2];
        switch (dir){
            case 0:
                res[0] = 0; res[1] = 1; break; // North = x+0, y+1
            case 1:
                res[0] = 1; res[1] = 0; break; // East = x+1, y+0
            case 2:
                res[0] = 0; res[1] = -1; break; // South = x+0, y-1
            case 3:
                res[0] = -1; res[1] = 0; break; // West = x-1, y+0
            default:
                break;
        }

        return res;
    }

    private static int invertDir(int dir){
        int res = dir - 2;
        if(dir < 0) dir += 4;
        return dir;
    }

    private List<int[]> avaibleNeightbors(Node[,] grid, int x, int y){
        // Function that return neightbooring spots that are null
        List<int[]> res = new List<int[]>();
        for(int i = 0; i < 4; i++){
            int[] newCoords = new int[2];
            newCoords[0] = x + getDir(i)[0];
            newCoords[1] = y + getDir(i)[1];
            if(grid[newCoords[0], newCoords[1]] == null){
                res.Add(newCoords);
            }
        }
        return res;
    }

    public List<Node> getNeightbors(Node[,] grid){
        // Function that return neightbooring spots that aren't null
        List<Node> res = new List<Node>();
        for(int i = 0; i < 4; i++){
            int[] newCoords = new int[2];
            newCoords[0] = x + getDir(i)[0];
            newCoords[1] = y + getDir(i)[1];
            if(grid[newCoords[0], newCoords[1]] != null){
                res.Add(grid[newCoords[0], newCoords[1]]);
            }
        }
        return res;
    }

    public int createChildren(Node[,] grid, int maxChildren){
        int childrenAmmount = maxChildren; // Get random amm of children
        childrenHaveBeenCreated = true;
        int res = 0; // new children counter
        System.Random rand = new System.Random();

        List<int[]> availableCoords = avaibleNeightbors(grid,x,y);
        int availableCoordsNum = availableCoords.Count;
        for(int i = 0; i < availableCoordsNum && i < maxChildren; i++){
            // Take random coord out of list
            int[] randCoord = availableCoords[rand.Next(0,availableCoords.Count-1)];
            availableCoords.Remove(randCoord);
            // Put new node in random coordinate
            Node newNode = new Node(randCoord[0], randCoord[1],true);
            newNode.parent = this;
            this.children.Add(newNode);
            newNode.children.Add(this); // Add parent to children array of child so it can be used later to iterate trough neighbors
            grid[randCoord[0], randCoord[1]] = newNode;
            res++;
        }
        AStar.printNodeGrid(grid, (int) Mathf.Sqrt(grid.Length),(int) Mathf.Sqrt(grid.Length));
        return res;
    }
}

public class AStar{

    private int width, height;

    public Node startNode;
    public Node endNode;

    private System.Random rand = new System.Random();
    
    private int roomAmmount; 
    private Node[,] grid; // matrix to find if a node already exists fast // Nodes initialized to null

    public List<Node> allNodes;
    public int[] offset = new int[2]; // calculated x and y offset so the map is created always on 0,0 even if its not like that on the generated grid

    public AStar(int width, int height, int roomAmmount)
    {
        this.roomAmmount = roomAmmount;
        this.width = width + roomAmmount * 2 + 2;
        this.height = height + roomAmmount * 2 + 2; // Add borders based on max Room ammt so we dont have to think about managing border/edge cases
        grid = new Node[this.width, this.height];
    }

    public List<Node> createMap(){
        int roomCounter = 0;
        int remainingRooms = roomAmmount;
        // Create startNode in random part of the center map (width - maxRooms * 2)
        int x = rand.Next(roomAmmount + 1, width-roomAmmount);
        int y = rand.Next(roomAmmount + 1, height-roomAmmount);


        List<Node> openSet = new List<Node>();
        allNodes = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        startNode = new Node(x,y,true);
        startNode.heuristic = rand.Next(roomAmmount, 100);
        startNode.cost = 0; // Set total cost from start node to 0
        grid[x,y] = startNode; // Create startNode and add it to the grid
        offset[0] = x; offset[1] = y; // Add start offset. Real one will be calculated later (offset = min distance from 0,0)
        remainingRooms--;

        openSet.Add(grid[x,y]); // Add startNode to the openSet

        if(remainingRooms <= 0){
            Debug.Log("PLEASE SET ROOM AMMOUNT TO AT LEAST TWO");
        }


        // Get open Node with smallest functional Cost (cost from distance + heuristic / predicted cost till end)
        while(openSet.Count > 0){
            Node current = openSet[0];
            for(int i = 1; i<openSet.Count; i++){
                if(openSet[i].funcCost < current.funcCost || openSet[i].funcCost == current.funcCost && openSet[i].heuristic < current.heuristic){
                    current = openSet[i];
                }
            }
            openSet.Remove(current);
            closedSet.Add(current);

            if(!current.childrenHaveBeenCreated){
                int roomCount = rand.Next(2,3);
                if(current == startNode || remainingRooms == 1) roomCount = 1;
                else if(remainingRooms == 2) roomCount = 2;
                roomCounter += current.createChildren(grid,roomCount);
            }

            foreach (Node neightbor in current.getNeightbors(grid)){
                if(neightbor == null || !neightbor.isWalkable || closedSet.Contains(neightbor)) continue;
                neightbor.cost = current.cost; // 100 = distance between nodes
                neightbor.heuristic = rand.Next(10, (remainingRooms)*100);

                if(!openSet.Contains(neightbor)) openSet.Add(neightbor);
            }

            remainingRooms -= roomCounter;
            roomCounter=0;

            if(remainingRooms <= 0){
                // Select random child of current (one of the last rooms created -> only has one entry) as endRoom
                if(current.children.Count > 0){
                    endNode = current.children[rand.Next(0, current.children.Count - 1)];
                }
                else{
                    endNode = current;
                }
                // Add all nodes to allNode list
                foreach(Node node in closedSet){
                    if(node.x < offset[0]) offset[0] = node.x;
                    if(node.y < offset[1]) offset[1] = node.y;
                    allNodes.Add(node);
                }
                foreach(Node node in openSet){
                    if(node.x < offset[0]) offset[0] = node.x;
                    if(node.y < offset[1]) offset[1] = node.y;
                    allNodes.Add(node);
                }
                return allNodes; // HAVE TO CHANGE
            }
        }
        return null;
    }

    public static void printNodeGrid(Node[,] grid, int width, int height){
        Debug.Log("START");
        if(grid == null) Debug.Log("ERRORRRRRRRRRRRRR");
        String matrix = "";
        for(int i = 0; i < width; i++){
            
            for(int j = 0; j < height; j++){
                if(grid[i,j] == null) matrix += (0 + " ");
                else  matrix += (1 + " "); 
            }
            matrix += ("\n");
        }
        Debug.Log(matrix);
    }
}

// class to paint the map recursively starting from endnode
public class MapPainter{

    private MapGen mapGen;
    private System.Random rand;

    public MapPainter(MapGen mapGen){
        this.mapGen = mapGen;
        rand = new System.Random();
    }

    public void paintNode(List<Node> nodes, int xoffset, int yoffset){
        foreach(Node node in nodes){
            int x = (node.x - xoffset) * (RoomType.MAX_ROOM_SIZE + 2*RoomType.NORMAL_CORRIDOR_LENGTH) + RoomType.NORMAL_CORRIDOR_LENGTH;
            int y = (node.y - yoffset) * (RoomType.MAX_ROOM_SIZE + 2*RoomType.NORMAL_CORRIDOR_LENGTH) + RoomType.NORMAL_CORRIDOR_LENGTH;
            // Make gameobject and put it in "rooms"
            GameObject room = new GameObject();
            room.name = "room " + (node.x - xoffset) + "," + (node.y - yoffset); // Put descriptive name
            room.transform.parent = mapGen.rooms.transform;
            Room roomScript = room.AddComponent<Room>();
            roomScript.x = x;
            roomScript.y = y;
            // Decide on room type
            if(node == mapGen.mapGenerator.startNode){
                roomScript.roomType = "StartRoom";
                roomScript.width = RoomType.NORMAL_ROOM_WIDTH;
                roomScript.length = RoomType.NORMAL_ROOM_LENGTH;
                roomScript.corridor_length_h = RoomType.NORMAL_CORRIDOR_LENGTH;
                roomScript.corridor_length_v = RoomType.NORMAL_CORRIDOR_LENGTH;
                roomScript.corridor_width = RoomType.CORRIDOR_WIDTH;
            }
            else if(node == mapGen.mapGenerator.endNode){
                roomScript.roomType = "EndRoom";
                roomScript.width = RoomType.NORMAL_ROOM_WIDTH;
                roomScript.length = RoomType.NORMAL_ROOM_LENGTH;
                roomScript.corridor_length_h = RoomType.NORMAL_CORRIDOR_LENGTH;
                roomScript.corridor_length_v = RoomType.NORMAL_CORRIDOR_LENGTH;
                roomScript.corridor_width = RoomType.CORRIDOR_WIDTH;
            }
            else{
                // Decide random Room type
                // Room types -> Enemy Room, Large Enemy Room, Streched Enemy Room H, Streched Enemy Room V, Chest room, store room, healing room
                // Enemy Room -> 70% -> Normal = 40%, large = 10%, streched h = 10%, streched v = 10%
                // chest room = 15%
                // store room = 10%
                // healing room = 5%
                int roomRandNum = rand.Next(0,100);
                if(roomRandNum <= 40){
                    roomScript.roomType = "NormalEnemyRoom";
                    roomScript.width = RoomType.NORMAL_ROOM_WIDTH;
                    roomScript.length = RoomType.NORMAL_ROOM_LENGTH;
                    roomScript.corridor_length_h = RoomType.NORMAL_CORRIDOR_LENGTH;
                    roomScript.corridor_length_v = RoomType.NORMAL_CORRIDOR_LENGTH;
                    roomScript.corridor_width = RoomType.CORRIDOR_WIDTH;
                }
                else if(roomRandNum <= 50){
                    roomScript.roomType = "LargeEnemyRoom";
                    roomScript.width = RoomType.LARGE_ROOM_WIDTH;
                    roomScript.length = RoomType.LARGE_ROOM_LENGTH;
                    roomScript.corridor_length_h = RoomType.LARGE_ROOM_CORRIDOR_LENGTH;
                    roomScript.corridor_length_v = RoomType.LARGE_ROOM_CORRIDOR_LENGTH;
                    roomScript.corridor_width = RoomType.CORRIDOR_WIDTH;
                    roomScript.x -= 5;
                    roomScript.y -= 5;
                    x-= 5;
                    y-=5;
                }
                else if(roomRandNum <= 60){
                    roomScript.roomType = "StrechedEnemyRoom_H";
                    roomScript.roomType = "LargeEnemyRoom";
                    roomScript.width = RoomType.LARGE_ROOM_WIDTH;
                    roomScript.length = RoomType.NORMAL_ROOM_LENGTH;
                    roomScript.corridor_length_h = RoomType.LARGE_ROOM_CORRIDOR_LENGTH;
                    roomScript.corridor_length_v = RoomType.NORMAL_CORRIDOR_LENGTH;
                    roomScript.corridor_width = RoomType.CORRIDOR_WIDTH;
                    roomScript.x -= 5;
                    x-= 5;
                }
                else if(roomRandNum <= 70){
                    roomScript.roomType = "StrechedEnemyRoom_V";
                    roomScript.roomType = "LargeEnemyRoom";
                    roomScript.width = RoomType.NORMAL_ROOM_WIDTH;
                    roomScript.length = RoomType.LARGE_ROOM_LENGTH;
                    roomScript.corridor_length_h = RoomType.NORMAL_CORRIDOR_LENGTH;
                    roomScript.corridor_length_v = RoomType.LARGE_ROOM_CORRIDOR_LENGTH;
                    roomScript.corridor_width = RoomType.CORRIDOR_WIDTH;
                    roomScript.y -= 5;
                    y-=5;
                }
                // Chest Room
                else if(roomRandNum <= 85){
                    roomScript.roomType = "ChestRoom";
                    roomScript.width = RoomType.NORMAL_ROOM_WIDTH;
                    roomScript.length = RoomType.NORMAL_ROOM_LENGTH;
                    roomScript.corridor_length_h = RoomType.NORMAL_CORRIDOR_LENGTH;
                    roomScript.corridor_length_v = RoomType.NORMAL_CORRIDOR_LENGTH;
                    roomScript.corridor_width = RoomType.CORRIDOR_WIDTH;
                }
                // Store room
                else if(roomRandNum <= 95){
                    roomScript.roomType = "StoreRoom";
                    roomScript.width = RoomType.NORMAL_ROOM_WIDTH;
                    roomScript.length = RoomType.NORMAL_ROOM_LENGTH;
                    roomScript.corridor_length_h = RoomType.NORMAL_CORRIDOR_LENGTH;
                    roomScript.corridor_length_v = RoomType.NORMAL_CORRIDOR_LENGTH;
                    roomScript.corridor_width = RoomType.CORRIDOR_WIDTH;
                }
                // Healing room
                else if(roomRandNum <= 100){
                    roomScript.roomType = "HealingRoom";
                    roomScript.width = RoomType.NORMAL_ROOM_WIDTH;
                    roomScript.length = RoomType.NORMAL_ROOM_LENGTH;
                    roomScript.corridor_length_h = RoomType.NORMAL_CORRIDOR_LENGTH;
                    roomScript.corridor_length_v = RoomType.NORMAL_CORRIDOR_LENGTH;
                    roomScript.corridor_width = RoomType.CORRIDOR_WIDTH;
                }
            }

            roomScript.createCollider(); // Create room collider after adding parameters
            // Draw floors
            for(int i = 0; i < roomScript.width; i++){
                for(int j = 0; j < roomScript.length; j++){
                    mapGen.floorMap.SetTile(new Vector3Int(x + i,y + j,0), mapGen.floorTile);
                }
            }

            // Set corridor booleans in roomScript
            setCorridorBooleans(node, roomScript);
            
            // Draw corridors
            drawCorridors(x,y,roomScript);

            // Draw walls
            drawWalls(x,y,roomScript);
        }
    }

    private void drawCorridors(int x, int y, Room roomScript){
        if(roomScript.north){
            for(int i = 0; i < roomScript.corridor_width; i++){
                for(int j = 0; j < roomScript.corridor_length_v; j++){
                    mapGen.floorMap.SetTile(new Vector3Int(x + (roomScript.width - roomScript.corridor_width)/2 + i,y + roomScript.length + j,0), mapGen.corridorTile);
                }
            }
        }
        if(roomScript.south){
            for(int i = 0; i < roomScript.corridor_width; i++){
                for(int j = 0; j < roomScript.corridor_length_v; j++){
                    mapGen.floorMap.SetTile(new Vector3Int(x + (roomScript.width - roomScript.corridor_width)/2 + i,y - 1 - j,0), mapGen.corridorTile);
                }
            } 
        }
        if(roomScript.east){
            for(int i = 0; i < roomScript.corridor_length_h; i++){
                for(int j = 0; j < roomScript.corridor_width; j++){
                    mapGen.floorMap.SetTile(new Vector3Int(x + roomScript.width + i,y + (roomScript.length - roomScript.corridor_width)/2 + j,0), mapGen.corridorTile);
                }
            } 
        }
        if(roomScript.west){
            for(int i = 0; i < roomScript.corridor_length_h; i++){
                for(int j = 0; j < roomScript.corridor_width; j++){
                    mapGen.floorMap.SetTile(new Vector3Int(x - 1 - i,y + (roomScript.length - roomScript.corridor_width)/2 + j,0), mapGen.corridorTile);
                }
            } 
        }
    }

    private void drawWalls(int x, int y, Room roomScript){
        if(roomScript.north){ // If it has a corridor on the north -> Dont draw walls where the hallway will pass trough (leave an empty space)
            // room northern wall with space
            for(int i = -1; i < (roomScript.width - roomScript.corridor_width)/2; i++){
                mapGen.wallMap.SetTile(new Vector3Int(x + i, y + roomScript.length, 0), mapGen.wallTile);
            }
            for(int i = (roomScript.width - roomScript.corridor_width)/2 + roomScript.corridor_width; i < roomScript.width + 1; i++){
                mapGen.wallMap.SetTile(new Vector3Int(x + i, y + roomScript.length, 0), mapGen.wallTile);
            }
            // corridor walls
            for(int i = 0; i < roomScript.corridor_length_v; i++){
                mapGen.wallMap.SetTile(new Vector3Int(x + (roomScript.width - roomScript.corridor_width)/2 - 1, y + roomScript.length + i, 0), mapGen.wallTile);
                mapGen.wallMap.SetTile(new Vector3Int(x + (roomScript.width - roomScript.corridor_width)/2 + roomScript.corridor_width, y + roomScript.length + i, 0), mapGen.wallTile);
            }
        }
        else{
            for(int i = -1; i < roomScript.width + 1; i++){
                mapGen.wallMap.SetTile(new Vector3Int(x + i, y + roomScript.length, 0), mapGen.wallTile);
            }
        }
        if(roomScript.south){ // If it has a corridor on the north -> Dont draw walls where the hallway will pass trough (leave an empty space)
            for(int i = -1; i < (roomScript.width - roomScript.corridor_width)/2; i++){
                mapGen.wallMap.SetTile(new Vector3Int(x + i, y - 1, 0), mapGen.wallTile);
            }
            for(int i = (roomScript.width - roomScript.corridor_width)/2 + roomScript.corridor_width; i < roomScript.width + 1; i++){
                mapGen.wallMap.SetTile(new Vector3Int(x + i, y - 1, 0), mapGen.wallTile);
            }
            for(int i = 1; i < roomScript.corridor_length_v + 1; i++){
                mapGen.wallMap.SetTile(new Vector3Int(x + (roomScript.width - roomScript.corridor_width)/2 - 1, y - i, 0), mapGen.wallTile);
                mapGen.wallMap.SetTile(new Vector3Int(x + (roomScript.width - roomScript.corridor_width)/2 + roomScript.corridor_width, y - i, 0), mapGen.wallTile);
            }
        }
        else{
            for(int i = -1; i < roomScript.width + 1; i++){
                mapGen.wallMap.SetTile(new Vector3Int(x + i, y - 1, 0), mapGen.wallTile);
            }
        }
        if(roomScript.east){ // If it has a corridor on the north -> Dont draw walls where the hallway will pass trough (leave an empty space)
            for(int i = -1; i < (roomScript.length - roomScript.corridor_width)/2; i++){
                mapGen.wallMap.SetTile(new Vector3Int(x+roomScript.width, y + i, 0), mapGen.wallTile);
            }
            for(int i = (roomScript.length - roomScript.corridor_width)/2 + roomScript.corridor_width; i < roomScript.length + 1; i++){
                mapGen.wallMap.SetTile(new Vector3Int(x+roomScript.width, y + i, 0), mapGen.wallTile);
            }
            for(int i = 1; i < roomScript.corridor_length_h + 1; i++){
                mapGen.wallMap.SetTile(new Vector3Int(x + roomScript.width + i, y + (roomScript.length - roomScript.corridor_width)/2 -1, 0), mapGen.wallTile);
                mapGen.wallMap.SetTile(new Vector3Int(x + roomScript.width + i, y + (roomScript.length - roomScript.corridor_width)/2 + roomScript.corridor_width, 0), mapGen.wallTile);
            }
        }
        else{
            for(int i = -1; i < roomScript.length + 1; i++){
                mapGen.wallMap.SetTile(new Vector3Int(x+roomScript.width, y + i, 0), mapGen.wallTile);
            }
        }
        if(roomScript.west){ // If it has a corridor on the north -> Dont draw walls where the hallway will pass trough (leave an empty space)
            for(int i = -1; i < (roomScript.length - roomScript.corridor_width)/2; i++){
                mapGen.wallMap.SetTile(new Vector3Int(x-1, y + i, 0), mapGen.wallTile);
            }
            for(int i = (roomScript.length - roomScript.corridor_width)/2 + roomScript.corridor_width; i < roomScript.length + 1; i++){
                mapGen.wallMap.SetTile(new Vector3Int(x-1, y + i, 0), mapGen.wallTile);
            }
            for(int i = 1; i < roomScript.corridor_length_h; i++){
                mapGen.wallMap.SetTile(new Vector3Int(x - i, y + (roomScript.length - roomScript.corridor_width)/2 -1, 0), mapGen.wallTile);
                mapGen.wallMap.SetTile(new Vector3Int(x - i, y + (roomScript.length - roomScript.corridor_width)/2 + roomScript.corridor_width, 0), mapGen.wallTile);
            }
        }
        else{
            for(int i = -1; i < roomScript.length + 1; i++){
                mapGen.wallMap.SetTile(new Vector3Int(x-1, y + i, 0), mapGen.wallTile);
            }
        }
    }

    private String getDirection(int x, int y){
        Debug.Log(x + " - " + y);
        if(x == -1){ // Current node is left of parent node -> draw to the right
            return "East";
        }
        else if( x == 1){
            return "West";
        }
        else if(y == -1){ // Current node is bellow parent node -> drow north
            return "North";
        }
        else if(y == 1){
            return "South";
        }
        else{
            Debug.Log("INVALID DIRECTION TO PARENT -> " + x + "," + y);
            return null;
        }
    }

    private void setCorridorBooleans(Node node, Room roomScript){
        // Set corridors for children
        foreach(Node childNode in node.children){
            String dir = getDirection(node.x - childNode.x, node.y - childNode.y);
            switch(dir){
                case "North": roomScript.north = true; break;
                case "East": roomScript.east = true; break;
                case "South": roomScript.south = true; break;
                case "West": roomScript.west = true; break;
                default: break;
            }
        }
    }
}

public static class RoomType{
    public const int MAX_ROOM_SIZE = 20;
    public const int NORMAL_CORRIDOR_LENGTH = 10;
    public const int LARGE_ROOM_CORRIDOR_LENGTH = 5;
    public const int CORRIDOR_WIDTH = 4;
    public const int NORMAL_ROOM_WIDTH = 20;
    public const int NORMAL_ROOM_LENGTH = 20;

    public const int LARGE_ROOM_WIDTH = 30;
    public const int LARGE_ROOM_LENGTH = 30;
}


