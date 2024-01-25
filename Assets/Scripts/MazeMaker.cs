using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MazeMaker : MonoBehaviour
{
 

    [SerializeField] public UnityEngine.Tilemaps.Tile[] tileSet;
    [SerializeField] public UnityEngine.Tilemaps.Tile[] tileSet_Terra;
    [SerializeField] public UnityEngine.Tilemaps.Tile[] tileSetConsumibles;
    [SerializeField] public Tilemap mapaConsumibles;
    [SerializeField] public Tilemap mapaTerreny;

    [SerializeField] public GameObject Consumibles;
    [SerializeField] public GameObject Espassa;
    [SerializeField] public GameObject Cofre;
    [SerializeField] public GameObject Final;
    [SerializeField] public GameObject Enemic;
    private GameObject _triggers;
    public char[,] Maze { private set; get; }

    [Range(1, 10000000)]    public int seed = 0;
    private Tilemap _mapa;
    public bool randomSeed;
    private int OffsetIntern = 3;
    int offset = 0;
    int inOffset = 10;
    int SpawnX, SpawnY, FiX, FiY;

    /*DEBUG*/
    [Range(20, 1000)]    public int tamany = 50;
    [Range(0, 70)]       public int MesCamins;
    [Range(0, 100)]      public int TrencarCami;
    [Range(1, 20000)]    public int iteracions = 1000;
    [Range(0, 10)]       public int consumibleStamina;
    [Range(0, 10)]       public int enemicsProb;
    [Range(0, 10)]       public int consumibleEspasa;
    [Range(1, 10)]       public int OffsetInternSapwnFi;
    int mescam = 0;

    private List<Enemic> enemics = new List<Enemic>();
    private GameObject tresor;
    private Vector2 posTresor;

    private char[] dic = { ' ', 'q', 'w', 'e', 'r', 't', 'y', 'u', 'i', 'o', 'p', 'a', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'z', 'x', 'c', 'v', 'b', 'n', 'm', ',', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', 'Q', 'W', 'E', 'R', 'T', 'Y', 'U', 'I', 'O', 'P', 'A', 'S', '_', '#' };
    private char[] dic2 = { 'D', 'F', 'G', 'H', 'J', 'K', 'L', 'Z', 'X', 'C', 'V', 'B', 'N', 'M', '!', '"', '$', '%', '&', '?', '/' };

    // La instancia estática de la clase
    private static MazeMaker _instance;

    // Propiedad pública para acceder a la instancia
    public static MazeMaker Instance
    {
        get
        {
            // Si la instancia no existe, créala
            if (_instance == null)
            {
                _instance = FindObjectOfType<MazeMaker>();

                // Si no se encontró una instancia en la escena, crea un nuevo GameObject y agrega el script
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(MazeMaker).Name);
                    _instance = singletonObject.AddComponent<MazeMaker>();
                }
            }

            // Devuelve la instancia
            return _instance;
        }
    }

    // Opcional: Puedes incluir los miembros regulares de tu clase MazeMaker aquí

    private void Awake()
    {
        // Asegúrate de que solo haya una instancia de esta clase
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }

        // Establece la instancia
        _instance = this;

        // Opcional: Asegúrate de que el singleton sobreviva a los cambios de escena
        DontDestroyOnLoad(gameObject);
    }

    public int getRelativeSpawnX()
    {
        return SpawnX-(tamany/2);
    }
    public int getRelativeSpawnY()
    {
        return SpawnY - (tamany / 2);
    }

    public int getRelativeCoor(int coor)
    {
        return coor - offset;
    }
   
    public int getOffset()
    {
        return offset;
    }

    public void Start(){
        _mapa = transform.Find("Grid").transform.Find("Tilemap").GetComponent<Tilemap>();
        _triggers = GameObject.Find("Triggers");

        StartGen();
    }

    void Update()
    {
        if (MesCamins != mescam)
        {
            Maze = new char[tamany, tamany];
            mescam = MesCamins;
            Generar();
        }
    }

    public void StartGen()
    {

        tamany += 15;
        offset = tamany / 2;
        inOffset = 15 + OffsetIntern;
        mescam = MesCamins;

        if (randomSeed)
            seed = UnityEngine.Random.Range(0, 10000000);
        
        UnityEngine.Random.InitState(seed);

        Maze = new char[tamany, tamany];
        Generar();
    }
    
    void Generar() 
    {
        clearMap(Maze); //Debugger
        FillMap(Maze);  
        Maker(Maze);
        prettyMap(Maze);
        GenerarConsumibles(Maze);
        GenerarEnemnics(Maze);
        TilMap(Maze);
        GenerarSpawnIFinal(Maze, ref SpawnX,ref SpawnY,ref FiX,ref FiY,OffsetInternSapwnFi);
        PrintMap(Maze);
    }

    void FillMap(char[,] map)
    {

        for (int i = 0; i < tamany; i++)
        {
            for (int j = 0; j < tamany; j++)
            {
                map[i, j] = '#';
            }
        }
    }

    void Maker(char[,] map)
    {
        map[tamany / 2, tamany / 2] = ' ';
        MakerRec1(map, tamany / 2, (tamany / 2) - 1, 0, iteracions);
    }

    void GenerarSpawnIFinal(char[,] map, ref int SpawnX, ref int SpawnY, ref int FiX, ref int FiY,int OffsetInternSapwnFi)
    {
        int spawnCosat = (int)(UnityEngine.Random.value * 10) % 4;
        int fiCosat = (int)(UnityEngine.Random.value * 10) % 4;

        while (spawnCosat == fiCosat) {
            fiCosat = (int)(UnityEngine.Random.value * 10) % 4;
        }

        //començar per S
        bool primer = true;
        
        for (int y = inOffset - OffsetInternSapwnFi; y < tamany - inOffset - OffsetInternSapwnFi; y++)
        {
            for (int x = inOffset - OffsetInternSapwnFi; x < tamany - inOffset - OffsetInternSapwnFi; x++)
            {

                if (map[x, y] == 'q' && primer)
                {
                    primer = false;
                    SpawnX = x; SpawnY = y;
                    map[SpawnX, SpawnY] = '?'; // Aqui per posar tile al spawn
                    mapaConsumibles.SetTile(new Vector3Int(x - offset, y - offset, 0), tileSetConsumibles[0]);
                    GameObject nouTrigger = Instantiate(Final, mapaConsumibles.GetCellCenterWorld(mapaConsumibles.WorldToCell(new Vector3Int(x - offset, y - offset, 0))), Quaternion.identity);
                    nouTrigger.transform.parent = _triggers.transform;
                }
                else if (map[x, y] == 'q')
                {
                    FiX = x; FiY = y;
                }
            }
        }

        if(SpawnX != FiX && SpawnY != FiY)
        {
            map[FiX, FiY] = '?';
            mapaConsumibles.SetTile(new Vector3Int(FiX - offset, FiY - offset, 0), tileSetConsumibles[1]);
            tresor = Instantiate(Cofre, mapaConsumibles.GetCellCenterWorld(mapaConsumibles.WorldToCell(new Vector3Int(FiX - offset, FiY - offset, 0))), Quaternion.identity);
            tresor.transform.parent = _triggers.transform;
            posTresor = new Vector2(tresor.transform.position.x, tresor.transform.position.y);
        }
    }

    void GenerarConsumibles(char[,] map)
    {
        for (int Y = inOffset - 3; Y < tamany - inOffset + 3; Y++)
        {

            for (int X = inOffset - 3; X < tamany - inOffset + 3; X++)
            {
                if (map[X, Y] == ' ')
                {
                    if ((int)(UnityEngine.Random.value * 1000) % 1000 <= consumibleStamina)
                    {
                        mapaConsumibles.SetTile(new Vector3Int(X - offset, Y - offset, 0), tileSetConsumibles[2]);
                        GameObject nouTrigger = Instantiate(Consumibles, mapaConsumibles.GetCellCenterWorld(mapaConsumibles.WorldToCell(new Vector3Int(X - offset, Y - offset, 0))), Quaternion.identity);
                        nouTrigger.transform.parent = _triggers.transform;
                    }
                    else
                    {
                        if ((int)(UnityEngine.Random.value * 1000) % 1000 <= consumibleEspasa)
                        {
                            mapaConsumibles.SetTile(new Vector3Int(X - offset, Y - offset, 0), tileSetConsumibles[3]);
                            GameObject nouTrigger = Instantiate(Espassa, mapaConsumibles.GetCellCenterWorld(mapaConsumibles.WorldToCell(new Vector3Int(X - offset, Y - offset, 0))), Quaternion.identity);
                            nouTrigger.transform.parent = _triggers.transform;
                            
                        }
                    }
                }
            }
        }
    }

    void GenerarEnemnics(char[,] map)
    {
        for (int Y = inOffset - 3; Y < tamany - inOffset + 3; Y++)
        {
            
            for (int X = inOffset - 3; X < tamany - inOffset + 3; X++)
            {
                if (map[X, Y] == ' ')
                {
                    if ((int)(UnityEngine.Random.value * 1000) % 1000 <= enemicsProb)
                    {
                        GameObject enemicNou = Instantiate(Enemic, mapaTerreny.GetCellCenterWorld(mapaTerreny.WorldToCell(new Vector3Int(X - offset, Y - offset, -1))), Quaternion.identity);
                        Debug.Log(enemicNou.transform.position);
                        enemicNou.transform.parent = _triggers.transform;
                        enemics.Add(enemicNou.GetComponent<Enemic>());
                    }                   
                }
            }
        }
    }

    public void FerTornEnemics()
    {
        foreach (Enemic enemic in enemics)
        {
            if(enemic != null) enemic.FerTorn();
        }
    }

    void MakerRec1(char[,] map, int NextX, int NextY, int dir, int iteracio)
    {

        if (NextX >= inOffset - 1 && NextX <= tamany - inOffset && NextY >= inOffset - 1 && NextY <= tamany - inOffset)
        {

            while (esCorrecte(map, NextX, NextY, dir) && iteracio > 0)
            {
                iteracio--;
                map[NextX, NextY] = ' ';
                //if (UnityEngine.Random.Range(1, 100) <= TrencarCami)

                    if ((int)(UnityEngine.Random.value* 100) % 100 <= MesCamins)
                {
                    int newDir = dir;
                    int auxDir = newDir;

                    int newNextX = NextX;
                    int newNextY = NextY;
                    int newNextDir = dir;

                    newDir = (int)(UnityEngine.Random.value*10) % 2;
                    if (dir == 0 || dir == 2)
                    {
                        if (newDir == 0 && esCorrecte(map, NextX + 1, NextY, 1))
                        {
                            newNextX = NextX + 1;
                            newNextDir = 1;
                        }
                        else if (newDir == 1 && esCorrecte(map, NextX - 1, NextY, 3))
                        {
                            newNextX = NextX - 1;
                            newNextDir = 3;
                        }
                    }
                    else if (dir == 1 || dir == 3)
                    {
                        if (newDir == 0 && esCorrecte(map, NextX, NextY - 1, 0))
                        {
                            newNextY = NextY - 1;
                            newNextDir = 0;
                        }
                        else if (newDir == 1 && esCorrecte(map, NextX, NextY + 1, 2))
                        {
                            newNextY = NextY + 1;
                            newNextDir = 2;
                        }
                    }

                    MakerRec1(map, newNextX, newNextY, newNextDir, iteracio);


                }

                switch (dir)
                {
                    case 0:
                        NextY -= 1;
                        break;
                    case 1:
                        NextX += 1;
                        break;
                    case 2:
                        NextY += 1;
                        break;
                    case 3:
                        NextX -= 1;
                        break;


                }

            }




        }
    }

    bool esCorrecte(char[,] map, int x, int y, int dir)
    {
        bool totOk = false;
        bool trencar = false;
        bool dinsLimits = x >= inOffset - 1 && x <= tamany - inOffset && y >= inOffset - 1 && y <= tamany - inOffset;

        if (dinsLimits)
        {

            //if (UnityEngine.Random.Range(1, 100) <= TrencarCami)/// mirar aixo que no mo acabo de creure 
            if ((int)(UnityEngine.Random.value* 100) % 100 <= TrencarCami)/// mirar aixo que no mo acabo de creure 
                trencar = true;



            if (dir != 1 && dir != 3)
            {
                if (map[x + 1, y] != ' ' && map[x - 1, y] != ' ')
                {

                    if (dir == 0)
                    {
                        totOk = map[x + 1, y - 1] != ' ' && map[x - 1, y - 1] != ' '; // 0

                        if (trencar && map[x + 1, y - 1] == ' ' && map[x - 1, y - 1] == ' ' && map[x, y - 1] == ' ')
                        {
                            totOk = true; // 0
                        }
                    }
                    else
                    {
                        totOk = map[x + 1, y + 1] != ' ' && map[x - 1, y + 1] != ' '; // 2
                        if (trencar && map[x + 1, y + 1] == ' ' && map[x - 1, y + 1] == ' ' && map[x, y + 1] == ' ')
                        {
                            totOk = true;
                        }
                    }
                }

            }
            else if (dir != 0 && dir != 2)
            {
                if (map[x, y + 1] != ' ' && map[x, y - 1] != ' ')
                {
                    if (dir == 1)
                    {
                        totOk = map[x + 1, y - 1] != ' ' && map[x + 1, y + 1] != ' ';
                        if (trencar && map[x + 1, y - 1] == ' ' && map[x + 1, y + 1] == ' ' && map[x + 1, y] == ' ')
                        {
                            totOk = true;
                        }
                    }
                    else
                    {
                        totOk = map[x - 1, y + 1] != ' ' && map[x - 1, y - 1] != ' ';

                        if (trencar && map[x - 1, y + 1] == ' ' && map[x - 1, y - 1] == ' ' && map[x - 1, y] == ' ')
                        {
                            totOk = true;
                        }
                    }
                }

            }


        }

        return totOk;
    }

    void prettyMap(char[,] map)
    {

        char[,] MazeAux = (char[,])map.Clone();


        // Fer copia del mapa, mirar la copia i modificar la original,
        for (int y = inOffset - 1; y < tamany - inOffset; y++)
        {
            for (int x = inOffset - 1; x < tamany - inOffset; x++)
            {
                if (map[x, y] == ' ')
                {

                    if (MazeAux[x - 1, y] == '#' && MazeAux[x - 1, y - 1] == '#' && MazeAux[x, y - 1] == '#' && MazeAux[x + 1, y - 1] == '#' && MazeAux[x + 1, y] == '#') //0
                    {
                        map[x, y] = '#';
                    }
                    else

                    if (MazeAux[x, y - 1] == '#' && MazeAux[x + 1, y + 1] == '#' && MazeAux[x + 1, y] == '#' && MazeAux[x + 1, y + 1] == '#' && MazeAux[x, y + 1] == '#') // 1
                    {
                        map[x, y] = '#';
                    }
                    else

                    if (MazeAux[x + 1, y] == '#' && MazeAux[x + 1, y + 1] == '#' && MazeAux[x, y + 1] == '#' && MazeAux[x - 1, y + 1] == '#' && MazeAux[x - 1, y] == '#') // 2
                    {
                        map[x, y] = '#';
                    }
                    else

                    if (MazeAux[x, y - 1] == '#' && MazeAux[x - 1, y - 1] == '#' && MazeAux[x - 1, y] == '#' && MazeAux[x - 1, y + 1] == '#' && MazeAux[x, y + 1] == '#') // 3
                    {
                        map[x, y] = '#';
                    }

                }
            }
        }
    }

    void clearMap(char[,] map)
    {

        for (int i = 0; i < tamany; i++)
        {
            for (int j = 0; j < tamany; j++) // es port fer amb un switch
            {
                //mapaMurs.SetTile(new Vector3Int(i - offset, j - offset, 0), null);
                _mapa.SetTile(new Vector3Int(i - offset, j - offset, 0), null);                
            }
        }
    }

    void PrintMap(char[,] map)
    {

        for (int i = 0; i < tamany; i++)
        {
            for (int j = 0; j < tamany; j++)
            {
                char cod = map[i, j];
                int index = Array.IndexOf(dic, cod);
                
                if(map[i, j] != ' ' && index != -1)
                    _mapa.SetTile(new Vector3Int(i - offset, j - offset, 0), tileSet[index]);

                char cod2 = map[i, j];
                int index2 = Array.IndexOf(dic2, cod2);
                if(index2 != -1)
                    _mapa.SetTile(new Vector3Int(i - offset, j - offset, 0), tileSet_Terra[index2]);
                
               
                
            }
        }
        
    }

    void TilMap(char[,] map)
    {
        char[,] Maze = (char[,])map.Clone();

        for (int Y = inOffset - 3; Y < tamany - inOffset + 3; Y++)
        {
            for (int X = inOffset - 3; X < tamany - inOffset + 3; X++)
            {
                if (Maze[X, Y] == '#')
                    switch (idCela(Maze, X, Y))
                    {

                        case 0://1
                            map[X, Y] = 'q';
                            break;
                        case 16:
                        case 24:
                        case 48:
                        case 56://2
                            map[X, Y] = 'w';
                            break;
                        case 64:
                        case 96:
                        case 192:
                        case 224://3
                            map[X, Y] = 'e';
                            break;
                        case 1:
                        case 3://4
                        case 129:
                        case 131://4
                            map[X, Y] = 'r';
                            break;
                        case 4:
                        case 6:
                        case 12:
                        case 14://5
                            map[X, Y] = 't';
                            break;
                        case 85://6
                            map[X, Y] = 'y';
                            break;
                        case 80:
                        case 88:
                        case 208:
                        case 216://7
                            map[X, Y] = 'u';
                            break;
                        case 65:
                        case 67:
                        case 97:
                        case 99://8
                            map[X, Y] = 'i';
                            break;
                        case 5:
                        case 13:
                        case 133:
                        case 141://9
                            map[X, Y] = 'o';
                            break;
                        case 20:
                        case 22:
                        case 52:
                        case 54://10
                            map[X, Y] = 'p';
                            break;
                        case 68:
                        case 70:
                        case 76:
                        case 78:
                        case 100:
                        case 102:
                        case 108:
                        case 110:
                        case 196:
                        case 198:
                        case 204:
                        case 206:
                        case 228:
                        case 230:
                        case 236:
                        case 238://11
                            map[X, Y] = 'a';//11
                            break;
                        case 17:
                        case 19:
                        case 25:
                        case 27:
                        case 49:
                        case 51:
                        case 57:
                        case 59:
                        case 145:
                        case 147:
                        case 155:
                        case 153:
                        case 177:
                        case 179:
                        case 185:
                        case 187://12
                            map[X, Y] = 's';//12
                            break;
                        case 69:
                        case 101:
                        case 77:
                        case 109:
                            map[X, Y] = 'g';//15
                            break;
                        case 84:
                        case 86:
                        case 212:
                        case 214:
                            map[X, Y] = 'h';//16
                            break;
                        case 81:
                        case 83:
                        case 89:
                        case 91:
                            map[X, Y] = 'j';//17
                            break;
                        case 21:
                        case 53:
                        case 149:
                        case 181:
                            map[X, Y] = 'k';//18
                            break;
                        case 112:
                        case 120:
                        case 240:
                        case 248:
                            map[X, Y] = 'l';//19
                            break;
                        case 193:
                        case 195:
                        case 225:
                        case 227:
                            map[X, Y] = 'z';//20
                            break;
                        case 7:
                        case 15:
                        case 135:
                        case 143:
                            map[X, Y] = 'x';//21
                            break;
                        case 28:
                        case 30:
                        case 60:
                        case 62:
                            map[X, Y] = 'c';//22
                            break;
                        case 124:
                        case 126:
                        case 252:
                        case 254:
                            map[X, Y] = 'v';//23
                            break;
                        case 241:
                        case 243:
                        case 249:
                        case 251:
                            map[X, Y] = 'b';//24
                            break;
                        case 199:
                        case 207:
                        case 231:
                        case 239:
                            map[X, Y] = 'n';//25
                            break;
                        case 31:
                        case 63:
                        case 159:
                        case 191:
                            map[X, Y] = 'm';//26
                            break;
                        case 255:
                            map[X, Y] = ',';//27
                            break;
                        case 117:
                            map[X, Y] = '1';//28
                            break;
                        case 213:
                            map[X, Y] = '2';//29
                            break;
                        case 87:
                            map[X, Y] = '3';//30
                            break;
                        case 93:
                            map[X, Y] = '4';//31
                            break;
                        case 245:
                            map[X, Y] = '5';//32
                            break;
                        case 215:
                            map[X, Y] = '6';//33
                            break;
                        case 95:
                            map[X, Y] = '7';//34
                            break;
                        case 125:
                            map[X, Y] = '8';//35
                            break;
                        case 253:
                            map[X, Y] = '9';//36
                            break;
                        case 247:
                            map[X, Y] = '0';//37
                            break;
                        case 223:
                            map[X, Y] = 'Q';//38
                            break;
                        case 127:
                            map[X, Y] = 'W';//39
                            break;
                        case 92:
                        case 94:
                        case 220:
                        case 222:
                            map[X, Y] = 'E';//40
                            break;
                        case 113:
                        case 115:
                        case 121:
                        case 123:
                            map[X, Y] = 'R';//41
                            break;
                        case 197:
                        case 205:
                        case 229:
                        case 237:
                            map[X, Y] = 'T';//42
                            break;
                        case 23:
                        case 55:
                        case 151:
                        case 183:
                            map[X, Y] = 'Y';//43
                            break;
                        case 116:
                        case 118:
                        case 244:
                        case 246:
                            map[X, Y] = 'U';//44
                            break;
                        case 209:
                        case 211:
                        case 217:
                        case 219:
                            map[X, Y] = 'I';//45
                            break;
                        case 71:
                        case 79:
                        case 103:
                        case 111:
                            map[X, Y] = 'O';//46
                            break;
                        case 29:
                        case 61:
                        case 157:
                        case 189:
                            map[X, Y] = 'P';//47
                            break;
                        case 119:
                            map[X, Y] = 'A';//48
                            break;
                        case 221:
                            map[X, Y] = 'S';//49
                            break;


                        default:
                            break;

                    }
                if (Maze[X, Y] == ' ')
                    switch (idCela(Maze, X, Y))
                    {
                        case 206://1
                        case 214:
                        case 198:
                        case 230:
                        case 238:
                        case 254:
                        
                       
                        
                        map[X, Y] = 'D';
                        break;
                        case 70:
                        case 78:
                        case 102:
                        case 110:
                        case 126://2
                        map[X, Y] = 'F';
                        break;
                        case 236://3
                        case 196:
                        case 204:
                        case 228:
                        case 252:
                        map[X, Y] = 'G';
                        break;
                        case 124://4
                        case 108:
                        case 68:
                        case 76:
                        case 100:
                        map[X, Y] = 'H';
                        break;
                        case 166://5
                        case 174:
                        case 158:
                        case 190:
                        map[X, Y] = 'J';
                        break;
                        
                        case 250://6
                        case 202:
                        case 242:
                        case 234:
                        
                        map[X, Y] = 'K';
                        break;
                        case 170://7
                        case 186:
                        case 178:
                        case 154:
                        case 146:
                        map[X, Y] = 'L';
                        break;
                        case 156://8
                        case 188:
                        case 164:
                        case 172:
                        map[X, Y] = 'Z';
                        break;
                        
                        case 114://9
                        case 106:
                        case 122:
                        case 74:
                        map[X, Y] = 'X';
                        break;

                        case 0://16
                        map[X, Y] = '"';
                        break;
                        case 17://17
                        case 19:
                        case 25:
                        case 27:
                        case 41:
                        case 43:
                        case 49:
                        case 51:
                        case 57:
                        case 59:
                        case 147:
                        case 145:
                        case 169:
                        case 171:
                        case 177:
                        case 179:
                        case 185:
                        case 187:
                        case 155:
                        case 153:
                        map[X, Y] = '$';
                        break;
                        case 191://18
                        case 175:
                        case 47:
                        case 39:
                        case 31:
                        case 63:
                        case 167:
                        case 159:

                        map[X, Y] = '%';
                        break;
                        case 251://19
                        case 241:
                        case 243:
                        case 203:
                        case 233:
                        case 235:
                        case 201:
                        case 249:
                        map[X, Y] = '&';
                        break;
                        case 239:// aquest ha de ser tile diferent
                        case 199:// aquest ha de ser tile diferent
                        case 207:// aquest ha de ser tile diferent
                        case 231:// aquest ha de ser tile diferent
                            map[X, Y] = '/';
                            break;
                        default:
                        map[X, Y] = '"';
                        break;
                    }
            }
        }
    }

    int idCela(char[,] mapa, int X, int Y) // fer bucle i que doni un numero del 0 al 255 que sera l'identificador de casella, fer-ho en binari per evitar repetits i despres agafar i mirar amb un switch quin número correspon a la cela i depenent del numero posar cada tipus de cela 
    {

        int contar = 0;

        if (mapa[X, Y + 1] == '#') contar += 1;
        if (mapa[X + 1, Y + 1] == '#') contar += 2;
        if (mapa[X + 1, Y] == '#') contar += 4;
        if (mapa[X + 1, Y - 1] == '#') contar += 8;

        if (mapa[X, Y - 1] == '#') contar += 16;
        if (mapa[X - 1, Y - 1] == '#') contar += 32;
        if (mapa[X - 1, Y] == '#') contar += 64;
        if (mapa[X - 1, Y + 1] == '#') contar += 128;

        return contar;
    }

    // Només per enemics!!!!!!!!!
    public bool EstaBuida(int x, int y)
    {
        TileBase nextTile = mapaTerreny.GetTile(new Vector3Int(x-1,y-1,0));
        Debug.Log(x+", "+y+": "+ nextTile.name);
        return nextTile.name.StartsWith("TileTerra_");
    }

    // Retorna true si la casella es buida i no hi ha l'spawn, el tresor o un enemic
    public bool EsViable(int x, int y)
    {
        Vector2 pos = new Vector2(x, y);
        bool viable = true;
        foreach (Enemic enemic in enemics)
        {
            if(enemic != null)
            {
                Transform transform = enemic.transform;
                Vector2 posEnemic = transform.position;
                if (pos == posEnemic) { viable = false; break; }
            }
        }
        Debug.Log("Es buida "+pos+"? "+EstaBuida(x,y));
        return viable && EstaBuida(x, y) && (tresor!=null || tresor!=null && posTresor != pos) && pos.x != SpawnX && pos.y != SpawnY;
    }
}

