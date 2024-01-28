using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{

    private MazeMaker maze;
    private Movement movement;
    private Enemic enemic;
    private List<PathNode> openList;
    private HashSet<PathNode> closedList;
    private const int MOVE_DIAGONAL_COST = 9999999;
    private const int MOVE_STRAIGHT_COST = 10;

    private int[] posEnemic = { 0, 0 };
    private int[] posJugador = { 0, 0 };

    public Pathfinding()
    {
        maze = new MazeMaker();
    }

    private List<PathNode> FindPath(int[] posEnemic, int[] posJugador)
    {
        posJugador = enemic.posicioJugador();
        posEnemic = enemic.posicioEnemic();
        PathNode startNode = new PathNode(maze, posEnemic[0], posEnemic[1]);
        PathNode endNode = new PathNode(maze, posJugador[0], posJugador[1]);

        openList = new List<PathNode> { startNode };
        closedList = new HashSet<PathNode>();

        for (int i = 0; i < maze.tamany; i++)
        {
            for (int j = 0; j < maze.tamany; j++)
            {
                PathNode pathnode = new PathNode(maze, i, j);
                pathnode.gCost = int.MaxValue;
                pathnode.CalculateFCost();
                pathnode.cameFromNode = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(openList);
            if (currentNode == endNode)
            {
                //Hem arribat al node final
                return CalculatePath(endNode);
            }
            else
            {
                openList.Remove(currentNode);
                closedList.Add(currentNode);

                foreach (PathNode neightbouNode in GetNeighbourList(currentNode))
                {
                    if (closedList.Contains(neightbouNode)) continue;
                    else
                    {
                        int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neightbouNode);

                        if (tentativeGCost < currentNode.gCost)
                        {
                            neightbouNode.cameFromNode = currentNode;
                            neightbouNode.gCost = tentativeGCost;
                            neightbouNode.hCost = CalculateDistanceCost(neightbouNode, endNode);
                            neightbouNode.CalculateFCost();

                            if (!openList.Contains(neightbouNode)) openList.Add(neightbouNode);
                        }
                    }
                }
            }
        }
        //Ens hem quedat sens nodes que mirar i per tant no existeix un camí transitable
        return null;
    }

    private List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode> neighbourList = new List<PathNode>();

        int[] dx = { -1, 1, 0, 0 }; // Desplazamientos en x para izquierda, derecha, sin cambio
        int[] dy = { 0, 0, -1, 1 }; // Desplazamientos en y para arriba, abajo, sin cambio

        for (int i = 0; i < dx.Length; i++)
        {
            int newX = currentNode.x + dx[i];
            int newY = currentNode.y + dy[i];

            if (newX >= 0 && newX < maze.tamany && newY >= 0 && newY < maze.tamany)
            {
                neighbourList.Add(GetNode(newX, newY));
            }
        }

        return neighbourList;



        if (currentNode.x - 1 >= 0)
        {
            //Esquerra
            neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y));

            //Diagonal inferior esquerra
            if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y - 1));

            //Diagonal superior esquerra
            if (currentNode.y + 1 < maze.tamany) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y + 1));
        }
        if (currentNode.x + 1 < maze.tamany)
        {
            //Dreta
            neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y));

            //Diagonal inferior dreta
            if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y - 1));

            //Diagonal superior dreta
            if (currentNode.y + 1 < maze.tamany) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y + 1));
        }

        //Abaix
        if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x, currentNode.y - 1));

        //Adalt
        if (currentNode.y + 1 < maze.tamany) neighbourList.Add(GetNode(currentNode.x, currentNode.y + 1));

        return neighbourList;
    }

    private PathNode GetNode(int x, int y)
{
    PathNode node = new PathNode(maze, x, y);
    return node;
}


    private List<PathNode> CalculatePath(PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();
        path.Add(endNode);
        PathNode currentNode = endNode;
        while (currentNode.cameFromNode != null)
        {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }
        path.Reverse();
        return path;
    }

    private int CalculateDistanceCost(PathNode a, PathNode b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;

    }

    private PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostNode = pathNodeList[0];
        for (int i = 1; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = pathNodeList[i];
            }
        }
        return lowestFCostNode;
    }
}