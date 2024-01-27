using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyAnimatorController : MonoBehaviour
{
    private Enemic enemic;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool isPatrolling = true;
    private bool isFollowing = false;
    private bool isFighting = false;
    private bool isDying = false;

    // Start is called before the first frame update
    void Start()
    {
        //_tileMapConsumibles = GameObject.Find("Maze").transform.GetChild(0).transform.GetChild(1).GetComponent<Tilemap>();
        //direction = GameObject.Find("direction").transform.GetChild(0).transform.GetChild(1).GetComponent<direction>();
        enemic = gameObject.GetComponent<Enemic>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        if (enemic == null) return;

    }
    private enum Animations
    {
        Back,
        Side,
        Front,
        Back_Attack,
        Back_Damage,
        Side_Attack,
        Side_Damage,
        Front_Attack,
        Front_Damage
    }


    public void ChangeAnimationState()
    {
        // Se podria optimizar pero por ahora deberia funcionar
        if (enemic.Direction == 1) spriteRenderer.flipX = true;
        else spriteRenderer.flipX = false;
        switch (enemic.state)
        {
            case EnemyStates.Idle:
                if (enemic.Direction == 1 || enemic.Direction == 3) animator.Play(EnemyAnimationStates.SIDE);
                else if (enemic.Direction == 4) animator.Play(EnemyAnimationStates.FRONT);
                else animator.Play(EnemyAnimationStates.BACK);
                break;
            case EnemyStates.Attacking:
                if (enemic.Direction == 1 || enemic.Direction == 3) animator.Play(EnemyAnimationStates.SIDE_ATTACK);
                else if (enemic.Direction == 4) animator.Play(EnemyAnimationStates.FRONT_ATTACK);
                else animator.Play(EnemyAnimationStates.BACK_ATTACK);
                break;
            case EnemyStates.Dying:
                if (enemic.Direction == 1 || enemic.Direction == 3) animator.Play(EnemyAnimationStates.SIDE_DAMAGE);
                else if (enemic.Direction == 4) animator.Play(EnemyAnimationStates.FRONT_DAMAGE);
                else animator.Play(EnemyAnimationStates.BACK_DAMAGE);
                break;
        }
    }

    public void animationAttackFinished()
    {
        if (enemic.StayAlive) enemic.ChangeState(EnemyStates.Idle);
        else
        {
            enemic.ChangeState(EnemyStates.Dying);
        }
    }

    public void animationDamageFinished()
    {
        enemic.Die();
    }
}



public class EnemyAnimationStates
{
    //Animation states
    public const string SIDE = "Side";
    public const string BACK = "Back";
    public const string FRONT = "Front";
    public const string SIDE_ATTACK = "Side_Attack";
    public const string SIDE_DAMAGE = "Side_Damage";
    public const string FRONT_ATTACK = "Frontal_Attack";
    public const string FRONT_DAMAGE = "Frontal_Damage";
    public const string BACK_ATTACK = "Back_Attack";
    public const string BACK_DAMAGE = "Back_Damage";
}

public enum EnemyStates
{
    Idle = 0,
    Attacking = 1,
    Dying = 2,
}