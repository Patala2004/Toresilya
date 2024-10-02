using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;
using Unity.VisualScripting;
using Unity.Collections;
using UnityEngineInternal;
using UnityEngine.Tilemaps;
using Unity.VisualScripting.FullSerializer;
using System.Data;
using System.Linq;
using UnityEditor.Tilemaps;



public class MapGen: MonoBehaviour{

    public Tile floorTile;
    public Tile corridorTile;
    public Tile wallTile;
    public Tilemap floorMap;
    public Tilemap wallMap;
    public GameObject box; // Box prefab

    public GameObject rooms;

    public AStarStruct mapGenerator;
    void Start(){
        rooms = new GameObject();
        rooms.name = "Rooms";
        rooms.transform.parent = transform;
        StartCoroutine(generateMap());
    }

    IEnumerator generateMap(){
        //mapGenerator = new AStar(10,10,14);
        System.Random seedGen = new System.Random();
        Unity.Mathematics.Random rand = new Unity.Mathematics.Random((uint) seedGen.Next(1,999999999));
        int gridSize = 31;
        NativeArray<NodeStruct> girdStruct = new NativeArray<NodeStruct>(gridSize*gridSize, Allocator.Persistent);
        NativeList<NodeStruct> openSet = new NativeList<NodeStruct>(Allocator.Persistent);
        NativeList<NodeStruct> allNodes = new NativeList<NodeStruct>(Allocator.Persistent);
        NativeHashSet<NodeStruct> closedSet = new NativeHashSet<NodeStruct>(100000, Allocator.Persistent);
        NativeArray<bool> booleanArray_s4 = new NativeArray<bool>(4, Allocator.Persistent);
        NativeList<NodeStruct> nativeListNodeStruc = new NativeList<NodeStruct>(Allocator.Persistent);
        AStarStruct mapGenerator2 = new AStarStruct(gridSize,rand,14, girdStruct,allNodes, openSet, closedSet, booleanArray_s4, nativeListNodeStruc, new NodeStruct());
        MapPainter renderer = new MapPainter(this, girdStruct, gridSize);
        mapGenerator2.createMap();

        Debug.Log("ENDNODE " + mapGenerator2.endNode.x + " " + mapGenerator2.endNode.y);
        Debug.Log("START " + mapGenerator2.startNode.x + " " + mapGenerator2.startNode.y);
        String a = "";
        // print grid of mapGenerator2
        Debug.Log(mapGenerator2.allNodes.Length);

        for(int i = 0; i < girdStruct.Length; i++){
            a += girdStruct[i].hasBeenCreated? 1 : 0 + " ";
            if(i%gridSize == 0) a+="\n";

        }
        Debug.Log(a);
        renderer.paintNode(mapGenerator2.allNodes, mapGenerator2.xoffset, mapGenerator2.yoffset, nativeListNodeStruc);

        girdStruct.Dispose();
        openSet.Dispose();
        allNodes.Dispose();
        closedSet.Dispose();
        booleanArray_s4.Dispose();
        nativeListNodeStruc.Dispose();


        // TEMP -> draw other collor at start and endnode
        floorMap.SetTile(new Vector3Int((mapGenerator2.startNode.x - mapGenerator2.xoffset) * 40 + 10, (mapGenerator2.startNode.y - mapGenerator2.yoffset) * 40 + 10, 0),corridorTile);
        floorMap.SetTile(new Vector3Int((mapGenerator2.endNode.x - mapGenerator2.xoffset) * 40 + 10, (mapGenerator2.endNode.y - mapGenerator2.yoffset) * 40 + 10, 0),corridorTile);

        //GameObject.Find("player").transform.position = new Vector3((mapGenerator.startNode.x - mapGenerator.offset[0]) * 40 + 10 + 10, (mapGenerator.startNode.y - mapGenerator.offset[1]) * 40 + 10 + 10, 0);

        yield return null;
    }
}

public struct NodeStruct : IEquatable<NodeStruct>{
    public int x, y;

    int parentdir; // 0 north ... 3 west
    public int cost;
    public int heuristic;
    public int funcCost => cost + heuristic;
    public bool hasBeenCreated;
    public bool childrenHaveBeenCreated;
    public bool isWalkable;
    public bool north;
    public bool east;
    public bool south;
    public bool west;

    public bool Equals(NodeStruct other){
        return this.x == other.x && this.y == other.y;
    }
    public override int GetHashCode()
    {
        return x.GetHashCode() ^ y.GetHashCode();
    }

    public override bool Equals(object other)
    {
        if(!(other is NodeStruct)) return false;
        NodeStruct other2 = (NodeStruct) other;
        return this.x == other2.x && this.y == other2.y;
    }

    // overload == 
    public static bool operator ==(NodeStruct a, NodeStruct b){
        return a.x == b.x && a.y == b.y;
    }
    public static bool operator !=(NodeStruct a, NodeStruct b){
        return !(a.x == b.x && a.y == b.y);
    }

    public void initialize(int x, int y){
        this.x = x;
        this.y = y;
        cost = 0;
        heuristic = 0;
        childrenHaveBeenCreated = false;
        isWalkable = true;
        hasBeenCreated = true;
        north = false;
        east = false;
        south = false;
        west = false;
        parentdir = -1;
    }

    public NativeList<NodeStruct> neighbors(NativeArray<NodeStruct> grid, int grid_size, NativeList<NodeStruct> res){
        // returns existing neightboring nodes
        if(west && parentdir != 3) res.Add(grid[(x-1) * grid_size +  y]);
        if(south && parentdir != 2) res.Add(grid[x*grid_size + y-1]);
        if(east && parentdir != 1) res.Add(grid[(x+1)*grid_size + y]);
        if(north && parentdir != 0) res.Add(grid[x*grid_size + y+1]);
        return res;
    }

    // Returns a boolean array indicating which of the 4 directions has an available space 
    public NativeArray<bool> availableNeighbors(NativeArray<NodeStruct> grid, int grid_size, NativeArray<bool> res){
        if(!grid[x*grid_size + y+1].hasBeenCreated) res[0] = true; // North
        if(!grid[(x+1)*grid_size + y].hasBeenCreated) res[1] = true; // East
        if(!grid[x*grid_size + y-1].hasBeenCreated) {res[2] = true;} // South
        if(!grid[(x-1)*grid_size + y].hasBeenCreated) res[3] = true; // West
        return res;
    }

    public int createChildren(NativeArray<NodeStruct> grid, int grid_size, int maxChildren, Unity.Mathematics.Random rand, NativeArray<bool> booleanArray_s4){
        childrenHaveBeenCreated = true;
        int res = 0; // created child counter
        // clean boolean array
        for(int i = 0; i < 4; i++){
            booleanArray_s4[i] = false;
        }
        NativeArray<bool> availableNeighbors = this.availableNeighbors(grid, grid_size, booleanArray_s4);
        int availableNeighborsCounter = 0;
        int remaindingChildren = maxChildren;
        for(int i = 0; i < 4; i++){ // Count neighbors
            if(availableNeighbors[i]) availableNeighborsCounter++;
        }




        // AQUI OCURRE ERROR


        // ABAJO


        // HAY QUE HACER QUE NO PUEDA ENTRAR EN BUCLE INFINITO



        // NO ME SORPRENDE; MENUDA MIERDA DE CODIGO QUE ES ESTO







        int ii = 0;
        while(availableNeighborsCounter > 0 && remaindingChildren > 0){
            ii++;
            if(ii > 10){Debug.Log("ERROR C"); return 0;}
            int i = rand.NextInt(0,3); // Random direction
            if(availableNeighbors[i]){
                // Update counters and values
                availableNeighbors[i]=false;
                availableNeighborsCounter--; 
                remaindingChildren--;
                res++;
                // Create new child
                if(i == 0){ // Create child in the north
                    north = true;
                    NodeStruct a = grid[x*grid_size + y+1];
                    a.initialize(x, y+1);
                    a.south = true;
                    a.parentdir = 2; // south
                    grid[x*grid_size + y+1] = a;
                }
                if(i == 1){ // Create child in the east
                    east = true;
                    NodeStruct a = grid[(x+1)*grid_size + y];
                    a.initialize(x+1,y);
                    a.west = true;
                    a.parentdir = 3; 
                    grid[(x+1)*grid_size + y] = a;
                }
                if(i == 2){ // Create child in the south
                    south = true;
                    NodeStruct a = grid[x*grid_size + y-1];
                    a.initialize(x,y-1);
                    a.north = true;
                    a.parentdir = 0;
                    grid[x*grid_size + y - 1] = a;
                }
                if(i == 3){ // Create child in the west a
                    west = true;
                    NodeStruct a = grid[(x-1)*grid_size +y];
                    a.initialize(x-1,y);
                    a.east = true;
                    a.parentdir = 1;
                    grid[(x-1)*grid_size +y] = a;
                }
            }
        }
        return res;
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

public struct AStarStruct{
    private int grid_size;
    public NodeStruct startNode;
    public NodeStruct endNode;

    private Unity.Mathematics.Random rand;

    private int roomAmmount;

    private NativeArray<NodeStruct> grid;

    public NativeList<NodeStruct> allNodes; // Closed set but not yet closed
    public NativeList<NodeStruct> openSet; // Auxiliary list for search
    public NativeHashSet<NodeStruct> closedSet;
    public NativeArray<bool> booleanArray_s4;
    private NativeList<NodeStruct> nodeStructList;

    public int xoffset;
    public int yoffset;

    public AStarStruct(int grid_size, Unity.Mathematics.Random rand, int roomAmmount, NativeArray<NodeStruct> grid, NativeList<NodeStruct> allNodes, NativeList<NodeStruct> openSet, 
    NativeHashSet<NodeStruct> closedSet, NativeArray<bool> booleanArray_s4, NativeList<NodeStruct> nodeStructList, NodeStruct tempStruc){
        this.grid_size = grid_size;
        this.rand = rand;
        this.roomAmmount = roomAmmount;
        this.grid = grid;
        this.allNodes = allNodes;
        this.openSet = openSet;
        this.closedSet = closedSet;
        this.booleanArray_s4 = booleanArray_s4;
        this.nodeStructList = nodeStructList;
        xoffset = 0;
        yoffset = 0;
        startNode = tempStruc; // Creates uninitialized temporal nodeStructs, which will then be replazed by the real startNode
        endNode = tempStruc;
    }



    // ADD CHECK SO YOU CANT PUT LESS THAN 2 ROOMS AS MAXROOMS
    public void createMap(){
        int roomCounter = 0;
        int remainingRooms = roomAmmount;
        // Create start node in random part of the center map (width - maxRooms * 2)
        int x = rand.NextInt(roomAmmount + 1, grid_size - roomAmmount);
        int y = rand.NextInt(roomAmmount + 1, grid_size - roomAmmount);

        startNode = grid[x*grid_size + y]; // STRUCTS ARE PASSED BY VALUE NOT POINTER -> MODIFYING THIS DOESNT MODIFY THE VALUE INSIDE OF THE ARRAY
        startNode.initialize(x,y);
        startNode.heuristic = rand.NextInt(roomAmmount, 10000);
        startNode.cost = 0;
        grid[x*grid_size + y] = startNode;
        xoffset = x;
        yoffset = y;
        remainingRooms--;

        openSet.Add(startNode);

        
        int ii = 0;
        // Get open Node with smallest functional Cost (costs + heuristic)
        while(openSet.Length > 0){
            ii++;
            if(ii > 17){Debug.Log("ERROR B"); return;}
            NodeStruct current = openSet[0];
            int currentOpenSetIndex = 0;
            for(int i = 0; i < openSet.Length; i++){
                if(openSet[i].funcCost < current.funcCost || openSet[i].funcCost == current.funcCost && openSet[i].heuristic < current.heuristic){
                    current = openSet[i];
                    currentOpenSetIndex = i;
                }
            }
            // current is now lowest cost node
            openSet.RemoveAt(currentOpenSetIndex);
            //closedSet.Add(current); // Have to add at the end

            if(!current.childrenHaveBeenCreated){
                int roomCount = rand.NextInt(2,3);
                if(current == startNode || remainingRooms == 1) roomCount = 1;
                else if(remainingRooms == 2) roomCount = 2;
                roomCounter+= current.createChildren(grid, grid_size, roomCount, rand, booleanArray_s4);
            }
            // Empty nodeStructList
            int iii = 0;
            while(nodeStructList.Length > 0){
                nodeStructList.RemoveAt(0);
                if(iii > 10){Debug.Log("ERROR A"); return;}
                iii++;
            }

            NativeList<NodeStruct> neighborList = current.neighbors(grid, grid_size, nodeStructList);
            // Cannot use foeach because foreach iterations cannot be modified
            for(int i = 0; i < neighborList.Length; i++){
                NodeStruct neighbor = neighborList[i];
                if(!neighbor.hasBeenCreated || !neighbor.isWalkable || closedSet.Contains(neighbor)) continue;
                neighbor.cost = current.cost;
                neighbor.heuristic = rand.NextInt(0, remainingRooms * 10);
                if(!openSet.Contains(neighbor)) openSet.Add(neighbor);
            }

            remainingRooms -= roomCounter;
            roomCounter = 0;

            closedSet.Add(current); 
            if(remainingRooms <= 0){
                // Select random child of current as endRoom (so endRoom only has one entry)
                // neightborList contains all neighbors of current
                if(neighborList.Length > 0){
                    // Select random child 
                    int neighborRandIndex = rand.NextInt(0, neighborList.Length - 1);
                    endNode = grid[neighborList[neighborRandIndex].x * grid_size + neighborList[neighborRandIndex].y];
                }
                else{
                    endNode = current;
                }
                // Add all nodes to the allNode list
                foreach(NodeStruct node in closedSet){
                    if(node.x < xoffset) xoffset = x;
                    if(node.y < yoffset) yoffset = y;
                    allNodes.Add(node);
                }
                foreach(NodeStruct node in openSet){
                    if(node.x < xoffset) xoffset = x;
                    if(node.y < yoffset) yoffset = y;
                    allNodes.Add(node);
                }
                return;
            }
        }
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
        String matrix = "";
        for(int i = 0; i < width; i++){
            
            for(int j = 0; j < height; j++){
                if(grid[i,j] == null) matrix += (0 + " ");
                else  matrix += (1 + " "); 
            }
            matrix += ("\n");
        }
    }
}

// class to paint the map recursively starting from endnode
public class MapPainter{

    private MapGen mapGen;
    private System.Random rand;

    private NativeArray<NodeStruct> grid;
    private int gridSize;

    public MapPainter(MapGen mapGen, NativeArray<NodeStruct> grid, int gridSize){
        this.grid = grid;
        this.gridSize = gridSize;
        this.mapGen = mapGen;
        rand = new System.Random();
    }

    public void paintNode(NativeList<NodeStruct> nodes, int xoffset, int yoffset, NativeList<NodeStruct> nodeStructList){
        foreach(NodeStruct node in nodes){
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
                else if(roomRandNum <= 55){
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
                else if(roomRandNum <= 65){
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
                else if(roomRandNum <= 75){
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
            setCorridorBooleans(node, roomScript, nodeStructList);
            
            // Draw corridors
            drawCorridors(x,y,roomScript);

            // Draw walls
            drawWalls(x,y,roomScript);

            // If enemy room -> add obstacles
            // Later do it so it checks for instance of enemyRoom class
            if(roomScript.roomType.Contains("EnemyRoom")){
                addObstacles(node, roomScript);
            }
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

    private void setCorridorBooleans(NodeStruct node, Room roomScript, NativeList<NodeStruct> nodeStructList){
        // Set corridors for childre
        foreach(NodeStruct childNode in node.neighbors(grid, gridSize, nodeStructList)){
            /*String dir = getDirection(node.x - childNode.x, node.y - childNode.y);
            switch(dir){
                case "North": roomScript.north = true; break;
                case "East": roomScript.east = true; break;
                case "South": roomScript.south = true; break;
                case "West": roomScript.west = true; break;
                default: break;
            }*/
            roomScript.north = node.north;
            roomScript.west = node.west;
            roomScript.east = node.east;
            roomScript.south = node.south;
        }
    }

    private void addObstacles(NodeStruct node, Room roomScript){
        // get center coordinates
        int x = roomScript.x + roomScript.width/2;
        int y = roomScript.y+ roomScript.length/2;
        // get size of usable center zone
        int width = (int) (roomScript.width*0.7);
        int length = (int) (roomScript.length*0.7);
        // Obstacle types
        // Boxes
        //      Normal Box (1x1)
        //      Box row v (2x4)
        //      Box row h (4x2)
        //      Box center (5x5 in the middle of the room)
        //      Box edges 1 (1 bix in each corner)
        //      Box corner 2 (3 boxes in each corner doing an L)

        // Calculate extra box spawn chance based on room size
        const float SIZE_BONUS = 2;
        float extraBoxChance = ((((float) (roomScript.width+roomScript.length))/(float)((RoomType.NORMAL_ROOM_WIDTH+RoomType.NORMAL_ROOM_LENGTH))) - 1f)*SIZE_BONUS + 1f; // 1 if room is normal, slightly larger if room is bigger
        // Place Boxes
        int normalBoxAmmount = rand.Next((int) (5*extraBoxChance),(int) (12*extraBoxChance)); // Random number between 0 and 15 with 15/3 chance of being 0
        int box_row_vAmmount = rand.Next(0,(int) (5*extraBoxChance)) - 2; // Random number between 0 and 4 with 50% chance of being 0
        int box_row_hAmmount = rand.Next(0,(int) (5*extraBoxChance)) - 2; // '' ''
        int boxEdgeType = rand.Next(0,5); // 0 - 2 = Nada (60%), 3 = box edge 1, 4 = box edge 2
        bool centerBox = rand.Next(0,100)<10? true:false; // 1 10% chance and 0 90%

        // If boxes are placed on the same time its no biggie, just let it
        // Place normal boxes and box clutters
        int boxX;
        int boxY;
        GameObject box;
        // normal rand boxes
        for(int i = 0; i < normalBoxAmmount && !centerBox; i++){
            boxX = rand.Next(x - width/2, x + width/2);
            boxY = rand.Next(y - length/2, y + length/2);
            box = GameObject.Instantiate(mapGen.box);
            box.transform.parent = roomScript.gameObject.transform; // Set box as child of room
            box.transform.position = new Vector2(boxX + 0.5f, boxY+ 0.5f);            
        }
        for(int i = 0; i < box_row_vAmmount && !centerBox; i++){
            boxX = rand.Next(x - width/2, x + width/2) - 1;
            boxY = rand.Next(y - length/2, y + length/2) - 3;
            // Places boxes on a 2 x 4 grid
            for(int a = 0; a < 2; a++){
                for(int b = 0; b < 4; b++){
                    box = GameObject.Instantiate(mapGen.box);
                    box.transform.parent = roomScript.gameObject.transform; // Set box as child of room
                    box.transform.position = new Vector2(boxX+a+0.5f, boxY+b+0.5f); 
                }
            }
        }
        for(int i = 0; i < box_row_hAmmount && !centerBox; i++){
            boxX = rand.Next(x - width/2, x + width/2) - 3;
            boxY = rand.Next(y - length/2, y + length/2) - 1;
            // Places boxes on a 4 x 2 grid
            for(int a = 0; a < 4; a++){
                for(int b = 0; b < 2; b++){
                    box = GameObject.Instantiate(mapGen.box);
                    box.transform.parent = roomScript.gameObject.transform; // Set box as child of room
                    box.transform.position = new Vector2(boxX+a+0.5f, boxY+b+0.5f); 
                }
            }
        }

        if(centerBox){
            boxEdgeType++; // Up the chance of a cronerL Box distr
            // Put a gigant box cluster on the center. Size depends on room size
            boxX = x - roomScript.width/6;
            boxY = y - roomScript.length/6;
            for(int i = 0; i < roomScript.width/3; i++){
                for(int j = 0; j < roomScript.length/3; j++){
                    box = GameObject.Instantiate(mapGen.box);
                    box.transform.parent = roomScript.gameObject.transform; // Set box as child of room
                    box.transform.position = new Vector2(boxX+i+0.5f, boxY+j+0.5f); 
                }
            }
        }

        // Corner boxes
        if(boxEdgeType == 3){
            // 11 -> one box on each corner
            // Edges = room.x, room.y, room.x + room.width, room.y etc
            boxX = roomScript.x;
            boxY = roomScript.y;
            box = GameObject.Instantiate(mapGen.box);
            box.transform.parent = roomScript.gameObject.transform; // Set box as child of room
            box.transform.position = new Vector2(boxX+0.5f, boxY+0.5f);
            boxX = roomScript.x + roomScript.width-1;
            boxY = roomScript.y;
            box = GameObject.Instantiate(mapGen.box);
            box.transform.parent = roomScript.gameObject.transform; // Set box as child of room
            box.transform.position = new Vector2(boxX+0.5f, boxY+0.5f);
            boxX = roomScript.x + roomScript.width-1;
            boxY = roomScript.y + roomScript.length-1;
            box = GameObject.Instantiate(mapGen.box);
            box.transform.parent = roomScript.gameObject.transform; // Set box as child of room
            box.transform.position = new Vector2(boxX+0.5f, boxY+0.5f);
            boxX = roomScript.x;
            boxY = roomScript.y + roomScript.length-1;
            box = GameObject.Instantiate(mapGen.box);
            box.transform.parent = roomScript.gameObject.transform; // Set box as child of room
            box.transform.position = new Vector2(boxX+0.5f, boxY+0.5f);
        }
        else if(boxEdgeType >= 4){
            boxX = roomScript.x;
            boxY = roomScript.y;

            // Pintar una L en esa esquina
            box = GameObject.Instantiate(mapGen.box);
            box.transform.parent = roomScript.gameObject.transform; // Set box as child of room
            box.transform.position = new Vector2(boxX+0.5f, boxY+0.5f);
            for(int i = 1; i <= (roomScript.width-roomScript.corridor_width)/4; i++){
                box = GameObject.Instantiate(mapGen.box);
                box.transform.parent = roomScript.gameObject.transform; // Set box as child of room
                box.transform.position = new Vector2(boxX+0.5f + i, boxY+0.5f);
            }
            for(int i = 1; i <= (roomScript.length-roomScript.corridor_width)/4; i++){
                box = GameObject.Instantiate(mapGen.box);
                box.transform.parent = roomScript.gameObject.transform; // Set box as child of room
                box.transform.position = new Vector2(boxX+0.5f, boxY+0.5f + i);
            }
            boxX = roomScript.x + roomScript.width-1;
            boxY = roomScript.y;
            box = GameObject.Instantiate(mapGen.box);
            box.transform.position = new Vector2(boxX+0.5f, boxY+0.5f);
            for(int i = 1; i <= (roomScript.width-roomScript.corridor_width)/4; i++){
                box = GameObject.Instantiate(mapGen.box);
                box.transform.parent = roomScript.gameObject.transform; // Set box as child of room
                box.transform.position = new Vector2(boxX+0.5f - i, boxY+0.5f);
            }
            for(int i = 1; i <= (roomScript.length-roomScript.corridor_width)/4; i++){
                box = GameObject.Instantiate(mapGen.box);
                box.transform.parent = roomScript.gameObject.transform; // Set box as child of room
                box.transform.position = new Vector2(boxX+0.5f, boxY+0.5f + i);
            }
            boxX = roomScript.x + roomScript.width-1;
            boxY = roomScript.y + roomScript.length-1;
            box = GameObject.Instantiate(mapGen.box);
            box.transform.position = new Vector2(boxX+0.5f, boxY+0.5f);
            for(int i = 1; i <= (roomScript.width-roomScript.corridor_width)/4; i++){
                box = GameObject.Instantiate(mapGen.box);
                box.transform.parent = roomScript.gameObject.transform; // Set box as child of room
                box.transform.position = new Vector2(boxX+0.5f - i, boxY+0.5f);
            }
            for(int i = 1; i <= (roomScript.length-roomScript.corridor_width)/4; i++){
                box = GameObject.Instantiate(mapGen.box);
                box.transform.parent = roomScript.gameObject.transform; // Set box as child of room
                box.transform.position = new Vector2(boxX+0.5f, boxY+0.5f - i);
            }
            boxX = roomScript.x;
            boxY = roomScript.y + roomScript.length-1;
            box = GameObject.Instantiate(mapGen.box);
            box.transform.position = new Vector2(boxX+0.5f, boxY+0.5f);
            for(int i = 1; i <= (roomScript.width-roomScript.corridor_width)/4; i++){
                box = GameObject.Instantiate(mapGen.box);
                box.transform.parent = roomScript.gameObject.transform; // Set box as child of room
                box.transform.position = new Vector2(boxX+0.5f + i, boxY+0.5f);
            }
            for(int i = 1; i <= (roomScript.length-roomScript.corridor_width)/4; i++){
                box = GameObject.Instantiate(mapGen.box);
                box.transform.parent = roomScript.gameObject.transform; // Set box as child of room
                box.transform.position = new Vector2(boxX+0.5f, boxY+0.5f - i);
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


