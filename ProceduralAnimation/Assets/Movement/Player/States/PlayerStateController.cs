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

    // On Ground
    [HideInInspector] public bool OnGround = false;
    [HideInInspector] public bool Stunned = false;
    [HideInInspector] public float AttackStateReturnDelay = 0;
    [HideInInspector] public float LastInputTime = 0;
    [HideInInspector] public Vector2 LastMoveDirection = new Vector2(0,0);

    [HideInInspector] public InputPlayer inputActions;

    [Header("State Components")]
    [HideInInspector] public PlayerStateMachine _stateMachine;
    public MovementComponent _movementComponent { get; private set; } // Player's movement component, access this to move and jump
    [HideInInspector] public DodgeComponent _dodgeComponent; // Player's dodge component, access this to
    private PlayerLockOnScript _lockOnComponent;
    [HideInInspector] public PlayerPowerHandler _powerComponent;
    [HideInInspector] public PlayerHitbox _hitboxComponent;
    //[HideInInspector] public ModelMovement _modelController;

    //[HideInInspector] public PlayerRespawn _respawnComponent;
    [HideInInspector] public PlayerAttributes _playerAttributes;
    [HideInInspector] public AnimHandler _animHandler;

    [HideInInspector] public Rigidbody _rb;
    [HideInInspector] public Transform _Camera;

    //Reset Player
    [HideInInspector] public bool Respawn = false;
    
    void Awake()
    {
        _movementComponent = GetComponent<MovementComponent>();
        _dodgeComponent = GetComponent<DodgeComponent>();
        _lockOnComponent = GetComponent<PlayerLockOnScript>();
        _powerComponent = GetComponent<PlayerPowerHandler>();
        _hitboxComponent = GetComponentInChildren<PlayerHitbox>();
        //_hitboxComponent.gameObject.SetActive(false);
        //_modelController = GetComponentInChildren<ModelMovement>();

        //_respawnComponent = GetComponent<PlayerRespawn>();
        _playerAttributes = GetComponent<PlayerAttributes>();
        _animHandler = GetComponentInChildren<AnimHandler>();

        _stateMachine = GetComponent<PlayerStateMachine>();
        InitializeStateMachine();

        _rb = GetComponent<Rigidbody>();
        _Camera = Camera.main.transform;
        inputActions = new InputPlayer();
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    void InitializeStateMachine()
    {
        var states = new Dictionary<Type, BaseState>()
        {
            {typeof(IdleState), new IdleState(controller:this) },
            {typeof(MovementState), new MovementState(controller:this) },
            {typeof(DodgeState), new DodgeState(controller:this) },
            {typeof(StunnedState), new StunnedState(controller:this) },
            {typeof(AttackState), new AttackState(controller:this) },
            {typeof(DeathState), new DeathState(controller:this) }
        };

        _stateMachine.SetStates(states);
    }
}
