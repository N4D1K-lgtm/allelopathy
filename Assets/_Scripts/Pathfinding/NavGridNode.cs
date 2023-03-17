using System;
using UnityEngine;

// Implement the Nodes
public class NavGridNode : IComparable<NavGridNode>
{
    // Each node has a position on the tilemap (this is equal to its key inside the dictionary)
    public Vector3Int position;

    // Initialize g, h and f cost, initialize a null parent reference to be used to return the path
    public bool traversable;
    public int gCost;
    public int hCost;
    public int fCost { get { return gCost + hCost; } }

    public NavGridNode parent;

    // Default empty Node
    public NavGridNode(Vector3Int pos, bool traverseabl)
    {
        traversable = traverseabl;
        position = pos;
        gCost = 0;
        hCost = 0;
        parent = null;
    }

    // CompareTo function for Min, Max functions
    public int CompareTo(NavGridNode other)
    {
        if (fCost < other.fCost)
        {
            return -1;
        }
        else if (fCost > other.fCost)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
}
