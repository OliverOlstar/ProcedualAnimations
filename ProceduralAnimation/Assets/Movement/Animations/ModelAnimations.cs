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

    private float _stepProgress = 0;

    [Header("Interpolation Graphs")]
    [SerializeField] private SOGraph _stepGraph;

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
        float time = (_stepProgress - secondStep) * 2;

        float steppingSpeed = _modelController.horizontalVelocity.magnitude / GetComponentInParent<MovementComponent>().maxSpeed;

        // Set Anim Stepping values
        _anim.SetFloat("Stepping Progress", GetCatmullRomPosition(time, _stepGraph).y + secondStep);
        _anim.SetFloat("Stepping Speed", (steppingSpeed > 1) ? 1 : steppingSpeed);

        // Set Anim Direction
        Vector3 relDirection = transform.rotation * new Vector3(-_modelController.horizontalVelocity.x, 0, _modelController.horizontalVelocity.z);
        relDirection.y = 0;
        relDirection.Normalize();

        _anim.SetFloat("Stepping Direction X", relDirection.x);
        _anim.SetFloat("Stepping Direction Z", relDirection.z);
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
