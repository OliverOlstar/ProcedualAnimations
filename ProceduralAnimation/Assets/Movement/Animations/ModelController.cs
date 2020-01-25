using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelController : MonoBehaviour
{
    private Rigidbody _rb;
    private Animator _anim;

    [HideInInspector] public bool onGround = false;
    [HideInInspector] public Vector3 acceleration;
    [Range(0, 1)] public float animSpeed = 1;

    private ModelWeights _modelWeights;
    private ModelAnimations _modelAnimation;
    private ModelMovement _modelMovement;

    [HideInInspector] public Vector3 horizontalVelocity;
    private bool _Attacking;
    
    void Start()
    {
        // Get Model Components
        _modelWeights = GetComponent<ModelWeights>();
        _modelAnimation = GetComponent<ModelAnimations>();
        _modelMovement = GetComponent<ModelMovement>();

        // Get Other Components
        _rb = GetComponentInParent<Rigidbody>();
        _anim = GetComponent<Animator>();

        // Setup Components
        _modelWeights.Init(this, _anim);
        _modelAnimation.Init(this, _rb, _anim);
        _modelMovement.Init(this, _anim);
    }

    void FixedUpdate()
    {
        horizontalVelocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);

        _modelWeights.UpdateWeights();
        _modelWeights.LerpWeights();

        _modelMovement.TiltingParent();
        _modelMovement.FacingSelf();

        _modelAnimation.SteppingAnim();
        _modelAnimation.JumpingAnim();

        if (_Attacking)
            _modelAnimation.AttackingAnim();
    }

    public void PlayAttack()
    {
        _Attacking = true;
        _modelWeights.SetWeights(0, 0, 0, 1);
    }
}
