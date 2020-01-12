using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public GameObject target;

    private Transform _ParentTransform;
    private Vector3 _LocalRotation;
    private Vector3 _TargetLocalPosition;

    public Transform lockOnTarget;

    public bool Inverted = false;

    [Header("Idle")]
    [SerializeField] private float idleSpinSpeed = 1;
    [SerializeField] private float idleSpinY = 40;

    [Header("LockOn")]
    [SerializeField] private float _lockOnXOffset = 0;
    [SerializeField] private float _lockOnInputInfluence = 0.2f;
    private float _timeToChangeTarget = 0.0f;
    [SerializeField] private float _lockOnChangeDelay = 1.0f;
    [SerializeField] private float _lockOnChangeAmount_KB = 10.0f;
    [SerializeField] private float _lockOnChangeAmount_GP = 1.5f;

    [Header("Camera Collision")]
    [SerializeField] private LayerMask _cameraCollisionLayers = new LayerMask();
    [SerializeField] private float _cameraCollisionDampening = 20;
    [SerializeField] [Range(0, 1)] private float _cameraCollisionMinDisPercent = 0.1f;
    [SerializeField] private float _cameraCollisionOffset = 0.1f;

    [Space]
    [SerializeField] private float _mouseSensitivity = 4f;
    [SerializeField] private SOCamera _defaultPreset;

    private float _mouseSensitivityMult = 1f;
    private float _turnDampening = 10f;
    private float _offSetLeft = 0f;
    private float _offSetUp = 0.6f;
    private float _cameraDistance = 6f;
    private float _cameraMinHeight = -20f;
    private float _cameraMaxHeight = 90f;

    [Space]
    public bool CameraDisabled = false;
    public bool Idle = false;
    
    private Coroutine _transRoutine;

    void Start()
    {
        //Set Camera to default values
        ResetCameraVars();

        //Getting Transforms
        _ParentTransform = transform.parent;

        //Maintaining Starting Rotation
        _LocalRotation.x = _ParentTransform.eulerAngles.y;
        _LocalRotation.y = _ParentTransform.eulerAngles.x;

        //Locking cursor
        //Cursor.lockState = CursorLockMode.Locked;

        //Setting camera distance
        _TargetLocalPosition = new Vector3(-_offSetLeft, 0f, _cameraDistance * -1f);
        transform.localPosition = _TargetLocalPosition;
    }

    public void ResetCameraVars()
    {
        _mouseSensitivityMult = _defaultPreset.SensitivityMult;
        _offSetUp = _defaultPreset.UpOffset;
        _offSetLeft = _defaultPreset.LeftOffset;
        _cameraDistance = _defaultPreset.Distance;
        _cameraMinHeight = _defaultPreset.MinY;
        _cameraMaxHeight = _defaultPreset.MaxY;
    }

    public void Respawn(float pRotationY) 
    {
        transform.parent.rotation = Quaternion.Euler(0, pRotationY, 0);
        _LocalRotation.x = pRotationY;
        _LocalRotation.y = 0;
    }

    void Update()
    {
        //Getting Mouse Movement
        if (!CameraDisabled)
        {
            if (lockOnTarget != null)
            {
                LockOnCameraMovement();
            }
            else if (Idle == true)
            {
                IdleCameraMovement();
            }
            else
            {
                DefaultCameraMovement();
            }
        }

        //Actual Camera Transformations
        Quaternion TargetQ = Quaternion.Euler(_LocalRotation.y, _LocalRotation.x, 0);
        _ParentTransform.rotation = Quaternion.Lerp(_ParentTransform.rotation, TargetQ, Time.deltaTime * _turnDampening);

        //Position the camera pivot on the player
        _ParentTransform.position = target.transform.position + (Vector3.up * _offSetUp);

        //Camera Collision
        CameraCollision();
    }

    void DefaultCameraMovement(float pInputModifier = 1f)
    {
        //Rotation of the camera based on mouse movement
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            _LocalRotation.x += Input.GetAxis("Mouse X") * _mouseSensitivity * _mouseSensitivityMult * pInputModifier;
            _LocalRotation.y -= Input.GetAxis("Mouse Y") * _mouseSensitivity * _mouseSensitivityMult * pInputModifier * (Inverted ? -1 : 1);

            //Clamping the y rotation to horizon and not flipping over at the top
            if (_LocalRotation.y < _cameraMinHeight)
            {
                _LocalRotation.y = _cameraMinHeight;
            }
            else if (_LocalRotation.y > _cameraMaxHeight)
            {
                _LocalRotation.y = _cameraMaxHeight;
            }
        }
    }

    void LockOnCameraMovement()
    {
        //Locked onto Target
        Vector2 direction = new Vector2(lockOnTarget.position.z, lockOnTarget.position.x)  - new Vector2(_ParentTransform.position.z, _ParentTransform.position.x) ;
        _LocalRotation.x = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + (_lockOnXOffset * _offSetLeft); // Add distance into this line potentially
        _LocalRotation.y = _ParentTransform.position.y - lockOnTarget.position.y;

        Vector3 _RotTarget = _LocalRotation;

        DefaultCameraMovement(_lockOnInputInfluence);

        //Change Target
        //float RequiredPushAmount = ((Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0) ? lockOnChangeAmount_KB : lockOnChangeAmount_GP);
        //if ((_LocalRotation - _RotTarget).magnitude >= RequiredPushAmount * MouseSensitivity && timeToChangeTarget <= Time.time)
        //{
        //    timeToChangeTarget = Time.time + lockOnChangeDelay;
        //    Vector2 inputVector = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        //    lockOnTarget = lockOnScript.changeTarget(inputVector); //Vector tagent to camera forward but facing mouse input direction
        //}
    }

    void IdleCameraMovement()
    {
        //Slowly Rotate
        _LocalRotation.x += idleSpinSpeed * Time.deltaTime;
        _LocalRotation.y = Mathf.Lerp(_LocalRotation.y, idleSpinY, Time.deltaTime);
    }

    // Camera Collision //////////////
    void CameraCollision()
    {
        RaycastHit hit;
        Vector3 rayDirection = (transform.position - _ParentTransform.position).normalized;
        Physics.Raycast(_ParentTransform.position, rayDirection, out hit, _cameraDistance + _cameraCollisionOffset, _cameraCollisionLayers);

        if (hit.point != Vector3.zero)
        {
            hit.point -= _ParentTransform.position + rayDirection * _cameraCollisionOffset;
            transform.localPosition = Vector3.Lerp(transform.localPosition, _TargetLocalPosition * Mathf.Clamp((hit.point.magnitude / _TargetLocalPosition.magnitude), _cameraCollisionMinDisPercent, 0.5f), Time.deltaTime * _cameraCollisionDampening);
            //Debug.Log(hit.point.magnitude / _TargetLocalPosition.magnitude * 2 * 100 + "%");
            //Debug.DrawLine(_ParentTransform.position, hit.point + _ParentTransform.position, Color.red, 0.1f);
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, _TargetLocalPosition * 0.5f, Time.deltaTime * _cameraCollisionDampening);
        }
    }

    // Camera Transition ////////////// Lerp camera variables
    public void ChangePlayerCamera(float pOffSetUp, float pOffSetLeft, float pTurnDampening, float pCameraDistance, float pCameraMinHeight, float pCameraMaxHeight, float pSensitivityMult, float pTransitionSpeed)
    {
        if (_transRoutine != null)
            StopCoroutine(_transRoutine);
        _transRoutine = StartCoroutine(CameraVarsTransition(pOffSetUp, pOffSetLeft, pTurnDampening, pCameraDistance, pCameraMinHeight, pCameraMaxHeight, pSensitivityMult, pTransitionSpeed));
    }

    public void ChangePlayerCamera(SOCamera pPreset, float pTransitionSpeed)
    {
        if (_transRoutine != null)
            StopCoroutine(_transRoutine);
        _transRoutine = StartCoroutine(CameraVarsTransition(pPreset.UpOffset, pPreset.LeftOffset, pPreset.TurnDampening, pPreset.Distance, pPreset.MinY, pPreset.MaxY, pPreset.SensitivityMult, pTransitionSpeed));
    }
    public void ReturnToDefaultPlayerCamera(float pTransitionSpeed)
    {
        if (_transRoutine != null)
            StopCoroutine(_transRoutine);
        _transRoutine = StartCoroutine(CameraVarsTransition(_defaultPreset.UpOffset, _defaultPreset.LeftOffset, _defaultPreset.TurnDampening, _defaultPreset.Distance, _defaultPreset.MinY, _defaultPreset.MaxY, _defaultPreset.SensitivityMult, pTransitionSpeed));
    }

    public IEnumerator CameraVarsTransition(float pOffSetUp, float pOffSetLeft, float pTurnDampening, float pCameraDistance, float pCameraMinHeight, float pCameraMaxHeight, float pSensitivityMult, float pTransitionSpeed)
    {
        _mouseSensitivityMult = pSensitivityMult;

        short done;

        do
        {
            done = 0;

            //Lerping all of the values
            _turnDampening = Mathf.Lerp(_turnDampening, pTurnDampening, pTransitionSpeed * Time.deltaTime * 10);
            if (Mathf.Abs(_turnDampening - pTurnDampening) <= 0.01f)
            {
                _turnDampening = pTurnDampening;
                done++;
            }

            _cameraDistance = Mathf.Lerp(_cameraDistance, pCameraDistance, pTransitionSpeed * Time.deltaTime);
            if (Mathf.Abs(_cameraDistance - pCameraDistance) <= 0.01f)
            {
                _cameraDistance = pCameraDistance;
                done++;
            }

            _cameraMinHeight = Mathf.Lerp(_cameraMinHeight, pCameraMinHeight, pTransitionSpeed * Time.deltaTime);
            if (Mathf.Abs(_cameraMinHeight - pCameraMinHeight) <= 0.01f)
            {
                _cameraMinHeight = pCameraMinHeight;
                done++;
            }

            _cameraMaxHeight = Mathf.Lerp(_cameraMaxHeight, pCameraMaxHeight, pTransitionSpeed * Time.deltaTime);
            if (Mathf.Abs(_cameraMaxHeight - pCameraMaxHeight) <= 0.01f)
            {
                _cameraMaxHeight = pCameraMaxHeight;
                done++;
            }

            _offSetLeft = Mathf.Lerp(_offSetLeft, pOffSetLeft, pTransitionSpeed * Time.deltaTime);
            if (Mathf.Abs(_offSetLeft - pOffSetLeft) <= 0.01f)
            {
                _offSetLeft = pOffSetLeft;
                done++;
            }

            _offSetUp = Mathf.Lerp(_offSetUp, pOffSetUp, pTransitionSpeed * Time.deltaTime);
            if (Mathf.Abs(_offSetUp - pOffSetUp) <= 0.01f)
            {
                _offSetUp = pOffSetUp;
                done++;
            }

            //Setting camera distance
            _TargetLocalPosition = new Vector3(-_offSetLeft, 0f, _cameraDistance * -1f);

            yield return null;
        }
        while (done != 6);
    }
}
