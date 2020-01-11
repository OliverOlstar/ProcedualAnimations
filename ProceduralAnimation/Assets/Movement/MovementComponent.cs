using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementComponent : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody _Rb;

    [Header("Movement")]
    public float moveSpeed = 1.0f;
    public float maxSpeed = 4.0f;

    [Header("Jump")]
    public float jumpDistance = 5;
    public float jumpDuration = 5;
    public float jumpForceUp = 4;

    public bool OnGround;

    void Start()
    {
        _Rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //Movement
        PlayerMove();

        //Jump
        Jump();
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (OnGround)
            {
                //Add force
                _Rb.AddForce(jumpForceUp * _Rb.mass * Vector3.up, ForceMode.Impulse);
            }
        }
    }

    private void PlayerMove()
    {
        //Getting Input
        float translation = Input.GetAxis("Vertical");
        float straffe = Input.GetAxis("Horizontal");

        //Move Vector
        Vector3 move = new Vector3(straffe, 0, translation);
        move = Camera.main.transform.TransformDirection(move);
        move = new Vector3(move.x, 0, move.z);
        move = move.normalized;

        //Moving the player
        move = move * Time.deltaTime * moveSpeed * _Rb.mass;
        if (new Vector3(_Rb.velocity.x, 0, _Rb.velocity.z).magnitude < maxSpeed)
            _Rb.AddForce(move);

    }

    public void EndJump()
    {
        StopCoroutine("JumpRoutine");
    }
}
