using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemic : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 2.0f;
    protected EnemyStates state = EnemyStates.Idle;
    protected int direction = 0;

    private List<int> direccions = new List<int>(){1, 2, 3, 4};
    private int[] posicio = { 0, 0 };
    private Vector2 novaPosicio;

    private MazeMaker maze;

    // Start is called before the first frame update
    void Start()
    {
        maze = MazeMaker.Instance;
        posicio[0] = (int)transform.position.x;
        posicio[1] = (int)transform.position.y;
        novaPosicio = new Vector2(posicio[0], posicio[1]);
    }

    // Update is called once per frame
    void Update()
    {
        float distancia = Vector3.Distance(transform.position, novaPosicio);
        if (distancia > 0.0001f)
        {
            transform.position = Vector3.MoveTowards(transform.position, novaPosicio, moveSpeed * Time.deltaTime);
        }
        else { posicio[0] = (int)novaPosicio.x; posicio[1] = (int)novaPosicio.y; }
    }

    public void FerTorn()
    {
        // veure si s'ha de moure, si ha d'atacar, si esta atentitisimo
        Debug.Log("me muevo");
        Moure();

    }

    private void Moure() // Com punyetes se mou realment. Un misteri
    {
        obternirNovaDireccio();
        int[] novaPosAux = posibleMoviment(direction);
        novaPosicio = new Vector2(novaPosAux[0], novaPosAux[1]);
        Debug.Log(posicio[0]+", " + posicio[1]+"; i anem cap a "+novaPosicio);
        direccions = new List<int>() { 1, 2, 3, 4 };
    }

    private bool estaDisponible(int n)
    {
        int[] novaPosicio = posibleMoviment(n);

        return maze.EsViable(novaPosicio[0], novaPosicio[1]);
    }

    private int[] posibleMoviment(int n)
    {
        int[] novaposicio = (int[])posicio.Clone();

        if (n == 1) novaposicio[0] = novaposicio[0] - 1;
        else if (n == 2) novaposicio[1] = novaposicio[1] + 1;
        else if(n == 3) novaposicio[0] = novaposicio[0] + 1;
        else novaposicio[1] = novaposicio[1] - 1;

        return novaposicio;
    }

    private int DireccioContrari(int n)
    {
        if (n == 1) return 3;
        else if (n == 2) return 4;
        else if (n == 3) return 1;
        else return 2;
    }

    private void obternirNovaDireccio()
    {
        int direccio = 0;
        bool direccioDisponible = false;
        int direccioAnt = DireccioContrari(direction);
        if(direction != 0) direccions.Remove(DireccioContrari(direction));
        while (!direccioDisponible && direccions.Count > 0)
        {
            int j = UnityEngine.Random.Range(0, direccions.Count);
            direccio = direccions[j];
            if (!estaDisponible(direccio)) { Debug.Log("borrant direccio "+direccio); direccions.Remove(direccio); }
            else direccioDisponible = true;
        }
        if(direccions.Count == 0) direction = direccioAnt;
        else direction = direccio;
    }
}