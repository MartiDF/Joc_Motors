using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float _stamina = 1;
    private GameController _gm;

    private bool _inConsumable, _inSword, _inChest, _inSpawn;
    private bool _chested;
    private Collider2D _currentCollider;
    public Directions Direction = Directions.down;

    public bool IsDead { private set; get; } = false;
    public bool IsFighting { private set; get; } = false;
    public bool IsWalking = false;
    public bool IsArmed = false;
    public Score _puntuacio;
    private float puntenemic= 250;

    public void Start()
    {
        _gm = GameObject.Find("GameManager").GetComponent<GameController>();
        _puntuacio = GameObject.Find("Scorenumber").GetComponent<Score>();
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
        return IsArmed;}

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
        IsFighting = true;

                
        if (obj.CompareTag("Enemy"))
        {
            SetStamina(IsArmed ? _gm.GetStaminaFightArmed() : _gm.GetStaminaFight());
            obj.gameObject.GetComponent<Enemic>().GetHit(_stamina <= 0);
            _unarmed = true;
            _puntuacio.Sumarpunts(puntenemic);
        }
    }

    public void GoArmed() {
        _chested = false;
        IsArmed = true;
        
    }

    public void GoChested() { 
        _chested = true;
        IsArmed = false;
    }

    internal void Die()
    {
        IsDead = true;
    }

    public void AttackAnimationFinished()
    {
        IsFighting = false;
        IsArmed = false;
    }
}