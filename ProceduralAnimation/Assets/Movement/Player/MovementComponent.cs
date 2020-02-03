using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementComponent : MonoBehaviour
{
    private Rigidbody _rb;
    private PlayerStateController _stateController;

    [Header("Movement")]
    [SerializeField] private float _moveAcceleration = 1.0f;
    public float maxSpeed = 4.0f;

    public float inputInfluence = 1.0f;

    [Header("Jump")]
    [SerializeField] private float _jumpForceUp = 4;
    [SerializeField] private float _jumpForceVelocityMult = 1.7f;

    [Space]
    public bool disableMovement = false;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _stateController = GetComponent<PlayerStateController>();
    }

    void Update()
    {
        if (disableMovement) 
            return;

        //Movement
        Move();
    }

    private void OnJump()
    {
        if (_stateController.onGround && disableMovement == false)
        {
            //Add force
            _rb.velocity = new Vector3(_stateController._rb.velocity.x * _jumpForceVelocityMult, 0, _stateController._rb.velocity.z * _jumpForceVelocityMult);
            _rb.AddForce(_jumpForceUp * Vector3.up, ForceMode.Impulse);

            _stateController._modelController.AddCrouching(1, 0.1f, 0.05f);
        }
    }

    private void Move()
    {
        //Move Vector
        Vector3 move = new Vector3(_stateController.moveInput.x, 0, _stateController.moveInput.y);
        move = _stateController._Camera.TransformDirection(move);
        move.y = 0;
        move = move.normalized * Time.deltaTime * _moveAcceleration * inputInfluence;

        //Moving the player
        Vector3 horizontalVelocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
        float nextMagnitudeWithInput = (horizontalVelocity + move * Time.deltaTime).magnitude;

        if (horizontalVelocity.magnitude < maxSpeed || nextMagnitudeWithInput <= horizontalVelocity.magnitude)
        {
            _rb.AddForce(move);
        }

        _stateController._modelController.acceleration = move.normalized;
        _stateController._modelController.onGround = _stateController.onGround;

        if (move.magnitude != 0)
            _stateController.LastMoveDirection = new Vector2(move.x, move.z).normalized;
    }
}
