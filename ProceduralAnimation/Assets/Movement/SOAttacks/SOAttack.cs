using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack ScriptableObject", menuName = "Attacks/New Attack", order = 0)]
public class SOAttack : ScriptableObject
{
    public float attackTime = 0.5f;

    [Space]
    public float transitionToTime = 0.2f;

    [Space]
    public float forceForwardTime = 0.2f;
    public float forceForwardAmount = 50.0f;

    //[Space]
    //public float enableHitboxTime = 0.2f;
    //public float disableHitboxTime = 0.4f;
}
