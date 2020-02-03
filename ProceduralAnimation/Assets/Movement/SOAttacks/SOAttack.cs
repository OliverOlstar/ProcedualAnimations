using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack ScriptableObject", menuName = "Attacks/New Attack", order = 0)]
public class SOAttack : ScriptableObject
{
    [Header("Attack itself")]
    [Min(0)] public float attackTime = 0.5f;
    public SOGraph attackGraph;
    
    [Header("Post Attack")]
    [Min(0)] public float holdEndPosTime = 0.1f;

    [Header("Pre Attack")]
    [Min(0)] public float holdStartPosTime = 0.1f;
    [Min(0)] public float transitionToTime = 0.2f;
    public bool chargeable = false;

    [Header("During Attack Stepping")]
    [Min(0)] public float forceForwardTime = 0.2f;
    [Min(0)] public float stopForceForwardTime = 0.3f;
    public float forceForwardAmount = 50.0f;

    //[Space]
    //public float enableHitboxTime = 0.2f;
    //public float disableHitboxTime = 0.4f;
}
