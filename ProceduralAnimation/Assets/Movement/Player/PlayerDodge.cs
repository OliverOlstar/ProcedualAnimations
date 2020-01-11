using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDodge : MonoBehaviour
{
    //Oliver

    [HideInInspector] public bool doneDodge = false;

    public float shortDodgeCooldown = 1.0f;
    public float longDodgeCooldown = 1.0f;
    private float _dodgeDelay = 0.0f;

    [Space]
    [SerializeField] private float shortDodgeDistance = 6.0f;
    [SerializeField] private float shortDodgeDuration = 0.2f;

    [Space]
    [SerializeField] private float longDodgeDistance = 12.0f;
    [SerializeField] private float longDodgeDuration = 0.3f;

    private Rigidbody _Rb;

    void Start()
    {
        _Rb = GetComponent<Rigidbody>();
    }

    public bool Dodge(bool pShortDodge, Vector3 pDirection)
    {
        //Cooldown
        if (Time.time > _dodgeDelay)
        {
            if (pShortDodge)
            {
                //Short Dodge
                StartCoroutine(DodgeRoutine(shortDodgeDistance, shortDodgeDuration, pDirection, shortDodgeCooldown));
            }
            else
            {
                //Long Dodge
                StartCoroutine(DodgeRoutine(longDodgeDistance, longDodgeDuration, pDirection, longDodgeCooldown));
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

    IEnumerator DodgeRoutine(float pDistance, float pDuration, Vector3 pDirection, float pCooldown)
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
            _Rb.velocity = pDirection * (pDistance / pDuration);
            yield return null;
        }

        //Stop player
        //_Rb.velocity = _Rb.velocity.normalized;
        doneDodge = true;
    }

    public void EndDodge()
    {
        doneDodge = true;
    }
}
