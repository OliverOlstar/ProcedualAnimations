using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeComponent : MonoBehaviour
{
    //Oliver

    [HideInInspector] public bool doneDodge = false;

    public float shortDodgeCooldown = 1.0f;
    public float longDodgeCooldown = 1.0f;
    private float _dodgeDelay = 0.0f;

    [Space]
    [SerializeField] private float shortDodgeMaxSpeed = 6.0f;
    [SerializeField] private float shortDodgeDuration = 0.2f;
    [SerializeField] private float shortDodgeAcceleration = 100;

    [Space]
    [SerializeField] private float longDodgeMaxSpeed = 12.0f;
    [SerializeField] private float longDodgeDuration = 0.3f;
    [SerializeField] private float longDodgeAcceleration = 100;

    [Header("Dodge Movement Anim")]
    [SerializeField] private float shortDodgeAnimSpeed = 1;
    [SerializeField] private float longDodgeAnimSpeed = 1;

    private Rigidbody _rb;
    private PlayerStateController _stateController;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _stateController = GetComponent<PlayerStateController>();
    }

    public bool Dodge(bool pShortDodge, Vector2 pDirection)
    {
        //Cooldown
        if (Time.time > _dodgeDelay)
        {
            if (pShortDodge)
            {
                //Short Dodge
                StartCoroutine(DodgeRoutine(shortDodgeMaxSpeed, shortDodgeDuration, shortDodgeAcceleration, pDirection, shortDodgeCooldown));
                _stateController._modelController.PlayDodge(_stateController.LastMoveDirection, shortDodgeAnimSpeed);
            }
            else
            {
                //Long Dodge
                StartCoroutine(DodgeRoutine(longDodgeMaxSpeed, longDodgeDuration, longDodgeAcceleration, pDirection, longDodgeCooldown));
                _stateController._modelController.PlayDodge(_stateController.LastMoveDirection, longDodgeAnimSpeed);
            }

            return true;
        }
        else
        {
            Debug.Log("Dodge on Cooldown");
            doneDodge = true;
            return false;
        }
    }

    IEnumerator DodgeRoutine(float pMaxSpeed, float pDuration, float pAcceleration, Vector2 pDirection, float pCooldown)
    {
        //Setting Delay
        _dodgeDelay = Time.time + pCooldown + pDuration;
        float dodgeEndTime = Time.time + pDuration;

        //Run Dodge Force
        while (Time.time <= dodgeEndTime)
        {
            //Can end early
            if (doneDodge) break;

            //Move player
            Vector3 dodgeVector = new Vector3(pDirection.x, 0, pDirection.y).normalized * pAcceleration * Time.deltaTime;

            if (new Vector3(_rb.velocity.x, 0, _rb.velocity.z).magnitude < pMaxSpeed)
            {
                _rb.AddForce(dodgeVector);
            }

            yield return null;
        }

        doneDodge = true;
    }

    public void StopDodge()
    {
        StopCoroutine("DodgeRoutine");
    }
}
