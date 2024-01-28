using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pathfinding : MonoBehaviour
{
    public static Pathfinding Instance { get; private set; }

    private MazeMaker maze;
    private Tilemap mapa;

    private int[,] mapaDistancia;

    private Vector2 posEnemic;
    private Vector2 posJugador;

    public Pathfinding()
    {
        Instance = this;
    }

    private void Start()
    {
        maze = MazeMaker.Instance;
        mapa = maze.mapaTerreny;

        mapaDistancia = new int[maze.tamany, maze.tamany];
        
        var min = mapa.localBounds.min;
        for (int i = 0; i < maze.tamany; i++)
        {
            for (int j = 0; j < maze.tamany; j++)
            {
                int mapaX = (int)(i + min.x);
                int mapaY = (int)(j + min.y);

                var tile = mapa.GetTile(new Vector3Int(mapaX, mapaY, 0));
                if (tile.name.StartsWith("TileMur_")) mapaDistancia[i, j] = -1; // Si és un mur, posem un -1
                else mapaDistancia[i, j] = 0;
            }
        }
    }

    // Reseteja a 0 totes les caselles que han de tenir un 0 (terra)
    void NetejarMapaDistancia()
    {
        for (int i = 0; i < maze.tamany; i++)
        {
            for (int j = 0; j < maze.tamany; j++)
            {
                if (mapaDistancia[i, j] != -1)
                    mapaDistancia[i, j] = 0;
            }
        }
    }

    // Emplenem el mapa amb valors heurístics calculats arrel de les posicions de l'enemic i el jugador
    void EmplenarMapaDistancia(Vector3Int start, Vector3Int end)
    {
        // No podem ser a sobre d'un mur
        if (mapaDistancia[start.x, start.y] == -1 || mapaDistancia[end.x, end.y] == -1) return;

        int distancia = 1;
        Queue<Vector3Int> caselles = new Queue<Vector3Int>(); // Cua de caselles que hem d'anar mirant
        mapaDistancia[end.x, end.y] = 1; // L'objectiu té un valor 1
        caselles.Enqueue(end);

        // Mentre hi hagin caselles per visitar
        while(caselles.Count > 0)
        {
            distancia++;

            // Mirem les caselles al voltant de la casella actual
            Vector3Int casella = caselles.Dequeue();
            Visitar(caselles, distancia, casella.x, casella.y + 1);
            Visitar(caselles, distancia, casella.x, casella.y - 1);
            Visitar(caselles, distancia, casella.x + 1, casella.y);
            Visitar(caselles, distancia, casella.x - 1, casella.y);
        }
    }

    // Mira la casella i li aplica una distancia
    void Visitar(Queue<Vector3Int> caselles, int distancia, int x, int y)
    {
        // Aixo mai hauria de passar per com es el nostre mapa, pero ho mirarem de totes maneres
        if (x < 0 || x >= maze.tamany) return;
        if (y < 0 || y >= maze.tamany) return;

        // Si la casella no esta visitada, escriurem la distancia i la encuem
        if (mapaDistancia[x,y] == 0)
        {
            mapaDistancia[x,y] = distancia;
            caselles.Enqueue(new Vector3Int(x,y,0));
        }
    }

    // Observa les caselles colindants a casella i retorna la que tingui un valor més petit. D'aquesta manera, no hem de computar tota la ruta.
    int[] ObtenirCasella(Vector3Int casella)
    {
        int min = int.MaxValue;
        int xMin = -1, yMin = -1;
        int x = casella.x; int y = casella.y;

        // Mirem al voltant i tractem d'obtenir la menor distancia possible
        TrobarCasellaMinima(x, y+1, ref xMin, ref yMin, ref min);
        TrobarCasellaMinima(x, y-1, ref xMin, ref yMin, ref min);
        TrobarCasellaMinima(x+1, y, ref xMin, ref yMin, ref min);
        TrobarCasellaMinima(x-1, y, ref xMin, ref yMin, ref min);

        // Enviem la casella en coordenades de món
        Vector3 global = ToGlobal(new Vector3Int(xMin, yMin));

        return new int[] { (int)global.x, (int)global.y };
    }

    // Actualitza xMin, yMin i min amb els valors corresponents si la casella actual té una distancia menor que (xMin,yMin)
    void TrobarCasellaMinima(int xAct, int yAct, ref int xMin, ref int yMin, ref int min)
    {
        int distancia = mapaDistancia[xAct, yAct];
        if (distancia == -1) return;
        if (min == int.MaxValue || distancia < min) 
        { 
            xMin = xAct; 
            yMin = yAct;
            min = distancia; 
        }
    }

    // Retorna la casella a la que ha d'anar l'enemic
    public int[] TrobarSegCasella(int[] posJ, int[] posE)
    {
        // Passem les posicions de món a coordenades del mapa de distancia
        Vector3Int start = ToLocal(new Vector3(posE[0], posE[1], 0));
        Vector3Int end = ToLocal(new Vector3(posJ[0], posJ[1], 0));

        NetejarMapaDistancia();
        EmplenarMapaDistancia(start, end);

        // Nosaltres no volem trobar tota la ruta, només la primera casella a la que ens hem de moure!
        return ObtenirCasella(start);
    }


    Vector3Int ToLocal(Vector3 World)
    {
        var local = World - mapa.transform.position - mapa.localBounds.min;

        return new Vector3Int((int)local.x - 1, (int)local.y - 1, 0);
    }

    Vector3 ToGlobal(Vector3Int local)
    {
        var f_local = new Vector3(local.x + 1, local.y + 1, 0);

        return f_local + transform.position + mapa.localBounds.min;
    }
}