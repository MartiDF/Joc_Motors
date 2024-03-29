using UnityEngine.Tilemaps;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    /* SCRIPTS */
    private Movement _movement;
    private Player _player;
    private MazeMaker _mazeMaker;

    private Tilemap _tileMapConsumibles;

    /* GAME OBJECTS */
    private GameObject _triggers;
    public GameObject _stamina;
    public GameObject _deathPanel;
    public GameObject _pausePanel;
    public GameObject _winPanel;
    public TMP_Text _staminaText; //debug

    public GameObject _cofreTrigger;
    public GameObject _espasaTrigger;
    public UnityEngine.Tilemaps.Tile _cofreTile;
    public UnityEngine.Tilemaps.Tile _espasaTile;
    public Image _keyUI;

    public Score _puntuacio;
    private float puntpas=0.005f;
    private float puntguanyar=10000;
    private float puntcons= 50;
    private float puntobjecte= 200;


    /* GAME OBJECTS */
    [Range(-0.01f, -1f)] public float _staminaFight = -0.3f;
    [Range(-0.01f, -1f)] public float _staminaFightArmed = -0.15f;
    [Range(-1.60f, -1f)] public float _staminaWalk = -1.4f; //valor relatiu
    [Range(+0.01f, +1f)] public float _staminaCons = 0.15f;   

    //GETTERS
    public float GetStaminaWalk(){        return _staminaWalk/1000;     }
    public float GetStaminaConsumable(){  return _staminaCons;          }
    public float GetStaminaFight(){       return _staminaFight;         }
    public float GetStaminaFightArmed(){  return _staminaFightArmed;    }

    void Start()
    {        
        //SETTERS
        _player = GameObject.Find("Player").GetComponent<Player>();
        _movement = GameObject.Find("Player").GetComponent<Movement>();
        _mazeMaker = GameObject.Find("Maze").GetComponent<MazeMaker>();
        _tileMapConsumibles = GameObject.Find("Maze").transform.GetChild(0).transform.GetChild(1).GetComponent<Tilemap>();
        _triggers = GameObject.Find("Maze").transform.GetChild(1).gameObject;
        _keyUI.enabled = false;
        _puntuacio = GameObject.Find("Scorenumber").GetComponent<Score>();
    }

    void Update(){

        StaminaControl();
        PlayerTriggers();
        MenuControl();
    }

    /* MENU CONTROL */
    public void PauseGame (){
        if(!_deathPanel.activeSelf){
            _pausePanel.GetComponent<Animator>().Play("Pause");
            Time.timeScale = 0;
        }
    }

    public void ResumeGame (){
        if(!_deathPanel.activeSelf){
            _pausePanel.GetComponent<Animator>().Play("PauseOut");
            Time.timeScale = 1;
        }
    }

    public void UpdateStamina(){        
        _stamina.transform.localScale = new Vector3(1, _player.GetStamina(), 1);
        _staminaText.text = (Mathf.Round(_player.GetStamina() * 100f) / 100f).ToString();
    }

    public void Guanyar(){
        _puntuacio.Sumarpunts(puntguanyar);
        _puntuacio.Sumarpunts(_player.GetStamina()*5);
        _winPanel.SetActive(true);
    }

    private void DeleteTile() {

        //Això elimina la tile del costat també (no importa suposo)
        Vector3Int positions = _tileMapConsumibles.WorldToCell(_player.transform.position);
        _tileMapConsumibles.SetTile(new Vector3Int(positions.x - 1, positions.y - 1, 0), null);
        _tileMapConsumibles.SetTile(new Vector3Int(positions.x - 2, positions.y - 1, 0), null);
        _tileMapConsumibles.SetTile(new Vector3Int(positions.x, positions.y - 1, 0), null);
        _tileMapConsumibles.SetTile(new Vector3Int(positions.x - 1, positions.y - 2, 0), null);
        _tileMapConsumibles.SetTile(new Vector3Int(positions.x - 1, positions.y, 0), null);       
    }

    private void GetConsumable() {
        DeleteTile();
        _puntuacio.Sumarpunts(puntobjecte);
        _puntuacio.Sumarpunts(puntcons);
        _player.SetStamina(GetStaminaConsumable());
        
    }

    private void GetSword() {
        _puntuacio.Sumarpunts(puntobjecte);
        DeleteTile();
        if (_player.GetIsChested())
        {
            Debug.Log("Deixar Cofre");
            _keyUI.enabled = false;
            puntpas=1;
            Vector3Int positions = _tileMapConsumibles.WorldToCell(_player.transform.position);
            _tileMapConsumibles.SetTile(new Vector3Int(positions.x - 1, positions.y - 1, 0), _cofreTile);
            GameObject nouTrigger = Instantiate(_cofreTrigger, _tileMapConsumibles.GetCellCenterWorld(_tileMapConsumibles.WorldToCell(new Vector3Int(positions.x - 1, positions.y - 1, 0))), Quaternion.identity);
            nouTrigger.transform.parent = _triggers.transform;
        }
        _player.GoArmed();
        _player.IsArmed = true;
    }

    private void GetChest() {
        DeleteTile();
        _puntuacio.Sumarpunts(puntobjecte);
        puntpas=3;
        if (_player.GetIsArmed())
        {
            Debug.Log("Deixar Espasa");
            Vector3Int positions = _tileMapConsumibles.WorldToCell(_player.transform.position);
            _tileMapConsumibles.SetTile(new Vector3Int(positions.x - 1, positions.y - 1, 0), _espasaTile);
            GameObject nouTrigger = Instantiate(_espasaTrigger, _tileMapConsumibles.GetCellCenterWorld(_tileMapConsumibles.WorldToCell(new Vector3Int(positions.x - 1, positions.y - 1, 0))), Quaternion.identity);
            nouTrigger.transform.parent = _triggers.transform;
        }
        _keyUI.enabled = true;
        _player.GoChested();
    }

    void StaminaControl() {
                
        if (_player.GetStamina() == 0)
        {
            _deathPanel.SetActive(true);
            _player.Die();
        }

        if (_movement.isWalking())
        {
            _puntuacio.Sumarpunts(puntpas);
            _player.SetStamina(_staminaWalk / 10000);

        }
    }

    //Control de consumibles i triggers del jugador
    void PlayerTriggers() {
        if (_player.GetInConsumable() && Input.GetKeyDown(KeyCode.E)) {
            GetConsumable(); 
                _player.ActionDone();
        }

        if (_player.GetInSword() && Input.GetKeyDown(KeyCode.E) && !_player.GetIsArmed()) {      
            GetSword();
                _player.ActionDone();
        }

        if (_player.GetInChest() && Input.GetKeyDown(KeyCode.E)) {          
            GetChest();
                _player.ActionDone();                    
        }

        if (_player.GetInSpawn() && _player.GetIsChested())
            Guanyar();
    }

    //Control de pausa
    void MenuControl() {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale > 0)
                PauseGame();

            else
                ResumeGame();
        }
    }
}
