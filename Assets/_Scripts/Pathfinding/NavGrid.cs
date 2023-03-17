using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public sealed class NavGrid : Singleton<NavGrid>
{
    /// <summary>
    ///     This is a class that generates a list of NavGridNodes for PathfindingControllers to use when seeking paths. This class should implement NavGrid updates, traversable blocks, a Gizmos api and expose a method called FindPath().
    ///     It inherits from Singleton so no more then one instance of NavGrid can exist at a time.
    /// </summary>

    [SerializeField] Tilemap _tilemap;
    [SerializeField] Grid _grid;

    [HideInInspector] public Vector3 _cellSize;

    private Dictionary<Vector3Int, NavGridNode> nodes = new Dictionary<Vector3Int, NavGridNode>();

    void Awake()
    {
        if (_tilemap == null) Debug.LogError("Tilemap not set in the Inspector", this);
        if (_grid = null) Debug.LogError("Grid not set in Inspector", this);

        _cellSize = _grid.cellSize;
    }
    void Start()
    {   
        // Initialize the grid of nodes from the tilemap
        foreach (Vector3Int cellPos in _tilemap.cellBounds.allPositionsWithin)
        {
            // Add node to the dictionary
            nodes[cellPos] = new NavGridNode(cellPos, _tilemap.GetSprite(cellPos) == null);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;

        // Iterate through all the cells in the Tilemap 
        foreach (Vector3Int cellPos in _tilemap.cellBounds.allPositionsWithin)
        {

            // Convert cell position to world position
            Vector3 cellWorldPos = _tilemap.CellToWorld(cellPos);


            // Draw the grid at the world position
            Gizmos.DrawWireCube(cellWorldPos + _cellSize / 2f, _cellSize);
        }
    }

    /*private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        
        
            // Draw a semitransparent red cube at the player's position
            if (_tilemap.WorldToCell(player_transform.position) == cellPos)
            {
                Gizmos.color = new Color(1, 0, 0, 0.5f);
                Gizmos.DrawCube(cellWorldPos + cellSize / 2f, cellSize);
                Gizmos.color = Color.white;
            }

            // Draw a semitransparent green cube at the enemy's position
            if (tilemap.WorldToCell(enemy_transform.position) == cellPos)
            {
                Gizmos.color = new Color(0, 1, 0, 0.5f);
                Gizmos.DrawCube(cellWorldPos + cellSize / 2f, cellSize);
                Gizmos.color = Color.white;
            }
        *//*
        List<NavGridNode> path = FindPath(_start.position, _target.transform.position);
        // print(nodes[_tilemap.WorldToCell(player_transform.position)].position);
        // Convert cell position to world position
        Gizmos.color = new Color(0, 0, 1, 0.5f);
        foreach (NavGridNode node in path)
        {
            Vector3 cellWorldPos = _tilemap.CellToWorld(node.position);
            Gizmos.DrawCube(cellWorldPos + _cellSize / 2f, _cellSize);*//*
        //}
    }*/

    public List<NavGridNode> FindPath(Vector3 start, Vector3 target)
    {
      
        /// <summary> Executes the A* pathfinding algorithm to find the shortest path to the target cell from the starting cell (requires a NavGrid instance in the scene)</summary>
        /// <param name="start"> The starting position in world units. </param>
        /// <param name="target"> The target position in world units. </param>
        /// <return> path The list of NavGridNodes that make the shortest path. </return>
     

        // Create lists for open and closed NavGridNodes
        List<NavGridNode> openSet = new List<NavGridNode>();
        HashSet<NavGridNode> closedSet = new HashSet<NavGridNode>();

        // Get the start and end NavGridNodes
        NavGridNode startNode = nodes[_tilemap.WorldToCell(start)];
        NavGridNode endNode = nodes[_tilemap.WorldToCell(target)];

        // Add the start node to the open set
        openSet.Add(startNode);

        while (true)
        {

            // Get the node with the lowest fCost from the open set
            NavGridNode currentNode;
            if (openSet.Any())
            {
                currentNode = openSet.Min();
            }
            else
            {
                Debug.Log("This is very bad.");
                return new List<NavGridNode>();
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
                List<NavGridNode> path = RetracePath(startNode, endNode);
                return path;
            }

            // Loop through the current node's neighbors
            foreach (Vector3Int neighborPos in GetNeighborPositions(currentNode.position))
            {
                NavGridNode neighborNode = nodes[neighborPos];

                // If the neighbor is in the closed set, skip it
                if (closedSet.Contains(neighborNode) || !neighborNode.traversable)
                {
                    continue;
                }

                // Calculate the tentative gCost of the neighbor
                int tentativeGCost = currentNode.gCost + GetDistance(currentNode.position, neighborNode.position);

                // Check if the neighbor is not in the open set or if the tentative gCost is lower than the current gCost
                if (!openSet.Contains(neighborNode) || tentativeGCost < neighborNode.gCost)
                {
                    // Set the neighbor's parent to the current node
                    neighborNode.parent = currentNode;
                    // Update the neighbor's gCost and hCost
                    neighborNode.gCost = tentativeGCost;
                    neighborNode.hCost = GetDistance(neighborPos, endNode.position);
                    // Add the neighbor to the open set
                    if (!openSet.Contains(neighborNode))
                    {
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
            // If the neighbor is within the _tilemap and has a floor tile, add it to the list of neighbor positions
            if (nodes.ContainsKey(neighborPos))
            {
                neighborPositions.Add(neighborPos);
            }
        }

        return neighborPositions;
    }

    List<NavGridNode> RetracePath(NavGridNode startNode, NavGridNode endNode)
    {
        List<NavGridNode> path = new List<NavGridNode>();
        NavGridNode currentNode = endNode;

        // Add each node's parent to the path
        while (currentNode.position != startNode.position)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        // Reverse the path so it goes from start to end
        path.Reverse();

        // Print the path
        foreach (NavGridNode node in path)
        {
            // Debug.Log(node.position);
        }
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
}
