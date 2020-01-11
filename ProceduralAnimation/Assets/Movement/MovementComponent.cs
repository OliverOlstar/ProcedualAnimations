using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementComponent : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody _rb;

    [Header("Movement")]
    public float moveAcceleration = 1.0f;
    public float maxSpeed = 4.0f;

    public float inputInfluence = 1.0f;

    [Header("Jump")]
    public float jumpDistance = 5;
    public float jumpDuration = 5;
    public float jumpForceUp = 4;

    public bool OnGround;

    public bool disableMovement = false;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (disableMovement) 
            return;

        //Movement
        Move();

        //Jump
        Jump();
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && OnGround)
        {
            //Add force
            _rb.AddForce(jumpForceUp * Vector3.up, ForceMode.Impulse);
        }
    }

    private void Move()
    {
        //Getting Input
        float translation = Input.GetAxis("Vertical");
        float straffe = Input.GetAxis("Horizontal");

        //Move Vector
        Vector3 move = new Vector3(straffe, 0, translation);
        move = Camera.main.transform.TransformDirection(move);
        move.y = 0;
        move = move.normalized * Time.deltaTime * moveAcceleration * inputInfluence;

        //Moving the player
        Debug.Log(new Vector3(_rb.velocity.x, 0, _rb.velocity.z).magnitude);
        if (new Vector3(_rb.velocity.x, 0, _rb.velocity.z).magnitude < maxSpeed)
            _rb.AddForce(move);
    }
}
