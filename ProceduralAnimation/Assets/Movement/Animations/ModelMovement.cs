using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelMovement : MonoBehaviour
{
    private Animator _anim;

    [SerializeField] private float _rotationDampening = 1;
    [SerializeField] private float _rotationDeadzone = 0.02f;
    
    [Space]
    [SerializeField] private float _tiltingDampening = 1;
    [SerializeField] private float _tiltingMult = 5;

    public void Init(Animator pAnim)
    {
        _anim = pAnim;
    }

    public void FacingSelf(Vector3 pHorizontalVelocity)
    {
        // Facing Velocity
        if (pHorizontalVelocity.magnitude > _rotationDeadzone)
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.LookRotation(pHorizontalVelocity, Vector3.up), Time.deltaTime * _rotationDampening);
    }

    public void TiltingParent(Vector3 pAcceleration)
    {
        // Save eularAngle
        Vector3 eulerAngles = transform.parent.localEulerAngles;

        // Tilting alter values to prevent jumping from 0 to 360
        if (eulerAngles.x > 180)
            eulerAngles.x = eulerAngles.x - 360;
        if (eulerAngles.z > 180)
            eulerAngles.z = eulerAngles.z - 360;

        // Lerp tilting values
        float horizontalAngle = Mathf.Lerp(eulerAngles.x, -pAcceleration.x * _tiltingMult, Time.deltaTime * _tiltingDampening);
        float verticalAngle = Mathf.Lerp(eulerAngles.z, -pAcceleration.z * _tiltingMult, Time.deltaTime * _tiltingDampening);
        
        eulerAngles = new Vector3(horizontalAngle, eulerAngles.y, verticalAngle);

        // Update eularAngle
        transform.parent.localEulerAngles = eulerAngles;
    }
}
