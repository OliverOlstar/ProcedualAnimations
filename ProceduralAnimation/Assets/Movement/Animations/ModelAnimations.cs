using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelAnimations : MonoBehaviour
{
    private Rigidbody _rb;
    private Animator _anim;

    [SerializeField] private float _steppingMult = 1;
    [SerializeField] private float _fallingAnimSpeed = 1;
    [SerializeField] private float _jumpingTransitionWidth = 1;

    [Space]
    [SerializeField] [Range(0, 1)] private float _stepProgress = 0;

    public void Init(Rigidbody pRb, Animator pAnim)
    {
        _rb = pRb;
        _anim = pAnim;
    }

    public void JumpingAnim()
    {
        // Jumping Speed
        _anim.SetFloat("Jumping Speed", _rb.velocity.y / _jumpingTransitionWidth);

        // Falling Progress
        float progress = _anim.GetFloat("Falling Progress") + (Time.fixedDeltaTime * _fallingAnimSpeed);
        if (progress >= 1) progress -= 1;

        _anim.SetFloat("Falling Progress", progress);
    }

    public void SteppingAnim(Vector3 pHorizontalVelocity)
    {
        // Increase Stepping Animation
        _stepProgress += Time.fixedDeltaTime * _steppingMult;
        if (_stepProgress >= 1)
            _stepProgress -= 1;

        float secondStep = (_stepProgress <= 0.5f) ? 0 : 0.5f;

        float steppingSpeed = pHorizontalVelocity.magnitude / GetComponentInParent<MovementComponent>().maxSpeed;

        // Set Anim Stepping values
        _anim.SetFloat("Stepping Progress", easeInOutSine((_stepProgress - secondStep) * 2, 0.25f, 0.5f) + secondStep);
        _anim.SetFloat("Stepping Speed", (steppingSpeed > 1) ? 1 : steppingSpeed);
        //_anim.SetVector("Stepping Relative Direction", _horizontalVelocity - new Vector2(transform.forward.x, transform.forward.z));
    }

    private float easeInOutSine(float pTime, float pChangeInValue, float pDuration)
    {
        return -pChangeInValue * Mathf.Cos((Mathf.PI / 2) * (pTime / pDuration)) + pChangeInValue;
    }
}
