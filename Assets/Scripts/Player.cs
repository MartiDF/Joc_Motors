using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float _stamina = 1;
    private GameController _gm;
    private Animations _anims;

    private bool _inConsumable, _inSword, _inChest, _inFight, _inSpawn;
    private bool _chested, _armed;
    private Collider2D _currentCollider;

    public void Start()
    {
        _anims = GetComponent<Animations>();
        _gm = GameObject.Find("GameManager").GetComponent<GameController>();
    }

    /* GETTERS */
    public bool GetInConsumable() {
        return _inConsumable;}

    public bool GetInChest() {
        return _inChest;}

    public bool GetInSword() {
        return _inSword;}

    public bool GetInSpawn() {
        return _inSpawn;}
    
    public bool GetIsChested() {
        return _chested;}

    public bool GetIsArmed()  {
        return _armed;}

    public float GetStamina() {
        return _stamina;}

    /* TRIGGERS */
    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Enemy":{
                GoFight(collision);
                    break;
            }
            case "Spawn":{
                _inSpawn = true;
                    break;
            }
            case "Consumable":{                
                _inConsumable = true;
                    break;
            }     
            case "Chest":{
                _inChest = true;
                    break;
            }        
            case "Sword":{
                _inSword = true;
                    break;
            }       
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //Coneixer el collider actual per la propia destrucció.
        _currentCollider = collision;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Consumable":{                
                _inConsumable = false;
                    break;
            }     
            case "Chest":{
                _inChest = false;
                    break;
            }        
            case "Sword":{
                _inSword = false;
                    break;
            } 
            case "Spawn":{
                _inSpawn = false;
                    break;
            }           
        }
    }

    public Collider2D GetCurrentCollision(){
        return _currentCollider;
    }

    public void ActionDone(){
        _inConsumable = false;
        _inChest = false;
        _inSword = false;
       
        Destroy(GetCurrentCollision().gameObject);
    }

    /* SETTERS */
    public void SetStamina(float valor){

       float aux = _stamina+valor;
       if(!(aux>1.0f)){             
            if(!(aux<0.0f))       
                _stamina+=valor;                           
            else
                _stamina = 0;            
        }
        else
            _stamina = 1;
        
        _gm.UpdateStamina();
    }    

    
    /* METHODS */
    public void GoFight(Collider2D obj) {
        if (obj == null) return;
        if (_anims.isArmed()) _anims.Armed_attack();
        else _anims.Unarmed_attack();
                
        if (obj.CompareTag("Enemy"))
        {
            SetStamina(_armed ? _gm.GetStaminaFightArmed() : _gm.GetStaminaFight());
            obj.gameObject.GetComponent<Enemic>().GetHit(_stamina <= 0);
            _armed = false;
        }
        else
        {
             Destroy(obj.gameObject);
            _armed = false;
        }
        

    }

    public void GoArmed() {
        _chested = false;
        _armed = true;
        
    }

    public void GoChested() { 
        _chested = true;
        _armed = false;
    }
}