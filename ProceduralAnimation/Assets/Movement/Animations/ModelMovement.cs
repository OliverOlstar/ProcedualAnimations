using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelMovement : MonoBehaviour
{
    private ModelController _modelController;
    private Animator _anim;

    [SerializeField] private float _rotationDampening = 1;
    [SerializeField] private float _rotationDeadzone = 0.02f;
    
    [Space]
    [SerializeField] private float _tiltingDampening = 1;
    [SerializeField] private float _tiltingMult = 5;

    public bool DisableRotation;

    public void Init(ModelController pController, Animator pAnim)
    {
        _modelController = pController;
        _anim = pAnim;
    }

    public void FacingSelf()
    {
        if (DisableRotation) 
            return;

        // Facing Velocity
        if (_modelController.horizontalVelocity.magnitude > _rotationDeadzone)
        {
            Quaternion targetQuaternion = Quaternion.LookRotation(new Vector3(_modelController.horizontalVelocity.z, 0, -_modelController.horizontalVelocity.x), Vector3.up);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetQuaternion, Time.deltaTime * _rotationDampening * _modelController.animSpeed);
        }
    }

    public void TiltingParent()
    {
        // Save eularAngle
        Vector3 eulerAngles = transform.parent.localEulerAngles;

        // Tilting alter values to prevent jumping from 0 to 360
        if (eulerAngles.x > 180)
            eulerAngles.x = eulerAngles.x - 360;
        if (eulerAngles.z > 180)
            eulerAngles.z = eulerAngles.z - 360;

        // Lerp tilting values
        float horizontalAngle = Mathf.Lerp(eulerAngles.x, -_modelController.acceleration.x * _tiltingMult, Time.deltaTime * _tiltingDampening * _modelController.animSpeed);
        float verticalAngle = Mathf.Lerp(eulerAngles.z, -_modelController.acceleration.z * _tiltingMult, Time.deltaTime * _tiltingDampening * _modelController.animSpeed);
        
        eulerAngles = new Vector3(horizontalAngle, eulerAngles.y, verticalAngle);

        // Update eularAngle
        transform.parent.localEulerAngles = eulerAngles;
    }
}
