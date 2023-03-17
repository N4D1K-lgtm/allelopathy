using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class PathfindingController : MonoBehaviour
{
    
    // Create references to the tilemap, grid, start and target transfroms 
    public Tilemap tilemap;
    public Transform player_transform;
    public Transform enemy_transform;
    public Grid grid;
    public BoxCollider2D enemyBoxCollider;
    private Vector3 cellSize;
    
    // initialize a dictionary of nodes with Vector3Ints as keys
    private Dictionary<Vector3Int, Node> nodes = new Dictionary<Vector3Int, Node>();
    private List<Vector3Int> positionOffsetsWithinEnemy;

    // Implement the Nodes
    private class Node : IComparable<Node>
    {
        // Each node has a position on the tilemap (this is equal to its key inside the dictionary)
        public Vector3Int position;

        // Initialize g, h and f cost, initialize a null parent reference to be used to return the path
        public bool traversable;
        public int gCost;
        public int hCost;
        public int fCost { get { return gCost + hCost; } }
        public Node parent;
    
        // Default empty Node
        public Node(Vector3Int pos, bool traverseabl)
        {
            traversable = traverseabl;
            position = pos;
            gCost = 0;
            hCost = 0;
            parent = null;
        }

        // CompareTo function for Min, Max functions
        public int CompareTo(Node other)
        {
            if (fCost < other.fCost) {
                return -1;
            } else if (fCost > other.fCost) {
                return 1;
            } else {
                return 0;
            }
        }
    }

    void Start() { 
        

        cellSize = grid.cellSize;
        
        foreach (Vector3Int cellPos in tilemap.cellBounds.allPositionsWithin)
        {
            // Convert cell position to world position
            Vector3 cellWorldPos = tilemap.CellToWorld(cellPos);
            // Add node to the dictionary
            nodes[cellPos] = new Node(cellPos, tilemap.GetSprite(cellPos) == null);
        }

        positionOffsetsWithinEnemy = GetPositionOffsetsWithinBoxCollider(enemyBoxCollider, enemy_transform);
    }
    void OnDrawGizmos() {
        if (Application.isPlaying) {
            List<Node> path = FindPath(enemy_transform.position, player_transform.position, positionOffsetsWithinEnemy);
            // print(nodes[tilemap.WorldToCell(player_transform.position)].position);
            // Convert cell position to world position
            Gizmos.color = new Color(0, 0, 1, 0.5f);
            foreach (Node node in path) {
                Vector3 cellWorldPos = tilemap.CellToWorld(node.position);   
                Gizmos.DrawCube(cellWorldPos + cellSize / 2f, cellSize);
            }

            Gizmos.color = new Color(0, 1, 0, 0.5f);
            // Color cells within enemy's collider green
            foreach (Vector3Int cellOffset in positionOffsetsWithinEnemy)
            {
                Vector3Int cellPos = tilemap.WorldToCell(enemy_transform.position);
                Vector3Int enemyNodePosition = cellPos + cellOffset;
                Vector3 cellWorldPos = tilemap.CellToWorld(enemyNodePosition);
                Gizmos.DrawCube(cellWorldPos + cellSize / 2f, cellSize);

            }
        }
    }

    List<Node> FindPath(Vector3 Start, Vector3 Target, List<Vector3Int> positionOffsets) 
    {
        /*
        *
        * @param Start The starting position in world units.
        * @param Target The ending position in world units.
        * @param positionOffsets List of nodes to check referenced from the start node.
        *        To make it just check the single tile, use new List<Vector3Int> {new Vector3Int(0,0,0)}.
        * @return path The list of nodes that make the shortest path.
        *
        */

        // Create lists for open and closed nodes
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        // Get the start and end nodes
        Node startNode = nodes[tilemap.WorldToCell(Start)];
        Node endNode = nodes[tilemap.WorldToCell(Target)];

        // Add the start node to the open set
        openSet.Add(startNode);

        while (true) {
            // Get the node with the lowest fCost from the open set
            Node currentNode;
            if (openSet.Any()) {
                currentNode = openSet.Min();
            } else {
                // We couldn't find a path to the target, so just get as close as we can
                Node tryThisNode = closedSet.Where(node => node.traversable && node.parent != null).OrderBy(node => node.hCost).First();
                return RetracePath(startNode, tryThisNode);
            }
            
            // Remove the current node from the open set and add it to the closed set
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            // Debug log the current node and end node positions
            // Debug.Log("Current node: " + currentNode.position);
            // Debug.Log("End node: " + endNode.position);

            // If the current node is the end node, we've found the path
            if (currentNode.position == endNode.position)
            {
                List<Node> path = RetracePath(startNode, endNode);
                return path;
            }

            // Loop through the current node's neighbors
            foreach (Vector3Int neighborPos in GetNeighborPositions(currentNode.position))
            {
                Node neighborNode = nodes[neighborPos];

                // If the neighbor is in the closed set, or is not traversable skip it
                bool skip = false;
                // Check if it's in the closed set
                if (closedSet.Contains(neighborNode))
                {
                    skip = true;
                }

                // Check to make sure that it, and all the nodes within the collider, are traversable
                foreach (Vector3Int cellOffset in positionOffsets)
                {
                    Vector3Int testPosition = neighborNode.position + cellOffset;
                    if (nodes.ContainsKey(testPosition))
                    {
                        Node testNode = nodes[testPosition];
                        if (!testNode.traversable)
                        {
                            skip = true; break;
                        }
                        
                    }
                }
                if (skip)
                {
                    continue;
                }
                // Calculate the tentative gCost of the neighbor
                int tentativeGCost = currentNode.gCost + GetDistance(currentNode.position, neighborNode.position);

                // Check if the neighbor is not in the open set or if the tentative gCost is lower than the current gCost
                if (!openSet.Contains(neighborNode) || tentativeGCost < neighborNode.gCost) {
                    // Set the neighbor's parent to the current node
                    neighborNode.parent = currentNode;
                    // Update the neighbor's gCost and hCost
                    neighborNode.gCost = tentativeGCost;
                    neighborNode.hCost = GetDistance(neighborPos, endNode.position);
                    // Add the neighbor to the open set
                    if (!openSet.Contains(neighborNode)) {
                        openSet.Add(neighborNode);
                    }
                }
            }
        }
    }

    List<Vector3Int> GetNeighborPositions(Vector3Int position)
    {
        List<Vector3Int> neighborPositions = new List<Vector3Int>();

        // Check neighbors in all 8 directions
        Vector3Int[] directions = new Vector3Int[] { Vector3Int.left, Vector3Int.right, Vector3Int.up, Vector3Int.down,
                                                     Vector3Int.left + Vector3Int.up, Vector3Int.right + Vector3Int.up, 
                                                     Vector3Int.left + Vector3Int.down, Vector3Int.right + Vector3Int.down};
        foreach (Vector3Int direction in directions)
        {
            Vector3Int neighborPos = position + direction;
            // If the neighbor is within the tilemap and has a floor tile, add it to the list of neighbor positions
            if (nodes.ContainsKey(neighborPos))
            {
                neighborPositions.Add(neighborPos);
            }
        }

        return neighborPositions;
    }

    List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        // Add each node's parent to the path
        while (currentNode.position != startNode.position)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        // Reverse the path so it goes from start to end
        path.Reverse();

        return path;
    }
    int GetDistance(Vector3Int posA, Vector3Int posB)
    {
        int distX = Mathf.Abs(posA.x - posB.x);
        int distY = Mathf.Abs(posA.y - posB.y);
        int diagonalSteps = Mathf.Min(distX, distY);
        int straightSteps = Mathf.Abs(distX - distY);
        int dist = diagonalSteps * 14 + straightSteps * 10;
        return dist;
    }

    List<Vector3Int> GetPositionOffsetsWithinBoxCollider(BoxCollider2D boxCollider, Transform playerTransform)
    {
        List<Vector3Int> positionsWithinBoxCollider = new List<Vector3Int>();
        Vector3Int playerCellPos = tilemap.WorldToCell(playerTransform.position);

        foreach (Node node in nodes.Values)
        {
            // Convert cell position to world position
            Vector3 cellWorldPos = tilemap.CellToWorld(node.position);
            cellWorldPos += cellSize / 2f;

            // We don't care about the z-axis for finding the tiles, so force them to be the same
            cellWorldPos[2] = playerTransform.position[2];

            // Check if the world position is inside the box collider
            if (boxCollider.bounds.Contains(cellWorldPos))
            {
                Vector3Int positionWithinBox = node.position - playerCellPos; // Subtract the player's cell position from the node's position
                positionsWithinBoxCollider.Add(positionWithinBox);
            }
        }

        return positionsWithinBoxCollider;
    }
}