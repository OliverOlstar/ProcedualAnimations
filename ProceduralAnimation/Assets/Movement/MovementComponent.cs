using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementComponent : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody _rb;
    private PlayerStateController _stateController;

    [Header("Movement")]
    public float moveAcceleration = 1.0f;
    public float maxSpeed = 4.0f;

    public float inputInfluence = 1.0f;

    [Header("Jump")]
    public float jumpForceUp = 4;

    public bool OnGround;

    public bool disableMovement = false;

    [HideInInspector] public Vector2 moveInput;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _stateController = GetComponent<PlayerStateController>();
        _stateController.inputActions.Player.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        _stateController.inputActions.Player.Jump.performed += ctx => Jump();
    }

    void Update()
    {
        if (disableMovement) 
            return;

        //Movement
        Move();
    }

    private void Jump()
    {
        if (OnGround && disableMovement == false)
        {
            //Add force
            _rb.velocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
            _rb.AddForce(jumpForceUp * Vector3.up, ForceMode.Impulse);
        }
    }

    private void Move()
    {
        //Move Vector
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        if (Camera.main != null)
            move = Camera.main.transform.TransformDirection(move);
        move.y = 0;
        move = move.normalized * Time.deltaTime * moveAcceleration * inputInfluence;

        //Moving the player
        if (new Vector3(_rb.velocity.x, 0, _rb.velocity.z).magnitude < maxSpeed)
            _rb.AddForce(move);

        _stateController._modelController.acceleration = move.normalized;

        if (move.magnitude != 0)
            _stateController.LastMoveDirection = new Vector2(move.x, move.z).normalized;
    }
}
