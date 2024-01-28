using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Player player;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GetComponent<Player>();
    }
    private void Update()
    {
        if (player.IsDead) animator.Play(PlayerAnimations.DEATH);
        else if (player.IsFighting)
        {
            if (player.IsArmed)
            {
                animator.Play(PlayerAnimations.ATTACK_ARMED[(int)player.Direction -1]);
            }
            else
            {
                animator.Play(PlayerAnimations.ATTACK[(int)player.Direction -1]);
            }
        }
        else if (player.IsWalking)
        {
            if (player.IsArmed)
            {
                animator.Play(PlayerAnimations.WALK_ARMED[(int)player.Direction -1]);
            }
            else
            {
                animator.Play(PlayerAnimations.WALK[(int)player.Direction -1]);
            }
        }
        else
        {
            if (player.IsArmed)
            {
                animator.Play(PlayerAnimations.IDLE_ARMED[(int)player.Direction -1]);
            }
            else
            {
                animator.Play(PlayerAnimations.IDLE[(int)player.Direction -1]);
            }
        }
    }

}

public class PlayerAnimations
{
    public static readonly string[] ATTACK_ARMED = {
        "Armed_LeftSide_Attack",
        "Armed_Back_attack",
        "Armed_RightSide_Attack",
        "Armed_Frontal_Attack"
    };

    public static readonly string[] WALK_ARMED = {
        "Armed_LeftSide_Walk",
        "Armed_Back_Walk",
        "Armed_RightSide_Walk",
        "Armed_Frontal_Walk"
    };
    public static readonly string[] IDLE_ARMED = {
        "Armed_LeftSide_Idle",
        "Armed_Back_Idle",
        "Armed_RightSide_Idle",
        "Armed_Frontal_Idle"
    };
    public static readonly string[] ATTACK = {
        "LeftSide_Attack",
        "Back_Attack",
        "RightSide_Attack",
        "Frontal_Attack"
    };
    public static readonly string[] WALK = {
        "LeftSide_Walk",
        "Back_Walk",
        "RightSide_Walk",
        "Frontal_Walk",
    };
    public static readonly string[] IDLE = {
        "LeftSide_Idle",
        "Back_Idle",
        "RightSide_Idle",
        "Frontal_Idle"
    };
    public static readonly string DEATH = "Frontal_Death";
}
