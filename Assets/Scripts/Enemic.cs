using System.Collections.Generic;
using UnityEngine;

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
    private Pathfinding pathfinding;
    private Movement movement;
    private EnemyAnimatorController animController;

    public int RangDeVisio = 6;
    private int[] localitzat = { 0, 0 };
    private bool isFollowing = false;
    private List<Vector2> pathVectorList;




    // Start is called before the first frame update
    void Start()
    {
        animController = gameObject.GetComponent<EnemyAnimatorController>();
        maze = MazeMaker.Instance;
        pathfinding = Pathfinding.Instance;
        movement = Movement.Instance;
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

    public int[] PosicioJugador()
    {
        int[] posPlayer = new int[] { (int)movement.transform.position.x, (int)movement.transform.position.y };
        return posPlayer;
    }

    private bool playerFound(int[] aRevisar)
    {
        int[] posPlayer = PosicioJugador();
        return aRevisar[0] == posPlayer[0] && aRevisar[1] == posPlayer[1];
    }

    private void actualitzaLocalitzat()
    {
        localitzat = PosicioJugador();
    }

    public void ChangeState(EnemyStates newState = EnemyStates.Idle)
    {
        state = newState;
        animController.ChangeAnimationState();
    }

    private void vigilancia(int n)
    {
        int[] posAct = PosicioEnemic();
        bool transitable = true;
        bool viaLliure = true;
        int i = 0;
        int[] aRevisar = posibleMoviment(n);
        while (i < RangDeVisio && transitable && viaLliure)
        {
            if (!maze.EsViable(aRevisar[0], aRevisar[1]))
            {
                transitable = false;
            }

            else {
                int[] auxRevisor = aRevisar;
                aRevisar = rangDeVigilancia(n, auxRevisor);
            }
             if (playerFound(aRevisar)) { viaLliure = false;}
            i++;
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

    public int[] PosicioEnemic()
    {
        int[] posActual = (int[])posicio.Clone();
        return posActual;
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

    private int[] rangDeVigilancia(int n, int[] pos)
    {
        int[] novaposicio = pos;

        if (n == 1) novaposicio[0] = novaposicio[0] - 1;
        else if (n == 2) novaposicio[1] = novaposicio[1] + 1;
        else if (n == 3) novaposicio[0] = novaposicio[0] + 1;
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

    private int Direccio(int x, int y)
    {
        int[] posActual = PosicioEnemic();
        if (x < posActual[0]) return 1;
        else if (y > posActual[1]) return 2;
        else if (x > posActual[0]) return 3;
        else if (y < posActual[1]) return 4;
        else return 0;
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

    private void Patrulla()
    {
        obternirNovaDireccio();
        int[] novaPosAux = posibleMoviment(Direction);
        ChangeState();
        novaPosicio = new Vector2(novaPosAux[0], novaPosAux[1]);
        direccions = new List<int>() { 1, 2, 3, 4 };
    }

    private void newDirection(int x, int y)
    {
        Direction = Direccio(x, y);
    }

    private void Persegueix()
    {
        int[] novaPosAux = pathfinding.TrobarSegCasella(PosicioJugador(),PosicioEnemic());
        moveSpeed += 0.1032525f;
        newDirection(novaPosAux[0], novaPosAux[1]);
        ChangeState();
        novaPosicio = new Vector2(novaPosAux[0], novaPosAux[1]);
    }

    internal void Die()
    {
        Destroy(gameObject);
    }
}
