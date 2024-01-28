using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animations : MonoBehaviour
{
	Rigidbody2D rbody;
	private Animator anim;
    private Movement _movement;
    Vector2 movement_vector = new Vector2();
    
    void Start()
    {
        rbody = GetComponent<Rigidbody2D> ();
		anim = GetComponent<Animator> ();
        _movement = GetComponent<Movement>();
    }

    public void Armed_attack()
    {
        anim.SetBool("chest", false);
        anim.SetBool("fighting", true);
        
    }

    public void Unarmed_attack()
    {
        anim.SetBool("chest", false);
        anim.SetBool("fighting", true);
    }

    // Update is called once per frame
    void Update()
    {        
        movement_vector = _movement.GetMovement() * Time.fixedDeltaTime;

        if(!anim.GetBool("fighting"))
        {

            anim.SetFloat("X",movement_vector.x);
            anim.SetFloat("Y",movement_vector.y);       
                
            if (movement_vector != Vector2.zero) 
                anim.SetBool ("walking", true);                  
            else 
                anim.SetBool ("walking", false);                                           
        }        
    }
       
    public void Die()
    {
        anim.Play("Frontal_Death");
    }

    public void Unarmed(){
        anim.SetBool("chest",false);
        anim.SetBool("unarmed",true);
    }
    public void Armed(){
        anim.SetBool("chest", false);
        anim.SetBool("unarmed",false);
        
    }
    public void Chested(){
        anim.SetBool("chest",true);
        anim.SetBool("unarmed",true); 
    }
    public void OutFight()
    {
        anim.SetBool("fighting", false);

        if (anim.GetBool("chest"))
            Chested();
        else
            Unarmed();
    }

    public Animator GetAnim(){
        return anim;
    }

    public bool isUnarmed()
    {
        return anim.GetBool("unarmed");
    }
}

