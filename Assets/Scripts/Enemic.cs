using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.XR;

public class Enemic : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 3.0f;
    public EnemyStates state = EnemyStates.Idle;
    public int Direction { get; private set; } = 0;
    public bool StayAlive { get; private set; } = false;

    private List<int> direccions = new List<int>(){1, 2, 3, 4};
    private int[] posicio = { 0, 0 };
    private Vector2 novaPosicio;

    private MazeMaker maze;
    private Movement movement;
    private EnemyAnimatorController animController;

    public int RangDeVisio = 6;
    private int[] localitzat = { 0, 0 };
    private bool isFollowing = false;

    // Start is called before the first frame update
    void Start()
    {
        animController = gameObject.GetComponent<EnemyAnimatorController>();
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
        if (!isFollowing) {
            Patrulla();
            vigilancia(Direction);
        }
        else
        {
            actualitzaLocalitzat();
            Persegueix();
        }
        
    }

    public int[] posicioJugador()
    {
        int[] posPlayer = new int[] { movement.currentCellPosition.x, movement.currentCellPosition.y };
        return posPlayer;
    }

    private bool playerFound(int[] aRevisar)
    {
        int[] posPlayer = posicioJugador();
        return aRevisar == posPlayer;
    }

    private void actualitzaLocalitzat()
    {
        localitzat = posicioJugador();
    }

    public void ChangeState(EnemyStates newState = EnemyStates.Idle)
    {
        // Debug.Log($"S'ha canviat a l'estat {newState}");
        state = newState;
        animController.ChangeAnimationState();
    }

    private void vigilancia(int n)
    {
        bool transitable = true;
        bool viaLliure = true;
        int i = 0;
        int[] aRevisar = posibleMoviment(n);
        while (i < RangDeVisio && transitable && viaLliure)
        {
            if (!maze.EsViable(aRevisar[0], aRevisar[1])) transitable = false;
            else aRevisar = posibleMoviment(n);
            i++;
            if (playerFound(aRevisar)) viaLliure = false;
        }
        if (!viaLliure) {
            localitzat = aRevisar;
            isFollowing = true;
        }
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
        int direccioAnt = DireccioContrari(Direction);
        if(Direction != 0) direccions.Remove(DireccioContrari(Direction));
        while (!direccioDisponible && direccions.Count > 0)
        {
            int j = UnityEngine.Random.Range(0, direccions.Count);
            direccio = direccions[j];
            if (!estaDisponible(direccio)) {
                // Debug.Log("borrant direccio "+direccio);
                direccions.Remove(direccio);
            }
            else direccioDisponible = true;
        }
        if(direccions.Count == 0) Direction = direccioAnt;
        else Direction = direccio;
    }

    public void GetHit(bool playerDied = false)
    {
        ChangeState(EnemyStates.Attacking);
        StayAlive = playerDied;
    }

    private void Patrulla() // Com punyetes se mou realment. Un misteri
    {
        obternirNovaDireccio();
        int[] novaPosAux = posibleMoviment(Direction);
        ChangeState();
        novaPosicio = new Vector2(novaPosAux[0], novaPosAux[1]);
        // Debug.Log(posicio[0]+", " + posicio[1]+"; i anem cap a "+novaPosicio);
        direccions = new List<int>() { 1, 2, 3, 4 };
    }

    private void Persegueix()
    {
        throw new NotImplementedException();
    }

    internal void Die()
    {
        Destroy(gameObject);
    }
}
