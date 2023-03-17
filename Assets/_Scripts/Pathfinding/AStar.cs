/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AStar : MonoBehaviour
{
    public Tilemap tilemap;
    private Dictionary<Vector3Int, Node> grid = new Dictionary<Vector3Int, Node>();
    private List<Node> open = new List<Node>();
    private List<Node> closed = new List<Node>();
    private Node startNode;
    private Node endNode;

    public Transform target;
    public Transform enemy;

    private void Start()
    {
        InitializeGrid();
    }

    private void Update()
    {
        // Call this function to update the positions of the player and enemies
        FindPath(tilemap.WorldToCell(enemy.position), tilemap.WorldToCell(target.position));
    }

    private void InitializeGrid()
    {
        // Create nodes for each tile in the tilemap and store them in a dictionary
        foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(pos);
            if (tile != null)
            {
                Node node = new Node(pos);
                grid.Add(pos, node);
                Debug.Log("Position of the Tile" + pos);
                Debug.Log("Node" + node.position.ToString());
                Debug.Log("Tile" + tile);
            }
        }
    }

    private void FindPath(Vector3Int Start, Vector3Int Target)
    {
        // Find the start and end nodes
        startNode = grid[Start];
        endNode = grid[Target];

        // Clear the open and closed lists
        open.Clear();
        closed.Clear();

        // Add the start node to the open list
        open.Add(startNode);

        // Loop until the end node is found or the open list is empty
        while (open.Count > 0)
        {
            // Get the node with the lowest f_cost from the open list
            Node current = open[0];
            for (int i = 1; i < open.Count; i++)
            {
                if (open[i].f_cost < current.f_cost)
                {
                    current = open[i];
                }
            }

            // Remove the current node from the open list and add it to the closed list
            open.Remove(current);
            closed.Add(current);

            // If the current node is the end node, the path has been found
            if (current == endNode)
            {
                // Construct the path by following the parent nodes
                List<Node> path = new List<Node>();
                Node node = current;
                while (node != null)
                {
                    path.Add(node);
                    node = node.parent;
                }
                path.Reverse();

                // Do something with the path
                foreach (Node tnode in path) { 
                    Debug.Log(tnode.position);
                }
                
                return;
            }

            // Check each neighbor of the current node
            foreach (Node neighbor in current.neighbors)
            {
                // Skip neighbors that are not traversable or are already in the closed list
                if (!neighbor.traversable || closed.Contains(neighbor))
                {
                    continue;
                }

                // Calculate the new g_cost and f_cost for the neighbor
                float tentative_g_cost =
                    current.g_cost + Vector3.Distance(current.position, neighbor.position);
                if (!open.Contains(neighbor))
                {
                    open.Add(neighbor);
                }
                else if (tentative_g_cost >= neighbor.g_cost)
                {
                    continue;
                }

                neighbor.g_cost = tentative_g_cost;
                neighbor.h_cost = Vector3.Distance(neighbor.position, endNode.position);
                neighbor.f_cost = neighbor.g_cost + neighbor.h_cost;
                neighbor.parent = current;
            }
        }
    }
}

private class Node
{
    public Vector3Int position;
    public bool traversable;
    public float g_cost;
    public float h_cost;
    public float f_cost;
    public Node parent;
    public List<Node> neighbors = new List<Node>();

    public Node(Vector3Int position)
    {
        this.position = position;
        traversable = true;
        g_cost = 0;
        h_cost = 0;
        f_cost = 0;
        parent = null;
    }
}
*/