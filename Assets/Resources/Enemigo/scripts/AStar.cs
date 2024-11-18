using System;
using System.Collections.Generic;
using UnityEngine;

public class AStar
{
    public Node[,] nodeMatrix;
	public Player player;

	 // Priority Queue to store open nodes
	SortedSet<Node> openSet = new(new NodeComparer());
	HashSet<Node> closedSet = new();

	// G and F costs dictionaries
	Dictionary<Node, float> gCost = new();
	Dictionary<Node, float> fCost = new();

	// Parent dictionary for reconstructing the path
	Dictionary<Node, Node> cameFrom = new();

    public AStar(Node[,] nodeMatrix, Player player)
    {
        this.nodeMatrix = nodeMatrix;
		this.player = player;
    }

	public Node getNearestNode(GameObject p)
	{
		Vector3 playerPosition = p.transform.position;

		Node nearestNode = null;
		float minDistance = float.MaxValue;

		foreach (Node node in nodeMatrix)
		{
			Debug.Log("iter");
			if (node == null || !node.isWalkable) 
				continue;

			float distance = Vector3.Distance(playerPosition, new Vector3(node.x, node.y));

			if (distance < minDistance) {
				minDistance = distance;
				nearestNode = node;
			}
		}

		return nearestNode;
	}

    public List<Node> FindPath(Node start, Node goal)
    {
		// Empty everything
    	openSet.Clear();
		closedSet.Clear();
		gCost.Clear();
		fCost.Clear();
		cameFrom.Clear();

        // Initialize start node
        gCost[start] = 0;
        fCost[start] = start.getHeuristic(goal.x, goal.y);
        openSet.Add(start);

        while (openSet.Count > 0)
        {
            // Get node with lowest F cost
            Node current = GetNodeWithLowestFCost(openSet, fCost);

            if (current == goal)
                return ReconstructPath(cameFrom, current);

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (var neighbor in GetNeighbors(current))
            {
                if (!neighbor.isWalkable || closedSet.Contains(neighbor))
                    continue;

                float tentativeGCost = gCost[current] + current.getCost(neighbor);

                if (!gCost.ContainsKey(neighbor) || tentativeGCost < gCost[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gCost[neighbor] = tentativeGCost;
                    fCost[neighbor] = gCost[neighbor] + neighbor.getHeuristic(goal.x, goal.y);

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        // No path found
        return null;
    }

    private List<Node> GetNeighbors(Node node)
    {
        var neighbors = new List<Node>
        {
            node.north,
            node.northEast,
            node.east,
            node.southEast,
            node.south,
            node.southWest,
            node.west,
            node.northWest
        };

        // Remove null neighbors
        neighbors.RemoveAll(n => n == null);

        return neighbors;
    }

    private Node GetNodeWithLowestFCost(SortedSet<Node> openSet, Dictionary<Node, float> fCost)
    {
        Node lowestNode = null;
        float lowestCost = float.MaxValue;

        foreach (var node in openSet)
        {
            if (fCost.ContainsKey(node) && fCost[node] < lowestCost)
            {
                lowestCost = fCost[node];
                lowestNode = node;
            }
        }

        return lowestNode;
    }

    private List<Node> ReconstructPath(Dictionary<Node, Node> cameFrom, Node current)
    {
        var path = new List<Node>();
        while (cameFrom.ContainsKey(current))
        {
            path.Add(current);
            current = cameFrom[current];
        }
        path.Add(current);
        path.Reverse();
        return path;
    }
}

// Helper class for priority queue
public class NodeComparer : IComparer<Node>
{
    public int Compare(Node a, Node b)
    {
        if (a.x == b.x && a.y == b.y)
            return 0;
        return a.x == b.x ? a.y.CompareTo(b.y) : a.x.CompareTo(b.x);
    }
}
