using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelWeights : MonoBehaviour
{
    private Animator _anim;

    [SerializeField] [Range(0, 1)] private float stepWeight = 0;
    [SerializeField] [Range(0, 1)] private float jumpWeight = 0;
    [SerializeField] [Range(0, 1)] private float crouchWeight = 0;

    [Space]
    [SerializeField] private float _weightChangeDampening = 1;
    [SerializeField] private float _weightChangeDeadzone = 0.1f;

    public void Init(Animator pAnim)
    {
        _anim = pAnim;
    }

    public void UpdateWeights(bool pOnGround)
    {
        if (pOnGround)
        {
            stepWeight = 1f;
            jumpWeight = 0;
        }
        else
        {
            stepWeight = 0;
            jumpWeight = 1f;
        }
    }

    public void LerpWeights()
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
}
