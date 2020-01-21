using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterpolationTesting : MonoBehaviour
{
    private Animator _anim;

    [SerializeField] private Vector2 _horizontalVelocity;

    [SerializeField] private float _steppingMult = 1;

    [SerializeField] [Range(0,1)] private float stepWeight = 0;
    [SerializeField] [Range(0, 1)] private float jumpWeight = 0;
    [SerializeField] [Range(0, 1)] private float crouchWeight = 0;

    [Space]
    [SerializeField] [Range(0,1)] private float stepProgress = 0;

    [SerializeField] private float _weightChangeDampening = 10;
    [SerializeField] private float _weightChangeDeadzone = 0.1f;

    void Start()
    {
        _anim = GetComponent<Animator>();
        _anim.SetFloat("Stepping Weight", 1);
        _anim.SetFloat("Jumping Weight", 0);
    }

    void FixedUpdate()
    {
        LerpWeights();

        SteppingAnim();
    }

    #region Weight Functions
    private void LerpWeights()
    {
        // Get total weight (Used to prevent Total Weight from going past 1)
        float totalWeight = stepWeight + jumpWeight + crouchWeight;
        if (totalWeight <= 1)
            totalWeight = 1;

        // Lerp weight values
        LerpWeight("Stepping Weight", stepWeight / totalWeight);
        LerpWeight("Jumping Weight", jumpWeight / totalWeight);
        LerpWeight("Crouching Weight", crouchWeight / totalWeight);
    }

    private void LerpWeight(string pWeight, float pTargetValue)
    {
        // Get current weight value
        float currentValue = _anim.GetFloat(pWeight);

        // Return if value is already at target
        if (currentValue == pTargetValue) return;

        // Lerp value towards target
        currentValue = Mathf.Lerp(currentValue, pTargetValue, _weightChangeDampening * Time.deltaTime);

        // If in deadzone just snap to value
        if (Mathf.Abs(currentValue - pTargetValue) < _weightChangeDeadzone)
            currentValue = pTargetValue;

        // Update Anim
        _anim.SetFloat(pWeight, currentValue);
    }
    #endregion

    private void SteppingAnim()
    {
        // Increase Stepping Animation
        stepProgress += Time.fixedDeltaTime * _steppingMult;
        if (stepProgress >= 1) 
            stepProgress -= 1;

        float secondStep = (stepProgress <= 0.5f) ? 0 : 0.5f;

        // Set Anim Stepping values
        _anim.SetFloat("Stepping Progress", easeInOutSine((stepProgress - secondStep) * 2, 0.25f, 0.5f) + secondStep);
        _anim.SetFloat("Stepping Speed", _horizontalVelocity.magnitude);
        //_anim.SetVector("Stepping Relative Direction", _horizontalVelocity - new Vector2(transform.forward.x, transform.forward.z));
    }

    //private void IncreaseProgress(string pProgress,)

    private float easeInOutSine(float pTime, float pChangeInValue, float pDuration)
    {
        return -pChangeInValue * Mathf.Cos((Mathf.PI / 2) * (pTime / pDuration)) + pChangeInValue;
    }
}
