using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimatorController : MonoBehaviour
{
    private Enemic enemic;
    // Start is called before the first frame update
    void Start()
    {
        enemic = gameObject.GetComponent<Enemic>();
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
}

public enum EnemyStates
{
    Idle = 0,
    Attacking = 1,
    Dying = 2,
}
