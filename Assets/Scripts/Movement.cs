using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class Movement : MonoBehaviour
{
    public static Movement Instance { get; private set; }

    [Range(0.5f,10f)]
    public float moveSpeed = 2f; 
    [Range(-1.0f,1.0f)]
    public float heightOffset = 0.3f;
    private Tilemap _tilemap; 
    private Vector3 destinationPosition; 
    public Vector3Int currentCellPosition;
    private MazeMaker _mazeMaker;
    private float verticalInput, horizontalInput;

    private Player player;

    private bool enemyCanMove = false;

    private List<Enemic> enemics = new List<Enemic>();

    void Start()
    {
        Instance = this;
        _tilemap = GameObject.Find("Maze").transform.Find("Grid").transform.Find("Tilemap").GetComponent<Tilemap>();
        _mazeMaker = GameObject.Find("Maze").GetComponent<MazeMaker>();
        player = gameObject.GetComponent<Player>();


        currentCellPosition += new Vector3Int(_mazeMaker.getRelativeSpawnX(), _mazeMaker.getRelativeSpawnY(), 0);
        destinationPosition = _tilemap.GetCellCenterWorld(currentCellPosition)+new Vector3(0,heightOffset,0);
        transform.position = destinationPosition; 
    }

    void Update()
    {        
        if(!isFighting()){

            float distancia = Vector3.Distance(transform.position, (destinationPosition));
            if (distancia < 0.0001f)
            {

                horizontalInput = Input.GetAxisRaw("Horizontal");
                verticalInput = Input.GetAxisRaw("Vertical");

                if (Mathf.Abs(horizontalInput) > Mathf.Abs(verticalInput))
                    verticalInput = 0f;
                
                else        
                    horizontalInput = 0f;

                if (verticalInput != 0 || horizontalInput != 0) player.Direction = findDirection(horizontalInput, verticalInput);
                Vector3Int nextCellPosition = currentCellPosition + new Vector3Int((int)horizontalInput, (int)verticalInput, 0);
                TileBase nextTile = _tilemap.GetTile(nextCellPosition);

                if (!nextTile.name.StartsWith("TileTerra_")) return;                

                currentCellPosition = nextCellPosition;
                Vector3 off = new Vector3(0,heightOffset,0);

                destinationPosition = _tilemap.GetCellCenterWorld(currentCellPosition)+off;
                
                if (horizontalInput != 0 || verticalInput != 0) enemyCanMove = true;
                player.IsWalking = false;
            }
            else
            {
                if (enemyCanMove)
                {
                    enemyCanMove = false;
                    _mazeMaker.FerTornEnemics();
                }
                transform.position = Vector3.MoveTowards(transform.position, (destinationPosition), moveSpeed * Time.deltaTime);
                player.IsWalking = true;
            }
        }
    }

    private Directions findDirection(float x, float y)
    {
        if (x > 0) return Directions.right;
        else if (x < 0) return Directions.left;
        else if (y > 0) return Directions.up;
        else return Directions.down;
    }

    public bool isWalking(){
        return horizontalInput!=0 || verticalInput!=0;
    }

    public bool isFighting(){
        return player.IsFighting;
    }
    
    public Vector2 GetMovement(){            
        return new Vector2(horizontalInput, verticalInput);
    }
}
