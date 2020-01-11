using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPivot : MonoBehaviour
{
    public GameObject target;
    private PlayerCamera _camera;
    public float offSetUp = 0.6f;

    private Coroutine transRoutine;

    //For Testing
    [Space]
    [SerializeField] private bool runFunc1 = false;
    [SerializeField] private SOCamera DemoVarsPreset;
    [SerializeField] private float DemoVarTrnsSpd = 1;

    void Start()
    {
        //Getting Reference to the camera
        _camera = GetComponentInChildren<PlayerCamera>();
    }

    void Update()
    {
        //Position the camera pivot on the player
        transform.position = target.transform.position + (Vector3.up * offSetUp);


        //For Testing
        if (runFunc1)
        {
            ChangePlayerCamera(DemoVarsPreset, DemoVarTrnsSpd);
            runFunc1 = false;
        }
    }

    

    // Camera Transition ////////////// Starts coroutines that lerp all camera variables
    public void ChangePlayerCamera(float pOffSetUp, float pOffSetLeft, float pTurnDampening, float pCameraDistance, float pCameraMinHeight, float pCameraMaxHeight, float pTransitionSpeed)
    {
        if (transRoutine != null)
            StopCoroutine(transRoutine);
        transRoutine = StartCoroutine(CameraOffSetTransition(pOffSetUp, pTransitionSpeed));

        _camera.ChangePlayerCamera(pOffSetLeft, pTurnDampening, pCameraDistance, pCameraMinHeight, pCameraMaxHeight, pTransitionSpeed);
    }

    public void ChangePlayerCamera(SOCamera pPreset, float pTransitionSpeed)
    {
        if (transRoutine != null)
            StopCoroutine(transRoutine);
        transRoutine = StartCoroutine(CameraOffSetTransition(pPreset.UpOffset, pTransitionSpeed));

        _camera.ChangePlayerCamera(pPreset.LeftOffset, pPreset.TurnDampening, pPreset.Distance, pPreset.MinY, pPreset.MaxY, pTransitionSpeed);
    }

    private IEnumerator CameraOffSetTransition(float pOffSetUp, float pTransitionSpeed)
    {
        while (Mathf.Abs(offSetUp - pOffSetUp) >= 0.01f)
        {
            offSetUp = Mathf.Lerp(offSetUp, pOffSetUp, pTransitionSpeed * Time.deltaTime);
            yield return null;
        }

        offSetUp = pOffSetUp;
    }
}
