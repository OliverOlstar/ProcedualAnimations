using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelController : MonoBehaviour
{
    private Rigidbody _rb;
    private Animator _anim;

    public bool onGround = false;
    public Vector3 acceleration;

    private ModelWeights _modelWeights;
    private ModelAnimations _modelAnimation;
    private ModelMovement _modelMovement;

    private Vector3 _horizontalVelocity;

    // Start is called before the first frame update
    void Start()
    {
        _modelWeights = GetComponent<ModelWeights>();
        _modelAnimation = GetComponent<ModelAnimations>();
        _modelMovement = GetComponent<ModelMovement>();

        _rb = GetComponentInParent<Rigidbody>();
        _anim = GetComponent<Animator>();

        _modelWeights.Init(_anim);
        _modelAnimation.Init(_rb, _anim);
        _modelMovement.Init(_anim);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _horizontalVelocity = new Vector3(_rb.velocity.z, 0, -_rb.velocity.x);

        _modelWeights.UpdateWeights(onGround);
        _modelWeights.LerpWeights();

        _modelMovement.TiltingParent(acceleration);
        _modelMovement.FacingSelf(_horizontalVelocity);

        _modelAnimation.SteppingAnim(_horizontalVelocity);
        _modelAnimation.JumpingAnim();
    }
}
