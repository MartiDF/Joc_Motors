using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class Movement : MonoBehaviour
{
    [Range(0.5f,10f)]
    public float moveSpeed = 2f; 
    [Range(0.0f,1.0f)]
    public float heightOffset = 0.3f;
    private Tilemap _tilemap; 
    private Vector3 destinationPosition; 
    private Vector3Int currentCellPosition; 
    private Animations _anims;
    private MazeMaker _mazeMaker;
    private float verticalInput, horizontalInput;

    void Start()
    {   
        _tilemap = GameObject.Find("Maze").transform.Find("Grid").transform.Find("Tilemap").GetComponent<Tilemap>();
        _anims = this.GetComponent<Animations>();
        _mazeMaker = GameObject.Find("Maze").GetComponent<MazeMaker>();  

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
                
                Vector3Int nextCellPosition = currentCellPosition + new Vector3Int((int)horizontalInput, (int)verticalInput, 0);
                TileBase nextTile = _tilemap.GetTile(nextCellPosition);

                if (!nextTile.name.StartsWith("TileTerra_")) return;                

                currentCellPosition = nextCellPosition;
                Vector3 off = new Vector3(0,heightOffset,0);

                destinationPosition = _tilemap.GetCellCenterWorld(currentCellPosition)+off;


            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, (destinationPosition), moveSpeed * Time.deltaTime);
            }
        }

        if (Input.GetButtonDown("Submit"))
        {
            Debug.Log("Enemics a mover la bochaina");
            _mazeMaker.FerTornEnemics();
        }
    }


    public bool isWalking(){
        return horizontalInput!=0 || verticalInput!=0;
    }

    public bool isFighting(){
        return _anims.GetAnim().GetBool("Fight");
    }
    
    public Vector2 GetMovement(){            
        return new Vector2(horizontalInput, verticalInput);
    }
}