using UnityEngine;
using UnityEngine.Tilemaps;

public class Pathfinding : MonoBehaviour
{
    public Tilemap tilemap;
    public Transform player_transform;
    public Transform enemy_transform;
    public Grid grid;
    private Vector3 cellSize;

    

    void Start()
    {
        cellSize = grid.cellSize;
        foreach (Vector3Int cellPos in tilemap.cellBounds.allPositionsWithin)
        {
            // Convert cell position to world position
            Vector3 cellWorldPos = tilemap.CellToWorld(cellPos);

        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        // Iterate through all the cells in the Tilemap 
        foreach (Vector3Int cellPos in tilemap.cellBounds.allPositionsWithin)
        {

            // Convert cell position to world position
            Vector3 cellWorldPos = tilemap.CellToWorld(cellPos);   

            // Draw a semitransparent red cube at the player's position
            if (tilemap.WorldToCell(player_transform.position) == cellPos) {
                Gizmos.color = new Color(1, 0, 0, 0.5f);
                Gizmos.DrawCube(cellWorldPos + cellSize / 2f, cellSize);
                Gizmos.color = Color.white;
            }

            // Draw a semitransparent green cube at the enemy's position
            if (tilemap.WorldToCell(enemy_transform.position) == cellPos) {
                Gizmos.color = new Color(0, 1, 0, 0.5f);
                Gizmos.DrawCube(cellWorldPos + cellSize / 2f, cellSize);
                Gizmos.color = Color.white;
            }

            // Draw the grid at the world position
            Gizmos.DrawWireCube(cellWorldPos + cellSize / 2f, cellSize);
        }
    }

    void Update() {

    }
     
}