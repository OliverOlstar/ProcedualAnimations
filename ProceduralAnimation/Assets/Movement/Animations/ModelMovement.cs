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

    public Vector3 acceleration;
    public float accelerationMag;

    [SerializeField] private float _steppingMult = 1;


    void Start()
    {
        _rb = GetComponentInParent<Rigidbody>();
        _anim = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        FacingSelf();
        TiltingParent();

        SteppingAnim();
    }

    private void SteppingAnim()
    {
        float progress = _anim.GetFloat("Stepping Progress") + (Time.fixedDeltaTime * _rb.velocity.magnitude * _steppingMult);
        if (progress >= 1) progress -= 1;

        _anim.SetFloat("Stepping Progress", progress);
        _anim.SetFloat("Stepping Speed", _rb.velocity.magnitude / GetComponentInParent<MovementComponent>().maxSpeed);
    }

    private void FacingSelf()
    {
        Vector3 horizontalVelocity = new Vector3(_rb.velocity.z, 0, -_rb.velocity.x);

        // Facing Velocity
        if (horizontalVelocity.magnitude > _rotationDeadzone)
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.LookRotation(horizontalVelocity, Vector3.up), Time.deltaTime * _rotationDampening);
    }

    private void TiltingParent()
    {
        Vector3 eulerAngles = transform.parent.localEulerAngles;

        // Tilting
        if (eulerAngles.x > 180)
            eulerAngles.x = eulerAngles.x - 360;
        if (eulerAngles.z > 180)
            eulerAngles.z = eulerAngles.z - 360;

        float horizontalAngle = Mathf.Lerp(eulerAngles.x, -acceleration.x * _tiltingMult, Time.deltaTime * _tiltingDampening);
        float verticalAngle = Mathf.Lerp(eulerAngles.z, -acceleration.z * _tiltingMult, Time.deltaTime * _tiltingDampening);

        eulerAngles = new Vector3(horizontalAngle, eulerAngles.y, verticalAngle);

        transform.parent.localEulerAngles = eulerAngles;
    }

    private float easeInOutSine(float pTime, float pStartValue, float pChangeInValue, float pDuration)
    {
        return -pChangeInValue / 2 * (Mathf.Cos(Mathf.PI * pTime / pDuration) - 1) + pStartValue;
    }
}
