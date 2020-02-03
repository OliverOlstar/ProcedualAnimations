using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : BaseState
{
    PlayerStateController _stateController;

    [SerializeField] private int _numberOfClicks = 0;
    [SerializeField] private float _exitStateTime = 0;
    [SerializeField] private float _addForceTime = 0;
    [SerializeField] private float _stopForceTime = 0;
    [SerializeField] private float _addForceAmount = 0;

    private float AttackStateReturnDelayLength = 0.2f;

    private bool _onHolding = false;
    public float chargeTimer = 0f;
    public bool attacking = false;
    public bool clickActive = false;

    public AttackState(PlayerStateController controller) : base(controller.gameObject)
    {
        _stateController = controller;
    }

    public override void Enter()
    {
        Debug.Log("AttackState: Enter");
        //stateController._hitboxComponent.gameObject.SetActive(true); /* Handled by animation events */
        _exitStateTime = 0;
        _onHolding = false;
        CheckForAttack();
    }

    public override void Exit()
    {
        Debug.Log("AttackState: Exit");
        //stateController._hitboxComponent.gameObject.SetActive(false); /* Handled by animation events */
        _stateController.AttackStateReturnDelay = Time.time + AttackStateReturnDelayLength;
        //stateController._hitboxComponent.gameObject.SetActive(false);
        _numberOfClicks = 0;
        //stateController._modelController.ClearAttackBools();

        //_stateController._modelController.DoneAttack();
    }

    public override Type Tick()
    {
        // Check for another attack
        CheckForAttack();

        // Leave state after attack
        if (Time.time > _exitStateTime && _onHolding == false)
        {
            return typeof(MovementState);
        }

        if (Time.time > _addForceTime && Time.time < _stopForceTime && _onHolding == false)
        {
            _stateController._rb.AddForce(_stateController._modelController.transform.forward * _addForceAmount);
        }

        // Stunned
        if (_stateController.Stunned)
        {
            return typeof(StunnedState);
        }

        return null;
    }

    public void CheckForAttack()
    {
        // Cannot go beyond combo limit
        if (_numberOfClicks <= 2)
        {
            // If holding don't listen for more attacks, listen for release else run holding code.
            if (_onHolding == true)
            {
                // ON RELEASE HEAVY (Called Once)
                if (_stateController.heavyAttackinput == 0 || chargeTimer >= 2)
                {
                    ReleaseHeavyAttack();
                    ClearInputs();
                }
                // ON HOLDING HEAVY
                else
                {
                    chargeTimer += Time.deltaTime;

                    // If Reached max charge release heavy attack
                    if (chargeTimer >= 1f)
                    {
                        ReleaseHeavyAttack();
                    }
                }
            }
            // Can only input for next attack if done previous attack
            else if (Time.time > _exitStateTime || _exitStateTime == 0)
            {
                // ON RELEASED HEAVY BEFORE CHARGING STARTED (Called Once)
                if (_stateController.heavyAttackinput == 0)
                {
                    PressedHeavyAttack();
                    ReleaseHeavyAttack();
                    ClearInputs();
                }
                // ON PRESSED HEAVY (Called Once)
                else if (_stateController.heavyAttackinput == 1)
                {
                    PressedHeavyAttack();
                    ClearInputs();
                }
                // ON PRESSED LIGHT (Called Once)
                else if (_stateController.lightAttackinput == 1)
                {
                    PressedLightAttack();
                    ClearInputs();
                }
            }
        }
    }

    #region Pressed & Release
    private void PressedLightAttack()
    {
        SOAttack curAttack = _stateController._modelController.attacks[_numberOfClicks];
        float PreAttackTime = curAttack.transitionToTime + curAttack.holdStartPosTime;
        SetAttackValues(curAttack, PreAttackTime);

        _stateController._modelController.PlayAttack(_numberOfClicks, false, false);

        // TODO set player hitbox damage & knockback

        _numberOfClicks++;
    }

    private void PressedHeavyAttack()
    {
        _stateController._modelController.PlayAttack(_numberOfClicks, true, true);

        _exitStateTime = 0;

        chargeTimer = 0;
        _onHolding = true;
    }

    private void ReleaseHeavyAttack()
    {
        SOAttack curAttack = _stateController._modelController.attacks[_numberOfClicks + 3];
        SetAttackValues(curAttack);

        // TODO send through how long attack was charged for and use that to know how fast the attack should move.
        _stateController._modelController.DoneChargingAttack();

        // TODO set player hitbox damage & knockback (including charging)

        _onHolding = false;
        _numberOfClicks++;
    }

    private void ReleaseHeavyAttackBeforeCharging()
    {
        SOAttack curAttack = _stateController._modelController.attacks[_numberOfClicks + 3];
        float PreAttackTime = curAttack.transitionToTime + curAttack.holdStartPosTime;
        SetAttackValues(curAttack, PreAttackTime);

        _stateController._modelController.PlayAttack(_numberOfClicks, true, false);

        // TODO set player hitbox damage & knockback

        _numberOfClicks++;
    }
    #endregion

    #region Set & Clear
    private void SetAttackValues(SOAttack pAttackVars, float pPreAttackTime = 0)
    {
        _exitStateTime = Time.time + pAttackVars.attackTime + pPreAttackTime + pAttackVars.holdEndPosTime;
        _addForceTime = Time.time + pAttackVars.forceForwardTime + pPreAttackTime;
        _stopForceTime = Time.time + pAttackVars.stopForceForwardTime + pPreAttackTime;
        _addForceAmount = pAttackVars.forceForwardAmount;
    }

    private void ClearInputs()
    {
        _stateController.lightAttackinput = -1.0f;
        _stateController.heavyAttackinput = -1.0f;
    }
    #endregion
}