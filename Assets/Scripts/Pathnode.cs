using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode : MonoBehaviour
{
    public MazeMaker maze;
    public int x;
    public int y;

    public int gCost;
    public int hCost;
    public int fCost;

    public PathNode cameFromNode;

    public PathNode(MazeMaker maze, int x, int y)
    {
        this.maze = maze;
        this.x = x;
        this.y = y;
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
