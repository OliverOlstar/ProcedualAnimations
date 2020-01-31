using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelAnimations : MonoBehaviour
{
    private ModelController _modelController;
    private Rigidbody _rb;
    private Animator _anim;

    [Header("Animation Lengths")]
    [SerializeField] private float _stepMult = 1;
    [SerializeField] private float _fallMult = 1;
    [SerializeField] private float _attackLength = 1;

    private float _stepProgress = 0;
    private float _fallProgress = 0;
    private float _attackProgress = 0;

    [Header("Interpolation Graphs")]
    [SerializeField] private SOGraph _stepGraph;
    [SerializeField] private SOGraph _fallGraph;
    [SerializeField] private SOGraph _attackGraph;
    
    [Space]
    [SerializeField] private float _jumpingTransitionWidth = 1;

    public void Init(ModelController pController, Rigidbody pRb, Animator pAnim)
    {
        _modelController = pController;
        _rb = pRb;
        _anim = pAnim;
    }

    public bool AttackingAnim()
    {
        // Increase Falling Animation
        _attackProgress = Mathf.Min(1, _attackProgress + Time.fixedDeltaTime / _attackLength * _modelController.animSpeed);
        _anim.SetFloat("Attacking Progress", _modelController.GetCatmullRomPosition(_attackProgress, _attackGraph).y);
         
        return _attackProgress == 1;
    }

    public void StartAttack(int pIndex)
    {
        _attackProgress = 0;
        _anim.SetFloat("Attacking Progress", 0);
        _anim.SetFloat("Attacking Index", pIndex);
    }

    public void JumpingAnim()
    {
        // Jumping Speed
        _anim.SetFloat("Jumping Speed", _rb.velocity.y / _jumpingTransitionWidth);

        // Increase Falling Animation
        _fallProgress = increaseProgress(_fallProgress, _fallMult);
        _anim.SetFloat("Falling Progress", _modelController.GetCatmullRomPosition(_fallProgress, _fallGraph).y);
    }

    public void SteppingAnim()
    {
        // Increase Stepping Animation
        _stepProgress = increaseProgress(_stepProgress, _stepMult);

        float secondStep = (_stepProgress <= 0.5f) ? 0 : 0.5f;
        float time = (_stepProgress - secondStep) * 2;

        float steppingSpeed = _modelController.horizontalVelocity.magnitude / GetComponentInParent<MovementComponent>().maxSpeed;

        // Set Anim Stepping values
        _anim.SetFloat("Stepping Progress", _modelController.GetCatmullRomPosition(time, _stepGraph).y + secondStep);
        _anim.SetFloat("Stepping Speed", (steppingSpeed > 1) ? 1 : steppingSpeed);

        // Set Anim Direction
        Vector3 relDirection = transform.rotation * new Vector3(-_modelController.horizontalVelocity.x, 0, _modelController.horizontalVelocity.z);
        relDirection.y = 0;
        relDirection.Normalize();

        _anim.SetFloat("Move Direction X", relDirection.x);
        _anim.SetFloat("Move Direction Z", relDirection.z);
    }

    private float increaseProgress(float pProgress, float pMult)
    {
        pProgress += Time.fixedDeltaTime * pMult * _modelController.animSpeed;
        if (pProgress >= 1)
            pProgress -= 1;

        return pProgress;
    }
}
