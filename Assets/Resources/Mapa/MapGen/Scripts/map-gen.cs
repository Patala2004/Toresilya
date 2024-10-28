using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;
using Unity.VisualScripting;
using Unity.Collections;
using UnityEngine.Tilemaps;
using System.Linq;
using Unity.Jobs;
using Unity.Burst;



public class MapGen: MonoBehaviour{

    public Tile floorTile;
    public Tile corridorTile;
    public Tile wallTile;
    public Tile doorTile;
    public Tilemap floorMap;
    public Tilemap wallMap;
    public GameObject box; // Box prefab

    public GameObject[] roomPrefabs18x18;
    public GameObject[] roomPrefabs26x26;
    public GameObject[] roomPrefabs18x26;
    public GameObject[] roomPrefabs26x18;

    public GameObject rooms;

    public AStarStruct mapGenerator;

    public int roomAmmount = 14;
    

    private bool hasBeenGenerated = false;

    private bool mapCanBeRendered = true;
    public String loadingStatus = "";

    void Start(){
        rooms = new GameObject();
        rooms.name = "Rooms";
        rooms.transform.parent = transform;
        generateMap();
    }
    public void generateMap(){
        StartCoroutine(generateMapCoroutine());
    }

    public void clearMap(){
        floorMap.ClearAllTiles();
        wallMap.ClearAllTiles();
        foreach (Transform child in rooms.transform){
            Destroy(child.gameObject);
        }
    }



    IEnumerator generateMapCoroutine(){
        loadingStatus = "Initializing map generation data strcutres";
        //mapGenerator = new AStar(10,10,14);
        System.Random seedGen = new System.Random();
        uint seed = (uint) seedGen.Next(1,999999999);
        Unity.Mathematics.Random rand = new Unity.Mathematics.Random(seed);
        int gridSize = roomAmmount*2 + 3;
        NativeArray<NodeStruct> girdStruct = new NativeArray<NodeStruct>(gridSize*gridSize, Allocator.Persistent);
        NativeList<NodeStruct> openSet = new NativeList<NodeStruct>(Allocator.Persistent);
        NativeList<NodeStruct> allNodes = new NativeList<NodeStruct>(Allocator.Persistent);
        NativeHashSet<NodeStruct> closedSet = new NativeHashSet<NodeStruct>(100000, Allocator.Persistent);
        NativeArray<bool> booleanArray_s4 = new NativeArray<bool>(4, Allocator.Persistent);
        NativeList<NodeStruct> nativeListNodeStruc = new NativeList<NodeStruct>(Allocator.Persistent);
        AStarStruct mapGenerator2 = new AStarStruct(gridSize,rand,roomAmmount, girdStruct,allNodes, openSet, closedSet, booleanArray_s4, nativeListNodeStruc, new NodeStruct());

        NativeArray<int> startNodeCoords = new NativeArray<int>(2, Allocator.Persistent);
        NativeArray<int> endNodeCoords = new NativeArray<int>(2, Allocator.Persistent);
        //mapGenerator2.createMap();

        MapNodeGenerationJob newJob = new MapNodeGenerationJob
        {
            mapGenerator3 = mapGenerator2,
            startNodeCoords = startNodeCoords,
            endNodeCoords = endNodeCoords,
        };

        loadingStatus = "Generating map tree";

        JobHandle jobhandle = newJob.Schedule();

        while(!jobhandle.IsCompleted){
            yield return null;
        }
        loadingStatus = "Map tree generated";
        jobhandle.Complete();
        

        // job is completed!
        girdStruct.Dispose();
        openSet.Dispose();
        closedSet.Dispose();
        booleanArray_s4.Dispose();
        nativeListNodeStruc.Dispose();

        loadingStatus = "Preparing data structures to calculate tile positions";

        int xoffset = allNodes[0].x;
        int yoffset = allNodes[0].y;
        foreach(NodeStruct node in allNodes){
            if(node.x < xoffset) xoffset = node.x;
            if(node.y < yoffset) yoffset = node.y;
        }

        


        NativeList<int> roomTypes = new NativeList<int>(Allocator.Persistent);
        NativeList<int> xFloorCoords = new NativeList<int>(Allocator.Persistent);
        NativeList<int> yFloorCoords = new NativeList<int>(Allocator.Persistent);
        NativeList<int> xCorridorCoords = new NativeList<int>(Allocator.Persistent);
        NativeList<int> yCorridorCoords = new NativeList<int>(Allocator.Persistent);
        NativeList<int> xWallCoords = new NativeList<int>(Allocator.Persistent);
        NativeList<int> yWallCoords = new NativeList<int>(Allocator.Persistent);

        RoomTypeGetter roomTypeGetter = new RoomTypeGetter(allNodes, rand, roomTypes, startNodeCoords[0], startNodeCoords[1], endNodeCoords[0], endNodeCoords[1]);
        FloorCoordinateGetter floorCoordinateGetter = new FloorCoordinateGetter(allNodes,roomTypes, xFloorCoords, yFloorCoords,xoffset, yoffset);
        CorridorCoordinateGetter corridorCoordinateGetter = new CorridorCoordinateGetter(allNodes,roomTypes, xCorridorCoords, yCorridorCoords,xoffset, yoffset);
        WallCoordinateGetter wallCoordinateGetter = new WallCoordinateGetter(allNodes,roomTypes, xWallCoords, yWallCoords, xoffset, yoffset);

        RoomTypeGeneratorJob roomTypeGenJob = new RoomTypeGeneratorJob{
            roomTypeGetter = roomTypeGetter,
        };

        FloorCoordinatesCalculatorJob floorCoordJob = new FloorCoordinatesCalculatorJob{
            floorCoordGetter = floorCoordinateGetter,
        };

        CorridorCoordinatesCalculatorJob corrCoordJob = new CorridorCoordinatesCalculatorJob{
            corridorCoordGetter = corridorCoordinateGetter,
        };

        WallCoordinatesCalculatorJob wallCoordJob = new WallCoordinatesCalculatorJob{
            wallCoordGetter = wallCoordinateGetter,
        };

        JobHandle roomTypeGen = roomTypeGenJob.Schedule();
        roomTypeGen.Complete(); // Room Types have to be generated before the rest can be done

        loadingStatus = "Calculating tile positions";

        JobHandle floorJob = floorCoordJob.Schedule();
        JobHandle coorJob = corrCoordJob.Schedule();
        JobHandle wallJob = wallCoordJob.Schedule();


        // Generate room objects
        RoomCreator.createRooms(allNodes, roomTypes, xoffset, yoffset, rooms, roomPrefabs18x18, roomPrefabs26x26, roomPrefabs18x26, roomPrefabs26x18);

        while(!floorJob.IsCompleted || !coorJob.IsCompleted || !wallJob.IsCompleted){
            // Dont block main thread while jobs are running
            yield return null;
        }
        // Syncronize the three already completed jobs with main thread
        floorJob.Complete();
        coorJob.Complete();
        wallJob.Complete();
        // Three jobs are completed 

        loadingStatus = "Tile positions calculated. Waiting for game permission to render them";   


        //Wait for the game to be ready to render a map
        while(!mapCanBeRendered){
            yield return null;
        }


        loadingStatus = "Rendering map";
        yield return null; // Give it a frame to update text

        TileRenderer tileRenderer = new TileRenderer(xFloorCoords, yFloorCoords, xCorridorCoords, yCorridorCoords, xWallCoords, yWallCoords, 
        floorTile, corridorTile, wallTile, roomTypes);
        
        floorMap.SetTiles(tileRenderer.vectorCoordinates, tileRenderer.tileArr);
        wallMap.SetTiles(tileRenderer.wallVectorCoordinates, tileRenderer.wallTileArr);

        GameObject.Find("player").transform.position = new Vector3((startNodeCoords[0] - xoffset) * (RoomType.MAX_ROOM_SIZE + RoomType.NORMAL_CORRIDOR_LENGTH) + RoomType.NORMAL_CORRIDOR_LENGTH + RoomType.NORMAL_ROOM_WIDTH/2, (startNodeCoords[1] - yoffset) * (RoomType.MAX_ROOM_SIZE + RoomType.NORMAL_CORRIDOR_LENGTH) + RoomType.NORMAL_CORRIDOR_LENGTH + RoomType.NORMAL_ROOM_LENGTH/2, 0);
        //GameObject.Find("Enemigo").transform.position = new Vector3((startNodeCoords[0] - xoffset) * (RoomType.MAX_ROOM_SIZE + RoomType.NORMAL_CORRIDOR_LENGTH) + RoomType.NORMAL_CORRIDOR_LENGTH + RoomType.NORMAL_ROOM_WIDTH/2, (startNodeCoords[1] - yoffset) * (RoomType.MAX_ROOM_SIZE + RoomType.NORMAL_CORRIDOR_LENGTH) + RoomType.NORMAL_CORRIDOR_LENGTH + RoomType.NORMAL_ROOM_LENGTH/2 + 3, 0);


        // TEMP -> draw other collor at start and endnode
        floorMap.SetTile(new Vector3Int((startNodeCoords[0] - xoffset) * (RoomType.MAX_ROOM_SIZE + RoomType.NORMAL_CORRIDOR_LENGTH) + RoomType.NORMAL_CORRIDOR_LENGTH + RoomType.NORMAL_ROOM_WIDTH/2, (startNodeCoords[1] - yoffset) * (RoomType.MAX_ROOM_SIZE + RoomType.NORMAL_CORRIDOR_LENGTH) + RoomType.NORMAL_CORRIDOR_LENGTH + RoomType.NORMAL_ROOM_LENGTH/2, 0),corridorTile);
        floorMap.SetTile(new Vector3Int((endNodeCoords[0] - xoffset) * (RoomType.MAX_ROOM_SIZE + RoomType.NORMAL_CORRIDOR_LENGTH) + RoomType.NORMAL_CORRIDOR_LENGTH  + RoomType.NORMAL_ROOM_WIDTH/2, (endNodeCoords[1] - yoffset) * (RoomType.MAX_ROOM_SIZE + RoomType.NORMAL_CORRIDOR_LENGTH) + RoomType.NORMAL_CORRIDOR_LENGTH  + RoomType.NORMAL_ROOM_LENGTH/2, 0),corridorTile);

        roomTypes.Dispose();
        xFloorCoords.Dispose();
        yFloorCoords.Dispose();
        xCorridorCoords.Dispose();
        yCorridorCoords.Dispose();
        xWallCoords.Dispose();
        yWallCoords.Dispose();
        allNodes.Dispose();
        startNodeCoords.Dispose();
        loadingStatus = "DONE";
    }

    [BurstCompile]
    struct MapNodeGenerationJob : IJob{
        public AStarStruct mapGenerator3;
        public int xoffset;
        public int yoffset;

        public NativeArray<int> startNodeCoords;
        public NativeArray<int> endNodeCoords;

        public void Execute(){
            mapGenerator3.createMap();
            startNodeCoords[0] = mapGenerator3.startNode.x;
            startNodeCoords[1] = mapGenerator3.startNode.y;
            endNodeCoords[0] = mapGenerator3.endNode.x;
            endNodeCoords[1] = mapGenerator3.endNode.y;
        }
    }

    [BurstCompile]
    struct RoomTypeGeneratorJob : IJob{

        public RoomTypeGetter roomTypeGetter;
        // xcoordinates and ycoordinates gets passed to the NativeLists passed to the constructor of the coordGetter 
        
        public void Execute(){
            roomTypeGetter.getRoomTypes();
        }
    }

    [BurstCompile]
    struct FloorCoordinatesCalculatorJob : IJob{

        public FloorCoordinateGetter floorCoordGetter;
        // xcoordinates and ycoordinates gets passed to the NativeLists passed to the constructor of the coordGetter 
        
        public void Execute(){
            floorCoordGetter.getFloorCoordinates();
        }
    }

    [BurstCompile]
    struct CorridorCoordinatesCalculatorJob : IJob{

        public CorridorCoordinateGetter corridorCoordGetter;
        // xcoordinates and ycoordinates gets passed to the NativeLists passed to the constructor of the coordGetter 
        
        public void Execute(){
            corridorCoordGetter.getCorridorCoordinates();
        }
    }

    [BurstCompile]
    struct WallCoordinatesCalculatorJob : IJob{
        public WallCoordinateGetter wallCoordGetter;
        // xcoordinates and ycoordinates gets passed to the NativeLists passed to the constructor of the coordGetter 
        
        public void Execute(){
            wallCoordGetter.getWallCoordinates();
        }
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

        while(availableNeighborsCounter > 0 && remaindingChildren > 0){
            int i = rand.NextInt(0,4); // Random direction -> 4 instead of 3 because MAX value is exclusive (cannot be generated)
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
        int x = rand.NextInt(roomAmmount + 1, grid_size - roomAmmount+1);
        int y = rand.NextInt(roomAmmount + 1, grid_size - roomAmmount+1); // +1 because maxValue is exclusive

        startNode = grid[x*grid_size + y]; // STRUCTS ARE PASSED BY VALUE NOT POINTER -> MODIFYING THIS DOESNT MODIFY THE VALUE INSIDE OF THE ARRAY
        startNode.initialize(x,y);
        startNode.heuristic = rand.NextInt(roomAmmount, 10000);
        startNode.cost = 0;
        grid[x*grid_size + y] = startNode;
        xoffset = x;
        yoffset = y;
        remainingRooms--;

        openSet.Add(startNode);

        // Get open Node with smallest functional Cost (costs + heuristic)
        while(openSet.Length > 0){
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
                int roomCount = rand.NextInt(2,4); // rand room ammount between 2 and 3 (4 because max value is exclusive so cant be chosen)
                if(current == startNode || remainingRooms == 1) roomCount = 1;
                else if(remainingRooms == 2) roomCount = 2;
                roomCounter+= current.createChildren(grid, grid_size, roomCount, rand, booleanArray_s4);
            }
            // Empty nodeStructList
            while(nodeStructList.Length > 0){
                nodeStructList.RemoveAt(0);
            }

            NativeList<NodeStruct> neighborList = current.neighbors(grid, grid_size, nodeStructList);
            // Cannot use foeach because foreach iterations cannot be modified
            for(int i = 0; i < neighborList.Length; i++){
                NodeStruct neighbor = neighborList[i];
                if(!neighbor.hasBeenCreated || !neighbor.isWalkable || closedSet.Contains(neighbor)) continue;
                neighbor.cost = current.cost;
                neighbor.heuristic = rand.NextInt(0, remainingRooms * 10 + 1);
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
                    int neighborRandIndex = rand.NextInt(0, neighborList.Length);
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

public struct RoomTypeGetter{

    [ReadOnly] public NativeList<NodeStruct> allNodes;
    public NativeList<int> roomType;
    public Unity.Mathematics.Random rand;

    [ReadOnly] int startx, starty, endx, endy;

    public RoomTypeGetter(NativeList<NodeStruct> allNodes, Unity.Mathematics.Random rand, NativeList<int> roomType,
    int startx, int starty, int endx, int endy){
        this.allNodes = allNodes;
        this.rand = rand;
        this.roomType = roomType;
        this.startx = startx;
        this.starty = starty;
        this.endx = endx;
        this.endy = endy;
        getRoomTypes(); // So the same struct can be used in many Jobs
    }

    

    // Using rand calculate the room Types
    public void getRoomTypes(){
        foreach(NodeStruct node in allNodes){
            // Check if node is start or endnode
            if(node.x == startx && node.y == starty){
                roomType.Add(RoomType.START_ROOM_CODE);
            }
            else if(node.x == endx && node.y == endy){
                roomType.Add(RoomType.END_ROOM_CODE);
            }
            else{
                // Generate random room Type
                // Room types -> Enemy Room, Large Enemy Room, Streched Enemy Room H, Streched Enemy Room V, Chest room, store room, healing room
                // Enemy Room -> 70% -> Normal = 40%, large = 10%, streched h = 10%, streched v = 10%
                // chest room = 15%
                // store room = 10%
                // healing room = 5%
                int roomRandNum = rand.NextInt(0,101); // random value between 0 and 100
                if(roomRandNum < 40){ // Normal enemy room
                    roomType.Add(RoomType.NORMAL_ENEMY_ROOM_CODE);
                }
                else if(roomRandNum < 50){
                    roomType.Add(RoomType.LARGE_ENEMY_ROOM_CODE);
                }
                else if(roomRandNum < 60){
                    roomType.Add(RoomType.STRECHED_ENEMY_ROOM_H_CODE);
                }
                else if(roomRandNum < 70){
                    roomType.Add(RoomType.STRECHED_ENEMY_ROOM_V_CODE);
                }
                else if(roomRandNum < 85){
                    roomType.Add(RoomType.CHEST_ROOM_CODE);
                }
                else if(roomRandNum < 95){
                    roomType.Add(RoomType.STORE_ROOM_CODE);
                }
                else if(roomRandNum <= 100){
                    roomType.Add(RoomType.HEALING_ROOM_CODE);
                }
            }
        }
    }
}

public struct FloorCoordinateGetter{

    [ReadOnly] public NativeList<NodeStruct> allNodes;

    [ReadOnly] public NativeList<int> roomType;

    public NativeList<int> floorxcoordinates;
    public NativeList<int> floorycoordinates;
    [ReadOnly] int xoffset, yoffset;

    public FloorCoordinateGetter(NativeList<NodeStruct> allNodes, NativeList<int> roomType, 
    NativeList<int> floorxcoordinates, NativeList<int> floorycoordinates,
    int xoffset, int yoffset){
        this.allNodes = allNodes;
        this.roomType = roomType;
        this.floorxcoordinates = floorxcoordinates;
        this.floorycoordinates = floorycoordinates;
        this.xoffset = xoffset;
        this.yoffset = yoffset;
    }
    public void getFloorCoordinates(){
        // allNodes.Length == roomTypes.length
        for(int i = 0; i < allNodes.Length; i++){
            int x = allNodes[i].x;
            int y = allNodes[i].y;
            int xsizeoffset = 0;
            int ysizeoffset = 0;
            // Set room sizes
            int width = RoomType.NORMAL_ROOM_WIDTH;
            int length = RoomType.NORMAL_ROOM_LENGTH;
            if(roomType[i] == RoomType.LARGE_ENEMY_ROOM_CODE){
                width = RoomType.LARGE_ROOM_WIDTH;
                length = RoomType.LARGE_ROOM_WIDTH; 
                xsizeoffset -= (RoomType.LARGE_ROOM_WIDTH-RoomType.NORMAL_ROOM_WIDTH)/2;
                ysizeoffset -= (RoomType.LARGE_ROOM_LENGTH-RoomType.NORMAL_ROOM_LENGTH)/2;
                
            }
            else if(roomType[i] == RoomType.STRECHED_ENEMY_ROOM_H_CODE){
                width = RoomType.LARGE_ROOM_WIDTH;
                length = RoomType.NORMAL_ROOM_LENGTH;
                xsizeoffset -= (RoomType.LARGE_ROOM_WIDTH-RoomType.NORMAL_ROOM_WIDTH)/2;
            }
            else if(roomType[i] == RoomType.STRECHED_ENEMY_ROOM_V_CODE){
                width = RoomType.NORMAL_ROOM_WIDTH;
                length = RoomType.LARGE_ROOM_WIDTH;      
                ysizeoffset -= (RoomType.LARGE_ROOM_LENGTH-RoomType.NORMAL_ROOM_LENGTH)/2;    
            }
            // Add coordinates based on room size by going from left to right
            for(int a = 0; a < width; a++){
                for(int b = 0; b < length; b++){
                    // Add x and y
                    floorxcoordinates.Add((x-xoffset)*(RoomType.MAX_ROOM_SIZE + RoomType.NORMAL_CORRIDOR_LENGTH) + xsizeoffset + RoomType.NORMAL_CORRIDOR_LENGTH + a );
                    floorycoordinates.Add((y-yoffset)*(RoomType.MAX_ROOM_SIZE + RoomType.NORMAL_CORRIDOR_LENGTH) + ysizeoffset + RoomType.NORMAL_CORRIDOR_LENGTH + b);
                }
            }
        }
    }
}
public struct WallCoordinateGetter{

    [ReadOnly] public NativeList<NodeStruct> allNodes;
    [ReadOnly] public NativeList<int> roomType;
    public NativeList<int> wallxcoordinates;
    public NativeList<int> wallycoordinates;

    [ReadOnly] int xoffset, yoffset;

    public WallCoordinateGetter(NativeList<NodeStruct> allNodes, NativeList<int> roomType,
    NativeList<int> wallxcoordinates, NativeList<int> wallycoordinates,
    int xoffset, int yoffset){
        this.allNodes = allNodes;
        this.roomType = roomType;
        this.wallxcoordinates = wallxcoordinates;
        this.wallycoordinates = wallycoordinates;
        this.xoffset = xoffset;
        this.yoffset = yoffset;
    }

    public void getWallCoordinates(){
        for(int i = 0; i < allNodes.Length; i++){
            int x = (allNodes[i].x - xoffset)*(RoomType.MAX_ROOM_SIZE + RoomType.NORMAL_CORRIDOR_LENGTH) + RoomType.NORMAL_CORRIDOR_LENGTH;
            int y = (allNodes[i].y - yoffset)*(RoomType.MAX_ROOM_SIZE + RoomType.NORMAL_CORRIDOR_LENGTH) + RoomType.NORMAL_CORRIDOR_LENGTH;

            // Set room sizes
            int width = RoomType.NORMAL_ROOM_WIDTH;
            int length = RoomType.NORMAL_ROOM_LENGTH;
            int vertical_corridor_length = RoomType.NORMAL_CORRIDOR_LENGTH;
            int horizontal_corridor_length = RoomType.NORMAL_CORRIDOR_LENGTH;
            if(roomType[i] == RoomType.LARGE_ENEMY_ROOM_CODE){
                width = RoomType.LARGE_ROOM_WIDTH;
                length = RoomType.LARGE_ROOM_WIDTH;
                x -= (RoomType.LARGE_ROOM_WIDTH-RoomType.NORMAL_ROOM_WIDTH)/2;
                y -= (RoomType.LARGE_ROOM_LENGTH-RoomType.NORMAL_ROOM_LENGTH)/2;
                vertical_corridor_length = RoomType.LARGE_ROOM_CORRIDOR_LENGTH;
                horizontal_corridor_length = RoomType.LARGE_ROOM_CORRIDOR_LENGTH;
            }
            else if(roomType[i] == RoomType.STRECHED_ENEMY_ROOM_H_CODE){
                width = RoomType.LARGE_ROOM_WIDTH;
                x -= (RoomType.LARGE_ROOM_WIDTH-RoomType.NORMAL_ROOM_WIDTH)/2;
                horizontal_corridor_length = RoomType.LARGE_ROOM_CORRIDOR_LENGTH;
            }
            else if(roomType[i] == RoomType.STRECHED_ENEMY_ROOM_V_CODE){
                length = RoomType.LARGE_ROOM_WIDTH;
                y -= (RoomType.LARGE_ROOM_LENGTH-RoomType.NORMAL_ROOM_LENGTH)/2;
                vertical_corridor_length = RoomType.LARGE_ROOM_CORRIDOR_LENGTH; 
            }

            // Add walls
            if(allNodes[i].north){
                int woffset = (width - RoomType.CORRIDOR_WIDTH)/2; // So it doesnt have to do a division every single tile (i fucking hate divisions)
                // Paint north wall with a hole for the corridor
                for(int a = -1; a < woffset; a++){
                    wallxcoordinates.Add(x + a);
                    wallycoordinates.Add(y + length);
                }
                for(int a = woffset + RoomType.CORRIDOR_WIDTH; a < width + 1 ; a++){
                    wallxcoordinates.Add(x + a);
                    wallycoordinates.Add(y + length);
                }
                // Paint corridor walls
                for(int a = 1; a < vertical_corridor_length - 1; a++){
                    wallxcoordinates.Add(x + woffset - 1);
                    wallycoordinates.Add(y + length + a);
                    wallxcoordinates.Add(x + woffset + RoomType.CORRIDOR_WIDTH);
                    wallycoordinates.Add(y + length + a);
                }
            }
            else{
                for(int a = -1; a < width + 1; a++){
                    wallxcoordinates.Add(x + a);
                    wallycoordinates.Add(y + length);
                }
            }
            if(allNodes[i].east){
                int loffset = (length - RoomType.CORRIDOR_WIDTH)/2;
                for(int a = 0; a < loffset; a++){
                    wallxcoordinates.Add(x + width);
                    wallycoordinates.Add(y + a);
                }
                for(int a = loffset + RoomType.CORRIDOR_WIDTH; a < length; a++){
                    wallxcoordinates.Add(x + width);
                    wallycoordinates.Add(y + a);                    
                }
                // Paint corridor walls
                for(int a = 1; a < horizontal_corridor_length - 1; a++){
                    wallxcoordinates.Add(x + width + a);
                    wallycoordinates.Add(y + loffset - 1);
                    wallxcoordinates.Add(x + width + a);
                    wallycoordinates.Add(y + loffset + RoomType.CORRIDOR_WIDTH);
                }
            }
            else{
                for(int a = 0; a < length; a++){
                    wallxcoordinates.Add(x + width);
                    wallycoordinates.Add(y + a);                          
                }
            }
            if(allNodes[i].south){
                int woffset = (width - RoomType.CORRIDOR_WIDTH)/2;
                for(int a = -1; a < woffset; a++){
                    wallxcoordinates.Add(x + a);
                    wallycoordinates.Add(y - 1);
                }
                for(int a = woffset + RoomType.CORRIDOR_WIDTH; a < width + 1 ; a++){
                    wallxcoordinates.Add(x + a);
                    wallycoordinates.Add(y - 1);
                }
                // Paint corridor walls
                for(int a = 2; a < vertical_corridor_length; a++){
                    wallxcoordinates.Add(x + woffset - 1);
                    wallycoordinates.Add(y - a);
                    wallxcoordinates.Add(x + woffset + RoomType.CORRIDOR_WIDTH);
                    wallycoordinates.Add(y - a);
                }
            }
            else{
                for(int a = -1; a < width + 1; a++){
                    wallxcoordinates.Add(x + a);
                    wallycoordinates.Add(y - 1);
                }
            }
            if(allNodes[i].west){
                int loffset = (length - RoomType.CORRIDOR_WIDTH)/2;
                for(int a = 0; a < loffset; a++){
                    wallxcoordinates.Add(x - 1);
                    wallycoordinates.Add(y + a);
                }
                for(int a = loffset + RoomType.CORRIDOR_WIDTH; a < length; a++){
                    wallxcoordinates.Add(x - 1);
                    wallycoordinates.Add(y + a);                    
                }
                // Paint corridor walls
                for(int a = 2; a < horizontal_corridor_length; a++){
                    wallxcoordinates.Add(x - a);
                    wallycoordinates.Add(y + loffset - 1);
                    wallxcoordinates.Add(x - a);
                    wallycoordinates.Add(y + loffset + RoomType.CORRIDOR_WIDTH);
                }
            }
            else{
                for(int a = 0; a < length; a++){
                    wallxcoordinates.Add(x - 1);
                    wallycoordinates.Add(y + a);                          
                }
            }
        }
    }
}
public struct CorridorCoordinateGetter{

    [ReadOnly]public NativeList<NodeStruct> allNodes;
    [ReadOnly]public NativeList<int> roomType;
    public NativeList<int> corridorxcoordinates;
    public NativeList<int> corridorycoordinates;
    [ReadOnly]int xoffset, yoffset;

    public CorridorCoordinateGetter(NativeList<NodeStruct> allNodes, NativeList<int> roomType,
    NativeList<int> corridorxcoordinates, NativeList<int> corridorycoordinates, 
    int xoffset, int yoffset){
        this.allNodes = allNodes;
        this.roomType = roomType;
        this.corridorxcoordinates = corridorxcoordinates;
        this.corridorycoordinates = corridorycoordinates;
        this.xoffset = xoffset;
        this.yoffset = yoffset;
    }

    public void getCorridorCoordinates(){
        // allNodes.Length == roomTypes.length -> Cant use foreach 
        for(int i = 0; i < allNodes.Length; i++){
            int x = (allNodes[i].x - xoffset)*(RoomType.MAX_ROOM_SIZE + RoomType.NORMAL_CORRIDOR_LENGTH) + RoomType.NORMAL_CORRIDOR_LENGTH;
            int y = (allNodes[i].y - yoffset)*(RoomType.MAX_ROOM_SIZE + RoomType.NORMAL_CORRIDOR_LENGTH) + RoomType.NORMAL_CORRIDOR_LENGTH;

            // Set room sizes
            int width = RoomType.NORMAL_ROOM_WIDTH;
            int length = RoomType.NORMAL_ROOM_LENGTH;
            int vertical_corridor_width = RoomType.NORMAL_CORRIDOR_LENGTH;
            int horizontal_corridor_width = RoomType.NORMAL_CORRIDOR_LENGTH;
            if(roomType[i] == RoomType.LARGE_ENEMY_ROOM_CODE){
                width = RoomType.LARGE_ROOM_WIDTH;
                length = RoomType.LARGE_ROOM_WIDTH;
                vertical_corridor_width = RoomType.LARGE_ROOM_CORRIDOR_LENGTH;
                horizontal_corridor_width = RoomType.LARGE_ROOM_CORRIDOR_LENGTH;
                x -= (RoomType.LARGE_ROOM_WIDTH-RoomType.NORMAL_ROOM_WIDTH)/2;
                y -= (RoomType.LARGE_ROOM_LENGTH-RoomType.NORMAL_ROOM_LENGTH)/2;
            }
            else if(roomType[i] == RoomType.STRECHED_ENEMY_ROOM_H_CODE){
                width = RoomType.LARGE_ROOM_WIDTH;
                length = RoomType.NORMAL_ROOM_LENGTH;
                horizontal_corridor_width = RoomType.LARGE_ROOM_CORRIDOR_LENGTH;
                x -= (RoomType.LARGE_ROOM_WIDTH-RoomType.NORMAL_ROOM_WIDTH)/2;
            }
            else if(roomType[i] == RoomType.STRECHED_ENEMY_ROOM_V_CODE){
                width = RoomType.NORMAL_ROOM_WIDTH;
                length = RoomType.LARGE_ROOM_WIDTH;    
                vertical_corridor_width = RoomType.LARGE_ROOM_CORRIDOR_LENGTH;
                y -= (RoomType.LARGE_ROOM_LENGTH-RoomType.NORMAL_ROOM_LENGTH)/2;     
            }

            // Add corridors
            if(allNodes[i].north){
                int woffset = (width - RoomType.CORRIDOR_WIDTH)/2; // So it doesnt have to do a division every single tile (i fucking hate divisions)
                for(int a = 0; a < RoomType.CORRIDOR_WIDTH; a++){
                    for(int b = 0; b < vertical_corridor_width; b++){
                        corridorxcoordinates.Add(x + woffset + a);
                        corridorycoordinates.Add(y + length + b);
                    }
                }
            }
            if(allNodes[i].east){
                int loffset = (length - RoomType.CORRIDOR_WIDTH)/2;
                for(int a = 0; a < RoomType.CORRIDOR_WIDTH; a++){
                    for(int b = 0; b < horizontal_corridor_width; b++){
                        corridorxcoordinates.Add(x + width + b);
                        corridorycoordinates.Add(y + loffset + a);
                    }
                }
            }
            if(allNodes[i].south){
                int woffset = (width - RoomType.CORRIDOR_WIDTH)/2; // So it doesnt have to do a division every single tile (i fucking hate divisions)
                for(int a = 0; a < RoomType.CORRIDOR_WIDTH; a++){
                    for(int b = 0; b < vertical_corridor_width; b++){
                        corridorxcoordinates.Add(x + woffset + a);
                        corridorycoordinates.Add(y - 1 - b);
                    }
                }
            }
            if(allNodes[i].west){
                int loffset = (length - RoomType.CORRIDOR_WIDTH)/2;
                for(int a = 0; a < RoomType.CORRIDOR_WIDTH; a++){
                    for(int b = 0; b < horizontal_corridor_width; b++){
                        corridorxcoordinates.Add(x - 1 - b);
                        corridorycoordinates.Add(y + loffset + a);
                    }
                }
            }
        }
    }
}

public class TileRenderer{

    public Tile floorTile;
    public Tile corridorTile;
    public Tile wallTile;
    public NativeList<int> floorxCoordinates;
    public NativeList<int> flooryCoordinates;
    public NativeList<int> corridorxCoordinates;
    public NativeList<int> corridoryCoordinates;
    public NativeList<int> wallyCoordinates;
    public NativeList<int> wallxCoordinates;
    public NativeList<int> roomTypes;

    public Vector3Int[] vectorCoordinates;
    public Vector3Int[] wallVectorCoordinates;
    public Tile[] tileArr;
    public Tile[] wallTileArr;

    public TileRenderer(NativeList<int> floorxCoordinates, NativeList<int> flooryCoordinates, NativeList<int> corridorxCoordinates, 
    NativeList<int> corridoryCoordinates, NativeList<int> wallxCoordinates, NativeList<int> wallyCoordinates,
    Tile floorTile, Tile corridorTile, Tile wallTile, NativeList<int> roomTypes){
        this.floorxCoordinates = floorxCoordinates;
        this.flooryCoordinates = flooryCoordinates;
        this.corridorxCoordinates = corridorxCoordinates;
        this.corridoryCoordinates = corridoryCoordinates;
        this.wallxCoordinates = wallxCoordinates;
        this.wallyCoordinates = wallyCoordinates;
        this.roomTypes = roomTypes;
        this.corridorTile = corridorTile;
        this.wallTile = wallTile;

        int arrLens = floorxCoordinates.Length + corridorxCoordinates.Length;
        vectorCoordinates = new Vector3Int[arrLens];
        tileArr = new Tile[arrLens];

        wallVectorCoordinates = new Vector3Int[wallxCoordinates.Length];
        wallTileArr = new Tile[wallVectorCoordinates.Length];

        for(int i = 0; i < floorxCoordinates.Length; i++){
            vectorCoordinates[i] = new Vector3Int(floorxCoordinates[i], flooryCoordinates[i], 0);
            tileArr[i] = floorTile;
        }

        for(int i = 0; i < corridorxCoordinates.Length; i++){
            vectorCoordinates[i + floorxCoordinates.Length] = new Vector3Int(corridorxCoordinates[i], corridoryCoordinates[i], 0);
            tileArr[i + floorxCoordinates.Length] = corridorTile;
        }

        for(int i = 0; i < wallxCoordinates.Length; i++){
            wallVectorCoordinates[i] = new Vector3Int(wallxCoordinates[i], wallyCoordinates[i], 0);
            wallTileArr[i] = wallTile;
        }
    }
}

public class RoomCreator{
    public static void createRooms(NativeList<NodeStruct> allNodes, NativeList<int> roomType, int xoffset, int yoffset, GameObject rooms
    , GameObject[] roomPrefab18x18, GameObject[] roomPrefab26x26, GameObject[] roomPrefab18x26, GameObject[] roomPrefab26x18){
        for(int i = 0; i < allNodes.Length; i++){ // AllNodes.Length == roomTypes.Length
            int x = allNodes[i].x - xoffset;
            int y = allNodes[i].y - yoffset;
            
            // Get width and length
            int length = RoomType.NORMAL_ROOM_LENGTH;
            int width = RoomType.NORMAL_ROOM_WIDTH;

            // Create object
            GameObject newRoom = new GameObject();
            newRoom.transform.parent = rooms.transform;
            newRoom.name = "room " + x +"," + y;
            x = x*(RoomType.MAX_ROOM_SIZE + RoomType.NORMAL_CORRIDOR_LENGTH) + RoomType.NORMAL_CORRIDOR_LENGTH;
            y = y*(RoomType.MAX_ROOM_SIZE + RoomType.NORMAL_CORRIDOR_LENGTH) + RoomType.NORMAL_CORRIDOR_LENGTH;

            if(roomType[i] == RoomType.LARGE_ENEMY_ROOM_CODE){
                width = RoomType.LARGE_ROOM_WIDTH;
                length = RoomType.LARGE_ROOM_WIDTH;
                x -= (RoomType.LARGE_ROOM_WIDTH-RoomType.NORMAL_ROOM_WIDTH)/2;
                y -= (RoomType.LARGE_ROOM_LENGTH-RoomType.NORMAL_ROOM_LENGTH)/2;
            }
            else if(roomType[i] == RoomType.STRECHED_ENEMY_ROOM_H_CODE){
                width = RoomType.LARGE_ROOM_WIDTH;
                x -= (RoomType.LARGE_ROOM_WIDTH-RoomType.NORMAL_ROOM_WIDTH)/2;
            }
            else if(roomType[i] == RoomType.STRECHED_ENEMY_ROOM_V_CODE){
                length = RoomType.LARGE_ROOM_WIDTH;    
                y -= (RoomType.LARGE_ROOM_LENGTH-RoomType.NORMAL_ROOM_LENGTH)/2;     
            }

            // Create collider
            newRoom.transform.position = new Vector3(x + width/2,y + length/2,0); // +width/2 etc so collider gets placed on the center, not bottom left
            BoxCollider2D roomCollider = newRoom.AddComponent<BoxCollider2D>();
            roomCollider.size = new Vector2Int(width - 2, length - 2); // Give some room so doors dont get close on player
            roomCollider.isTrigger = true;

            // Add Script component and give it its variables
            Room roomScript = newRoom.AddComponent<Room>();
            roomScript.roomCollider = roomCollider;
            roomScript.x = x; // Pass along coordinates of bottom left corner
            roomScript.y = y;
            roomScript.width = width;
            roomScript.length = length;
            roomScript.roomType = roomType[i];
            // Corridor booleans
            roomScript.passCorridorBooleans(allNodes[i].north,  allNodes[i].east,  allNodes[i].south,  allNodes[i].west);

            if(roomType[i] == RoomType.STORE_ROOM_CODE){
                GameObject prefab = GameObject.Instantiate(Resources.Load<GameObject>("Mapa/MapGen/Prefabs/Store_1"));
                prefab.transform.parent = newRoom.transform; // Set as child            
                prefab.transform.localPosition = new Vector3(0,0,0);    
            }
            else if(roomType[i] == RoomType.CHEST_ROOM_CODE){
                GameObject prefab = GameObject.Instantiate(Resources.Load<GameObject>("Mapa/MapGen/Prefabs/Chest_1"));
                prefab.transform.parent = newRoom.transform; // Set as child            
                prefab.transform.localPosition = new Vector3(0,0,0);  
            }
            else if(roomType[i] == RoomType.END_ROOM_CODE){
                GameObject prefab = GameObject.Instantiate(Resources.Load<GameObject>("Mapa/MapGen/Prefabs/EndRoom_1"));
                prefab.transform.parent = newRoom.transform; // Set as child            
                prefab.transform.localPosition = new Vector3(0,0,0);  
            }

            /*

            
            System.Random rand = new System.Random();
            GameObject randRoomPrefab = null;
            // Add a room Obstacle prefab
            if(roomType[i] == RoomType.NORMAL_ENEMY_ROOM_CODE){
                randRoomPrefab = GameObject.Instantiate(roomPrefab18x18[rand.Next(0,roomPrefab18x18.Length)]); 
            }
            else if(roomType[i] == RoomType.LARGE_ENEMY_ROOM_CODE){
                randRoomPrefab = GameObject.Instantiate(roomPrefab26x26[rand.Next(0,roomPrefab26x26.Length)]); 
            }
            else if(roomType[i] == RoomType.STRECHED_ENEMY_ROOM_V_CODE){
                randRoomPrefab = GameObject.Instantiate(roomPrefab18x26[rand.Next(0,roomPrefab18x26.Length)]); 
            }
            else if(roomType[i] == RoomType.STRECHED_ENEMY_ROOM_H_CODE){
                randRoomPrefab = GameObject.Instantiate(roomPrefab26x18[rand.Next(0,roomPrefab26x18.Length)]); 
            }

            if(randRoomPrefab != null){
                randRoomPrefab.transform.position = new Vector3(x + width/2,y + length/2,0);
                randRoomPrefab.transform.parent = newRoom.transform;
            }

            
            // Create node Matrix
            roomScript.createMatrix(width, length, randRoomPrefab);

            */

            
        }
    }
}





















// UNUSED -> HAVE TO IMPLENT STUFF FROM HERE TO THE JOB SYSTEM THING STUFF



/*


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

    private void setCorridorBooleans(NodeStruct node, Room roomScript, NativeList<NodeStruct> nodeStructList){
        // Set corridors for childre
        foreach(NodeStruct childNode in node.neighbors(grid, gridSize, nodeStructList)){
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
*/
public static class RoomType{
    // Enemy Room Type codes
    public const int START_ROOM_CODE = 0;
    public const int END_ROOM_CODE = 1;
    public const int CHEST_ROOM_CODE = 2;
    public const int HEALING_ROOM_CODE = 3;
    public const int STORE_ROOM_CODE = 4;
    public const int NORMAL_ENEMY_ROOM_CODE = 5;
    public const int LARGE_ENEMY_ROOM_CODE = 6;
    public const int STRECHED_ENEMY_ROOM_V_CODE = 7;
    public const int STRECHED_ENEMY_ROOM_H_CODE = 8;

    // Room sizes

    // PLEASE MAKE SURE ROOM-SIZE-4 IS DIVISIBLE BY 2 ( (..._ROOM_(SIZE/WIDTH/LENGTH) - 4 ) % 2 == 0)
    public const int MAX_ROOM_SIZE = 26;
    public const int NORMAL_CORRIDOR_LENGTH = 10;
    public const int LARGE_ROOM_CORRIDOR_LENGTH = 6;
    public const int CORRIDOR_WIDTH = 4;
    public const int NORMAL_ROOM_WIDTH = 18;
    public const int NORMAL_ROOM_LENGTH = 18;

    public const int LARGE_ROOM_WIDTH = 26;
    public const int LARGE_ROOM_LENGTH = 26;
}


