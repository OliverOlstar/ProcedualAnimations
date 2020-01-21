using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelAnimations : MonoBehaviour
{
    private ModelController _modelController;
    private Rigidbody _rb;
    private Animator _anim;

    [SerializeField] private float _steppingMult = 1;
    [SerializeField] private float _fallingAnimSpeed = 1;
    [SerializeField] private float _jumpingTransitionWidth = 1;

    [Space]
    private float _stepProgress = 0;

    public void Init(ModelController pController, Rigidbody pRb, Animator pAnim)
    {
        _modelController = pController;
        _rb = pRb;
        _anim = pAnim;
    }

    public void JumpingAnim()
    {
        // Jumping Speed
        _anim.SetFloat("Jumping Speed", _rb.velocity.y / _jumpingTransitionWidth);

        // Falling Progress
        float progress = _anim.GetFloat("Falling Progress") + (Time.fixedDeltaTime * _fallingAnimSpeed * _modelController.animSpeed);
        if (progress >= 1) progress -= 1;

        _anim.SetFloat("Falling Progress", progress);
    }

    public void SteppingAnim()
    {
        // Increase Stepping Animation
        _stepProgress += Time.fixedDeltaTime * _steppingMult * _modelController.animSpeed;
        if (_stepProgress >= 1)
            _stepProgress -= 1;

        float secondStep = (_stepProgress <= 0.5f) ? 0 : 0.5f;

        float steppingSpeed = _modelController.horizontalVelocity.magnitude / GetComponentInParent<MovementComponent>().maxSpeed;

        // Set Anim Stepping values
        _anim.SetFloat("Stepping Progress", easeInOutSine((_stepProgress - secondStep) * 2, 0.25f, 0.5f) + secondStep);
        _anim.SetFloat("Stepping Speed", (steppingSpeed > 1) ? 1 : steppingSpeed);

        // Set Anim Direction
        Vector3 relDirection = transform.TransformDirection(_modelController.horizontalVelocity);
        relDirection.y = 0;
        relDirection.Normalize();

        _anim.SetFloat("Stepping Direction X", relDirection.x);
        _anim.SetFloat("Stepping Direction Y", relDirection.z);
    }

    private float easeInOutSine(float pTime, float pChangeInValue, float pDuration)
    {
        return -pChangeInValue * Mathf.Cos((Mathf.PI / 2) * (pTime / pDuration)) + pChangeInValue;
    }
}
