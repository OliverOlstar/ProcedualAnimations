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

    [SerializeField] private SOGraph _steppingGraph;

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
        float time = (stepProgress - secondStep) * 2;
        Debug.Log(time);

        // Set Anim Stepping values
        _anim.SetFloat("Stepping Progress", GetCatmullRomPosition(time, _steppingGraph).y + secondStep);
        _anim.SetFloat("Stepping Speed", _horizontalVelocity.magnitude);
        _anim.SetFloat("Stepping Direction X", 1);
    }

    private Vector2 GetCatmullRomPosition(float pTime, SOGraph pGraph)
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
