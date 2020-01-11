using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateController : MonoBehaviour
{
    //THIS IS THE STATE MANAGER - Danish, Oliver

         /*
         This script is to change the player's active state 
         depending on the inputs received. 
         */


    [Header("Inputs")]
    // Movement variables
    [HideInInspector] public float horizontalInput = 0;
    [HideInInspector] public float verticalInput = 0;
    [HideInInspector] public Vector3 movementDir;
    [HideInInspector] public float moveAmount = 0;

    // Movement Variables
    [HideInInspector] public bool jumpInput = false;
    [HideInInspector] public bool longDodgeInput = false;
    [HideInInspector] public bool shortDodgeInput = false;

    // Attack Varaibles
    [HideInInspector] public bool quickAttackInput = false;
    [HideInInspector] public bool heavyAttackInput = false;

    //Camera
    [HideInInspector] public bool lockOnInput = false;

    // Power Use Inputs
    [HideInInspector] public int powerInput = 0;

    // Menu Inputs
    [HideInInspector] public bool pauseInput = false;
    [HideInInspector] public bool mapInput = false;

    // On Ground
    [HideInInspector] public bool OnGround = false;
    [HideInInspector] public bool Stunned = false;
    [HideInInspector] public float AttackStateReturnDelay = 0;
    [HideInInspector] public float LastInputTime = 0;

    [Header("State Components")]
    [HideInInspector] public PlayerStateMachine _stateMachine;
    public MovementComponent _movementComponent { get; private set; } // Player's movement component, access this to move and jump
    [HideInInspector] public PlayerDodge _dodgeComponent; // Player's dodge component, access this to
    private PlayerLockOnScript _lockOnComponent;
    [HideInInspector] public PlayerPowerHandler _powerComponent;
    [HideInInspector] public PlayerHitbox _hitboxComponent;

    //[HideInInspector] public PlayerRespawn _respawnComponent;
    [HideInInspector] public PlayerAttributes _playerAttributes;
    [HideInInspector] public AnimHandler _animHandler;

    [HideInInspector] public Rigidbody _rb;
    [HideInInspector] public Transform _Camera;

    //Reset Player
    [HideInInspector] public bool Respawn = false;
    
    void Start()
    {
        movementDir = transform.forward;

        _movementComponent = GetComponent<MovementComponent>();
        _dodgeComponent = GetComponent<PlayerDodge>();
        _lockOnComponent = GetComponent<PlayerLockOnScript>();
        _powerComponent = GetComponent<PlayerPowerHandler>();
        _hitboxComponent = GetComponentInChildren<PlayerHitbox>();
        _hitboxComponent.gameObject.SetActive(false);

        //_respawnComponent = GetComponent<PlayerRespawn>();
        _playerAttributes = GetComponent<PlayerAttributes>();
        _animHandler = GetComponentInChildren<AnimHandler>();

        _stateMachine = GetComponent<PlayerStateMachine>();
        InitializeStateMachine();

        _rb = GetComponent<Rigidbody>();
        _Camera = Camera.main.transform;
    }

    void InitializeStateMachine()
    {
        var states = new Dictionary<Type, BaseState>()
        {
            {typeof(MovementState), new MovementState(controller:this) },
            {typeof(DodgeState), new DodgeState(controller:this) },
            {typeof(StunnedState), new StunnedState(controller:this) },
            {typeof(AttackState), new AttackState(controller:this) },
            {typeof(DeathState), new DeathState(controller:this) }
        };

        _stateMachine.SetStates(states);
    }
}
