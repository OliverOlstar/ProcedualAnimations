using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelMovement : MonoBehaviour
{
    private Rigidbody _rb;
    private Animator _anim;

    [SerializeField] private float _rotationDampening = 1;
    [SerializeField] private float _rotationDeadzone = 0.02f;
    [SerializeField] private float _tiltingDampening = 1;
    [SerializeField] private float _tiltingMult = 5;

    private Vector3 _horizontalVelocity;

    public Vector3 acceleration;
    public float accelerationMag;

    [SerializeField] private float _steppingMult = 1;
    [SerializeField] private float _fallingAnimSpeed = 1;
    [SerializeField] private float _jumpingTransitionWidth = 1;
    public bool onGround = false;

    private float steppingWeight;
    private float jumpingWeight;

    [SerializeField] private float _weightChangeDampening = 1;


    void Start()
    {
        _rb = GetComponentInParent<Rigidbody>();
        _anim = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        _horizontalVelocity = new Vector3(_rb.velocity.z, 0, -_rb.velocity.x);

        UpdateWeights();
        LerpWeights();

        FacingSelf();
        TiltingParent();

        SteppingAnim();
        JumpingAnim();
    }

    private void UpdateWeights()
    {
        if (onGround)
        {
            steppingWeight = 0.5f;
            jumpingWeight = 0;
        }
        else
        {
            steppingWeight = 0;
            jumpingWeight = 0.5f;
        }
    }

    private void LerpWeights()
    {
        // Get total weight
        float totalWeight = steppingWeight + jumpingWeight;

        // Prevent Total Weight from going past 1
        float steppingWeightAvg = steppingWeight / totalWeight;
        float jumpingWeightAvg = jumpingWeight / totalWeight;

        // Lerp weight values
        _anim.SetFloat("Stepping Weight", Mathf.Lerp(_anim.GetFloat("Stepping Weight"), steppingWeightAvg, _weightChangeDampening * Time.deltaTime));
        _anim.SetFloat("Jumping Weight", Mathf.Lerp(_anim.GetFloat("Jumping Weight"), jumpingWeightAvg, _weightChangeDampening * Time.deltaTime));
    }

    private void JumpingAnim()
    {
        // Jumping Speed
        _anim.SetFloat("Jumping Speed", _rb.velocity.y / _jumpingTransitionWidth);

        // Falling Progress
        float progress = _anim.GetFloat("Falling Progress") + (Time.fixedDeltaTime * _fallingAnimSpeed);
        if (progress >= 1) progress -= 1;

        _anim.SetFloat("Falling Progress", progress);
    }

    private void SteppingAnim()
    {
        // Increase Stepping Animation
        float progress = _anim.GetFloat("Stepping Progress") + (Time.fixedDeltaTime * _horizontalVelocity.magnitude * _steppingMult);
        if (progress >= 1) progress -= 1;

        // Set Anim Stepping values
        _anim.SetFloat("Stepping Progress", progress);
        _anim.SetFloat("Stepping Speed", _horizontalVelocity.magnitude / GetComponentInParent<MovementComponent>().maxSpeed);
    }

    private void FacingSelf()
    {
        // Facing Velocity
        if (_horizontalVelocity.magnitude > _rotationDeadzone)
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.LookRotation(_horizontalVelocity, Vector3.up), Time.deltaTime * _rotationDampening);
    }

    private void TiltingParent()
    {
        // Save eularAngle
        Vector3 eulerAngles = transform.parent.localEulerAngles;

        // Tilting alter values to prevent jumping from 0 to 360
        if (eulerAngles.x > 180)
            eulerAngles.x = eulerAngles.x - 360;
        if (eulerAngles.z > 180)
            eulerAngles.z = eulerAngles.z - 360;

        // Lerp tilting values
        float horizontalAngle = Mathf.Lerp(eulerAngles.x, -acceleration.x * _tiltingMult, Time.deltaTime * _tiltingDampening);
        float verticalAngle = Mathf.Lerp(eulerAngles.z, -acceleration.z * _tiltingMult, Time.deltaTime * _tiltingDampening);
        
        eulerAngles = new Vector3(horizontalAngle, eulerAngles.y, verticalAngle);

        // Update eularAngle
        transform.parent.localEulerAngles = eulerAngles;
    }

    private float easeInOutSine(float pTime, float pStartValue, float pChangeInValue, float pDuration)
    {
        return -pChangeInValue / 2 * (Mathf.Cos(Mathf.PI * pTime / pDuration) - 1) + pStartValue;
    }
}
