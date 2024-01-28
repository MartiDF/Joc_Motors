using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Debugger_ : MonoBehaviour
{    
    public TMP_Text _movement;
    public TMP_Text _pose;       
    public TMP_Text _stamina;       
    public TMP_Text _trigger;       
    public GameObject _panel;
    public GameObject _panel2;
    public GameObject _light;
    public GameController _gm;
    public Player _player;
    public Movement _move;
    public SceneM _scene;

    private bool toggled = false;
    void Update()
    {
        if(Input.GetKey(KeyCode.LeftControl)){

            _trigger.text = "InSword ["+_player.GetInSword()+"] - "+
                            "InChest ["+_player.GetInChest()+"] - "+
                            "InSpawn ["+_player.GetInSpawn()+"] - "+
                            "InCons ["+_player.GetInConsumable()+"]";

            //Moviment
            if(_move.isWalking())
                _movement.text="WALKING";
            else if(_move.isFighting())
                _movement.text="FIGHTING";
            else
                _movement.text="\"IDLE\"";        
            
            //Pose 
            if(_player.GetIsArmed())
                _pose.text="armed";
            else if(_player.GetIsChested())
                _pose.text="chest";
            else
                _pose.text="unarmed";



            //Controls 
            if (Input.GetKeyDown(KeyCode.I)) {       //_panel INFO
                _panel.SetActive(!_panel.activeSelf);
                _panel2.SetActive(!_panel2.activeSelf);

                if(toggled)
                toggled = false;
                else
                toggled = true;
            }
        }

            if (Input.GetKeyDown(KeyCode.G) && toggled)        //GUANYAR
                _gm.Guanyar(); 
            

            if (Input.GetKeyDown(KeyCode.Q) && toggled){       //ARMED
                _player.GoArmed();
            }

            if (Input.GetKeyDown(KeyCode.C) && toggled)  {       //CHEST
                _player.GoChested();
            }

                

            if (Input.GetKeyDown(KeyCode.F) && toggled)        //FIGHT       
                _player.GoFight(null);             
            
            if (Input.GetKeyDown(KeyCode.R) && toggled)        //Reload Scene          
                _scene.ChangeScene("Main");    
            
            if (Input.GetKeyDown(KeyCode.N) && toggled)       //CONSUMABLE      
                _player.SetStamina(_gm.GetStaminaConsumable());
            
            if (Input.GetKeyDown(KeyCode.L) && toggled)       //LLUM     
                _light.SetActive(!_light.activeSelf);
            
        
    }

}
