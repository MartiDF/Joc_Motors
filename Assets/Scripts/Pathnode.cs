using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathNode : MonoBehaviour
{
    public MazeMaker maze;
    public int x;
    public int y;

    public int gCost;
    public int hCost;
    public int fCost;
    public bool isWalkable;

    public PathNode cameFromNode;

    public PathNode(MazeMaker maze, int x, int y)
    {
        this.maze = maze;
        this.x = x;
        this.y = y;
        isWalkable = true;
    }

    public bool isItWalkable(int x, int y)
    {
        return maze.EstaBuida(x, y);
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public override string ToString()
    {
        return x + "," + y;
    }

}
