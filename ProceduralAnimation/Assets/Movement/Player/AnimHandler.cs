using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimHandler : MonoBehaviour
{
    //Animation Handling for the player model - Oliver

    [HideInInspector] public PlayerStateController _stateController;
    private Animator _anim;

    [HideInInspector] public int attackState = 0;

    [SerializeField] private float _rotationDampening = 11f;
    [SerializeField] private float _animSpeedDampening = 5f;

    private float offGroundTimer = 0;

    void Start()
    {
        _stateController = GetComponentInParent<PlayerStateController>();
        _anim = GetComponentInChildren<Animator>();

        //Temp (animation's origins are slightly off (my b))
        transform.GetChild(0).localPosition = new Vector3(0, -1.05f, 0);
    }
    
    void Update()
    {
        //If Movement isn't disabled
        if (_stateController._movementComponent.disableMovement == false)
        {
            //If Movement Inputed
            //if (_stateController._movementComponent.moveDirection != Vector3.zero)
            //{
            //    //Lerp Player Model Rotation
            //    Vector3 dir = _stateController._movementComponent.moveDirection;

            //    if (transform.forward.normalized == -_stateController._movementComponent.moveDirection.normalized)
            //        dir += new Vector3(Random.value / 15f, 0, Random.value / 15f);

            //    transform.forward = Vector3.Lerp(transform.forward, dir, Time.deltaTime * _rotationDampening);
            //}

            //Set Anim
            //_anim.SetFloat("Speed", Mathf.Lerp(_anim.GetFloat("Speed"), _stateController._movementComponent.moveDirection.magnitude, Time.deltaTime * _animSpeedDampening));
        }
        else
        {
            //Set Anim Speed Stopping
            _anim.SetFloat("Speed", Mathf.Lerp(_anim.GetFloat("Speed"), 0.1f, Time.deltaTime * _animSpeedDampening));
        }

        //Falling & OnGround
        _anim.SetBool("OnGround", _stateController.OnGround);

        if (_stateController.OnGround)
        {
            _anim.SetBool("Falling", false);
            offGroundTimer = 0;
        }
        else
        {
            offGroundTimer += Time.deltaTime;
            if (offGroundTimer >= 0.4f)
                _anim.SetBool("Falling", true);
        }
    }

    public void StartDodge(Vector3 pFacing)
    {
        transform.forward = pFacing;
        _anim.SetBool("Dodge", true);
        _anim.SetFloat("Speed", 0);
    }

    public void StopDodge()
    {
        _anim.SetBool("Dodge", false);
    }

    public void StartJump(Vector3 pFacing)
    {
        transform.forward = pFacing;
        _anim.SetTrigger("Jump");
        _anim.SetFloat("Speed", 0);
    }

    public void SetOnGround(bool pOnGround)
    {
        _anim.SetBool("OnGround", pOnGround);
    }

    public void Dead()
    {
        _anim.SetBool("IsDead", true);
        _anim.SetTrigger("Died");
    }

    public void StartAttack(bool pHeavy, int pCombo)
    {
        _anim.SetBool("Heavy Attack", pHeavy);
        _anim.SetInteger("Combo", pCombo);
        attackState = 0;
    }

    public void StartPower(int pPowerIndex)
    {
        _anim.SetInteger("WhichPower", pPowerIndex);
        _anim.SetTrigger("Power");
        attackState = 0;
    }

    public void StopAttacking()
    {
        if (attackState != 1)
            return;

        LeaveAttackState();
    }

    public void LeaveAttackState()
    {
        attackState = 2;
        _anim.SetInteger("Combo", 0);
    }

    public int GetCurrentCombo()
    {
        if (_anim.GetBool("Heavy Attack"))
            return 4;
        else
            return _anim.GetInteger("Combo");
    }

    public void Stunned(bool pLeft)
    {
        _anim.SetBool("ReactLeft", pLeft);
        _anim.SetTrigger("React");
        _stateController.Stunned = true;
    }
    
    public void Respawn()
    {
        _stateController.Stunned = false;
        _anim.SetTrigger("Respawn");
        _anim.SetBool("IsDead", false);
    }
}
