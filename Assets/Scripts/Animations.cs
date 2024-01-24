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

    public void Fight()
    {        
        anim.SetBool("unarmed", false);
        anim.Play("Fight");
        anim.SetBool("Fight", true);
    }

    // Update is called once per frame
    void Update()
    {        
        movement_vector = _movement.GetMovement() * Time.fixedDeltaTime;

        if(!anim.GetBool("Fight")){

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
        anim.Play("Die");
    }

    public void Unarmed(){
        anim.SetBool("chest",false);
        anim.SetBool("armed",false);
        anim.SetBool("unarmed",true);
    }
    public void Armed(){
        anim.SetBool("unarmed",false);
        anim.SetBool("armed",true);
        anim.SetBool("chest",false);
    }
    public void Chested(){
        anim.SetBool("chest",true);
        anim.SetBool("unarmed",false);
        anim.SetBool("armed",false);        
    }
    public void OutFight()
    {
        anim.SetBool("Fight", false);

        if (anim.GetBool("chest"))
            Chested();
        else
            Unarmed();
    }

    public Animator GetAnim(){
        return anim;
    }
}

public class Enemies_Animations : MonoBehaviour
{
    Rigidbody2D rbody;
    private Animator anim;
    private Movement _movement;
    Vector2 movement_vector = new Vector2();

    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        _movement = GetComponent<Movement>();
    }

    public void Fight()
    {
        anim.SetBool("Patrol", false);
        anim.Play("Fight");
        anim.SetBool("Fight", true);
    }

    // Update is called once per frame
    void Update()
    {
        movement_vector = _movement.GetMovement() * Time.fixedDeltaTime;

        if (!anim.GetBool("Fight"))
        {

            anim.SetFloat("X", movement_vector.x);
            anim.SetFloat("Y", movement_vector.y);

            if (movement_vector != Vector2.zero)
                anim.SetBool("patrolling", true);
            else
                anim.SetBool("patrolling", false);
        }
    }

    public void Die()
    {
        anim.Play("Die");
    }

    public void Patrol()
    {
        anim.SetBool("player_on_sight", false);
        anim.SetBool("following", false);
        anim.SetBool("attacking", false);
    }
    public void Following()
    {
        anim.SetBool("player_on_sight", true);
        anim.SetBool("following", true);
        anim.SetBool("attacking", false);
    }
    public void Attacking()
    {
        anim.SetBool("player_on_sight", true);
        anim.SetBool("following", true);
        anim.SetBool("attacking", true);
    }
    

    public Animator GetAnim()
    {
        return anim;
    }
}

