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
    private bool _Dodging;
    
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
        _modelMovement.Init(this);
    }

    void FixedUpdate()
    {
        horizontalVelocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);

        if (_Dodging == false)
        {
            if (_Attacking == true)
            {
                if (_modelAnimation.AttackingAnim())
                    DoneAttack();
            }
            else
            {
                _modelWeights.UpdateWeights();
            }
        }

        _modelWeights.LerpWeights();

        _modelMovement.TiltingParent();
        _modelMovement.FacingSelf();

        _modelAnimation.SteppingAnim();
        _modelAnimation.JumpingAnim();
    }

    public void PlayAttack()
    {
        _Attacking = true;
        _modelMovement.DisableRotation = true;
        _modelWeights.SetWeights(0, 0, 0, 1, 0);
        _modelAnimation.ResetAttack();
    }

    public void DoneAttack()
    {
        _Attacking = false;
        _modelMovement.DisableRotation = false;
        _modelWeights.SetWeights(0, 0, 0, 0, 0);
    }

    public void PlayDodge(Vector2 pDirection, float pSpeed)
    {
        _Dodging = true;
        _modelMovement.DisableRotation = true;
        _modelWeights.SetWeights(0, 0, 0, 0, 1);
        _modelMovement.PlayFlipParent(pDirection, pSpeed);
    }

    public void DoneDodge()
    {
        _Dodging = false;
        _modelMovement.DisableRotation = false;
        _modelWeights.SetWeights(0, 0, 0, 0, 0);
    }

    public Vector2 GetCatmullRomPosition(float pTime, SOGraph pGraph)
    {
        Vector2 startPoint = Vector2.zero;
        Vector2 endPoint = new Vector2(1, pGraph.EndValue);

        Vector2 a = 2f * startPoint;
        Vector2 b = endPoint - pGraph.firstBender;
        Vector2 c = 2f * pGraph.firstBender - 5f * startPoint + 4f * endPoint - pGraph.secondBender;
        Vector2 d = -pGraph.firstBender + 3f * startPoint - 3f * endPoint + pGraph.secondBender;

        Vector2 pos = 0.5f * (a + (b * pTime) + (c * pTime * pTime) + (d * pTime * pTime * pTime));

        return pos;
    }
}
