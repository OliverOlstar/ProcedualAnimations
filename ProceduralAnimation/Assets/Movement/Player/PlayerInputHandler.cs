using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    // THIS IS THE INPUT HANDLER - Danish, Oliver

    /*
    This script is to get keyboard and gamepad input 
    and then send through to the State Manager
    */

    [Header("Dodge Input Settings")]
    [SerializeField] private float dodge_MaxHoldTime = 0.4f;
    [SerializeField] private float dodge_TimeToLong = 0.3f;

    [Header("Movement")]
    // Movement Inputs
    private float vertical;
    private float horizontal;
    //[SerializeField] private KeyCode KCverticalUp = KeyCode.W;
    //[SerializeField] private KeyCode KCverticalDown = KeyCode.S;
    //[SerializeField] private KeyCode KChorizontalLeft = KeyCode.A;
    //[SerializeField] private KeyCode KChorizontalRight = KeyCode.D;

    [Header("Attack")]
    [SerializeField] private KeyCode KCattack_Input = KeyCode.Mouse0;
    [SerializeField] private KeyCode gp_KCattack_Input = KeyCode.JoystickButton2;
    private bool attack_Input;
    private bool attack_Release_Input;
    private bool attack_Input_WaitingForRelease;
    private float attack_Timer;

    [Space]
    [SerializeField] private float attack_TimeToHeavy = 0.3f;
    [SerializeField] private float attack_MaxHoldTime = 0.6f;

    [Header("Camera")]
    [SerializeField] private KeyCode KClockon_Input = KeyCode.Mouse1;
    [SerializeField] private KeyCode gp_KClockon_Input = KeyCode.JoystickButton3;
    private bool lockon_Input;

    [Header("Dodge")]
    [SerializeField] private KeyCode KCdodge_Input = KeyCode.Space;
    [SerializeField] private KeyCode gp_KCdodge_Input = KeyCode.JoystickButton0;
    private bool dodge_Input;
    private bool dodge_release_Input;
    private bool dodge_Input_WaitingForRelease;
    private float dodge_Timer;

    [Header("Jump")]
    [SerializeField] private KeyCode KCjump_Input = KeyCode.LeftShift;
    [SerializeField] private KeyCode gp_KCjump_Input = KeyCode.JoystickButton1;
    private bool jump_Input;

    [Header("Powers")]
    [SerializeField] private KeyCode KCpower1_Input = KeyCode.Q;
    [SerializeField] private KeyCode KCpower2_Input = KeyCode.E;
    [SerializeField] private KeyCode KCpower3_Input = KeyCode.F;
    [SerializeField] private KeyCode gp_KCpower1_Input = KeyCode.JoystickButton4;
    [SerializeField] private KeyCode gp_KCpower2_Input = KeyCode.JoystickButton5;
    [SerializeField] private KeyCode gp_KCpower3_Input = KeyCode.JoystickButton8;
    private int power_Input;

    [Header("Menu")]
    [SerializeField] private KeyCode KCpause_Input = KeyCode.Escape;
    [SerializeField] private KeyCode KCmap_Input = KeyCode.M;
    [SerializeField] private KeyCode gp_KCpause_Input = KeyCode.JoystickButton6;
    [SerializeField] private KeyCode gp_KCmap_Input = KeyCode.JoystickButton7;
    private bool pause_Input;
    private bool map_Input;

    [Header("Disable")]
    public bool inputDisabled = false;

    private PlayerStateController _stateController;

    private float delta;

    private void Awake()
    {
        _stateController = GetComponent<PlayerStateController>();
    }
    
    private void Update()
    {
        if (inputDisabled) return;

        delta = Time.deltaTime;

        GetInput();
        UpdateStates();
        ResetInputAndTimers();
    }

    void GetInput()
    {
        //vertical = Input.GetKey(KCverticalUp) - Input.GetKey(KCverticalDown);
        vertical = Input.GetAxisRaw("Vertical");
        horizontal = Input.GetAxisRaw("Horizontal");

        //ATTACK
        attack_Release_Input = Input.GetKeyUp(KCattack_Input);
        if (attack_Release_Input == false)
            attack_Release_Input = Input.GetKeyUp(gp_KCattack_Input);

        //Only Listening to attack if player has released the key between attacks
        if (attack_Input_WaitingForRelease == false)
        {
            attack_Input = Input.GetKey(KCattack_Input);
            if (attack_Input == false)
                attack_Input = Input.GetKey(gp_KCattack_Input);
        }
        else if (attack_Input_WaitingForRelease && attack_Release_Input)
        {
            attack_Input_WaitingForRelease = false;
            attack_Release_Input = false;
        }
        else
        {
            attack_Input = false;
        }

        if (attack_Input)
        {
            attack_Timer += delta;
        }

        //DODGE
        dodge_release_Input = Input.GetKeyUp(KCdodge_Input);
        if (dodge_release_Input == false)
            dodge_release_Input = Input.GetKeyUp(gp_KCdodge_Input);

        //Only Listening to dodge if player has released the key between dodges
        if (dodge_Input_WaitingForRelease == false)
        {
            dodge_Input = Input.GetKey(KCdodge_Input);
            if (dodge_Input == false)
                dodge_Input = Input.GetKey(gp_KCdodge_Input);
        }
        else if (dodge_Input_WaitingForRelease && dodge_release_Input)
        {
            dodge_Input_WaitingForRelease = false;
            dodge_release_Input = false;
        }
        else
        {
            dodge_Input = false;
        }

        if (dodge_Input)
        {
            dodge_Timer += delta;
        }

        lockon_Input = Input.GetKeyDown(KClockon_Input);
        if (lockon_Input == false)
            lockon_Input = Input.GetKeyDown(gp_KClockon_Input);

        jump_Input = Input.GetKeyDown(KCjump_Input);
        if (jump_Input == false)
            jump_Input = Input.GetKeyDown(gp_KCjump_Input);

        if (Input.GetKeyDown(KCpower1_Input) || Input.GetKeyDown(gp_KCpower1_Input))
        {
            power_Input = 1;
        }
        else if (Input.GetKeyDown(KCpower2_Input) || Input.GetKeyDown(gp_KCpower2_Input))
        {
            power_Input = 2;
        }
        else if (Input.GetKeyDown(KCpower3_Input) || Input.GetKeyDown(gp_KCpower3_Input))
        {
            power_Input = 3;
        }
    }

    void UpdateStates()
    {
        //Movement Input
        _stateController.horizontalInput = horizontal;
        _stateController.verticalInput = vertical;

        Vector3 dir = (vertical * transform.forward + horizontal * transform.right).normalized;
        if (dir != Vector3.zero)
            _stateController.movementDir = _stateController._Camera.TransformDirection(dir);

        float m = Mathf.Abs(vertical) + Mathf.Abs(horizontal);
        _stateController.moveAmount = Mathf.Clamp01(m);



        //Attacking Input
        if(attack_Release_Input && attack_Timer > attack_TimeToHeavy)
        {
            _stateController.heavyAttackInput = true;
            attack_Timer = 0;
        }
        else if(attack_Release_Input && attack_Timer <= attack_TimeToHeavy)
        {
            _stateController.quickAttackInput = true;
            attack_Timer = 0;
        }
        else if (attack_Timer >= attack_MaxHoldTime)
        {
            attack_Input_WaitingForRelease = true;
            _stateController.heavyAttackInput = true;
            attack_Timer = 0;
        }

        //Dodging Input
        if (dodge_release_Input && dodge_Timer > dodge_TimeToLong)
        {
            _stateController.longDodgeInput = true;
            dodge_Timer = 0;
        }
        else if (dodge_release_Input && dodge_Timer <= dodge_TimeToLong)
        {
            _stateController.shortDodgeInput = true;
            dodge_Timer = 0;
        }
        //Dodge if held for max time
        else if (dodge_Timer > dodge_MaxHoldTime)
        {
            dodge_Input_WaitingForRelease = true;
            _stateController.longDodgeInput = true;
            dodge_Timer = 0;
        }

        //LockOn Input
        if (lockon_Input)
        {
            _stateController.lockOnInput = true;
            lockon_Input = false;
        }

        //Jump Input
        _stateController.jumpInput = jump_Input;

        //Powers Input
        if (power_Input > 0)
        {
            _stateController.powerInput = power_Input;
            power_Input = 0;
        }

        if (Input.anyKeyDown || Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0 || horizontal != 0 || vertical != 0)
        {
            _stateController.LastInputTime = Time.time;
        }
    }
    
    void ResetInputAndTimers()
    {
        if (!attack_Input)
        {
            attack_Timer = 0;
        }
    }
}
