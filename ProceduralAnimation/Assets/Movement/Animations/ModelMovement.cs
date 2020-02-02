using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelMovement : MonoBehaviour
{
    private ModelController _modelController;

    [Header("Rotation")]
    [SerializeField] private float _rotationDampening = 1;
    [SerializeField] private float _rotationDeadzone = 0.02f;
    
    [Header("Tilt")]
    [SerializeField] private float _tiltingDampening = 1;
    [SerializeField] private float _tiltingMult = 5;

    [Header("Flip")]
    [SerializeField] private SOGraph _flipGraph;
    private float _flipProgress;
    private float _flipYRotation;

    private Quaternion _parentDefaultRotation;

    public bool DisableRotation;

    public void Init(ModelController pController)
    {
        _modelController = pController;
        _parentDefaultRotation = transform.parent.rotation;
    }

    public void FacingSelf()
    {
        if (DisableRotation) 
            return;

        // Facing Velocity
        if (_modelController.horizontalVelocity.magnitude > _rotationDeadzone)
        {
            Quaternion targetQuaternion = Quaternion.LookRotation(new Vector3(_modelController.horizontalVelocity.z, 0, -_modelController.horizontalVelocity.x), Vector3.up);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetQuaternion, Time.deltaTime * _rotationDampening);
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
        float horizontalAngle = Mathf.Lerp(eulerAngles.x, -_modelController.acceleration.x * _tiltingMult, Time.deltaTime * _tiltingDampening);
        float verticalAngle = Mathf.Lerp(eulerAngles.z, -_modelController.acceleration.z * _tiltingMult, Time.deltaTime * _tiltingDampening);

        eulerAngles = new Vector3(horizontalAngle, eulerAngles.y, verticalAngle);

        // Update eularAngle
        transform.parent.localEulerAngles = eulerAngles;
    }

    public void PlayFlipParent(Vector2 pDirection, float pSpeed)
    {
        Vector3 direction = new Vector3(pDirection.x, 0, pDirection.y);
        _flipYRotation = Quaternion.LookRotation(direction.normalized, Vector3.up).eulerAngles.y;

        StopCoroutine("FlipParentRoutine");
        StartCoroutine("FlipParentRoutine", pSpeed);
    }

    IEnumerator FlipParentRoutine(float pSpeed)
    {
        DisableRotation = true;

        // Setup for Flip
        Quaternion originalRotation = transform.localRotation;
        transform.localEulerAngles = new Vector3(0, -_flipYRotation + transform.eulerAngles.y, 0);
        _flipProgress = 0;

        // Loop until complete
        while (_flipProgress != 1)
        {
            // Break out if reached end goal
            _flipProgress += Time.deltaTime * pSpeed;
            if (_flipProgress >= 1)
                _flipProgress = 1;

            transform.parent.localEulerAngles = new Vector3(Mathf.Lerp(0, 360, _modelController.GetCatmullRomPosition(_flipProgress, _flipGraph).y), _flipYRotation, 0);
            yield return null;
        }

        // Return to original rotation
        transform.parent.eulerAngles = new Vector3(0, -90, 0);
        transform.localRotation = originalRotation;

        DisableRotation = false;
    }
}
