using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGround : MonoBehaviour
{
    [SerializeField] private float _isGroundedCheckDistance = 1.0f;
    [SerializeField] private float _respawnYOffset = 1;
    private Vector3 _lastPoint = new Vector3(0,0,0);

    [Header("Fall Damage")]
    [SerializeField] private float _fallMaxTime = 2;
    [SerializeField] private float _fallDamageStartTime = 1;
    [SerializeField] private int _fallDamageMax = 40;
    [SerializeField] private int _fallDamageMin = 20;
    private float _terminalFallingTimer = 0;

    [Space]
    [SerializeField] private float _inputInfluenceGrounded = 1.0f;
    [SerializeField] private float _inputInfluenceInAir = 0.7f;
    [SerializeField] private float _influenceUpdateRate = 1.0f;

    [Space]
    [SerializeField] private float _downForceRate = 6f;
    [SerializeField] private float _downForceTerminal = 4f;
    private float _downForce = 0;

    private MovementComponent _moveComponent;
    private Rigidbody _rb;

    private void Awake()
    {
        _moveComponent = GetComponent<MovementComponent>();
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //Falling Force (Add extra force to falling to make falling feel better)
        //if (_stateController._movementComponent.disableMovement == false)
        FallingForce();

        //Check if on the ground
        CheckGrounded();

        //Damage player if they fall for too long and teleport them back to ground
        CheckFellTeleport();
    }
    
    private void CheckGrounded()
    {
        //Raycast to check for if grounded
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, _isGroundedCheckDistance))
        {
            _moveComponent.onGround = true;
            _lastPoint = hit.point;

            CheckFellLanding();
            _terminalFallingTimer = 0;
        }
        else
        {
            _moveComponent.onGround = false;

            if (_downForce >= _downForceTerminal)
                _terminalFallingTimer += Time.deltaTime;
        }
    }

    private void CheckFellTeleport()
    {
        //If falling for max time, teleport back to last place on ground
        if (_terminalFallingTimer >= _fallMaxTime)
        {
            _rb.velocity = Vector3.zero;
            transform.position = _lastPoint + new Vector3(0, _respawnYOffset, 0);
            _terminalFallingTimer = 0;
        }
    }

    private void CheckFellLanding()
    {
        if (_terminalFallingTimer >= _fallDamageStartTime)
        {
            float fallPercent = (_terminalFallingTimer - _fallDamageStartTime) / (_fallMaxTime - _fallDamageStartTime);
            //int fallDamage = Mathf.RoundToInt(fallDamageMax * fallPercent + fallDamageMin * (1 - fallPercent));
        }
    }

    public void FallingForce()
    {
        //Change the amount of influence player input has on the player movement based on wether he is grounded or not
        if (_moveComponent.onGround)
        {
            _moveComponent.inputInfluence = _inputInfluenceGrounded;
            _downForce = 0;
        }
        else
        {
            _moveComponent.inputInfluence = _inputInfluenceInAir;

            //Add force downwards which adds ontop of gravity
            if (_downForce < _downForceTerminal)
                _downForce += _downForceRate * Time.deltaTime;
            else
                _downForce = _downForceTerminal;
        }

        _rb.AddForce(Vector3.down * Mathf.Pow(_downForce, 2));
    }

    private void LerpInputInfluence(float pTarget)
    {
        //Update inputInflunce to target
        //_moveComponent.inputInfluence = Mathf.Lerp(_moveComponent.inputInfluence, pTarget, influenceUpdateRate * Time.deltaTime);
    }
}
