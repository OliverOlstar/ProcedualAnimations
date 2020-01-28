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
        //Debug.Log("AttackState: Enter");
        //stateController._hitboxComponent.gameObject.SetActive(true); /* Handled by animation events */

        _onHolding = false;
        CheckForAttack();
    }

    public override void Exit()
    {
        //Debug.Log("AttackState: Exit");
        //stateController._hitboxComponent.gameObject.SetActive(false); /* Handled by animation events */
        _stateController.AttackStateReturnDelay = Time.time + AttackStateReturnDelayLength;
        //stateController._hitboxComponent.gameObject.SetActive(false);
        _numberOfClicks = 0;
        //stateController._modelController.ClearAttackBools();
    }

    public override Type Tick()
    {
        // Leave state after attack
        if (Time.time > _exitStateTime && _onHolding == false)
        {
            return typeof(MovementState);
        }

        if (Time.time > _addForceTime && _onHolding == false)
        {
            _stateController._rb.AddForce(_stateController._modelController.transform.forward * 20);
        }

        // Stunned
        if (_stateController.Stunned)
        {
            return typeof(StunnedState);
        }

        // Check for another attack
        CheckForAttack();

        return null;
    }

    public void CheckForAttack()
    {
        if (_numberOfClicks <= 2)
        {
            // On Release Heavy (Called Once)
            if ((_stateController.heavyAttackinput == 0 || chargeTimer >= 2) && _onHolding == true)
            {
                ClearInputs();

                // All attacks are 1 second length for now
                _exitStateTime = Time.time + 1;

                _numberOfClicks++;

                _onHolding = false;

                //stateController._modelController.setAnimSpeed(1f);
            }

            // On Holding Heavy
            else if (_onHolding)
            {
                chargeTimer += Time.deltaTime;

                if (chargeTimer >= 0.1f)
                {
                    //string animBoolName = "Vertical" + (numberOfClicks + 1).ToString();
                    //stateController._modelController.modifyAnimSpeed(-4f * Time.deltaTime);
                }    
            }

            // On Pressed Heavy (Called Once)
            else if (_stateController.heavyAttackinput == 1)
            {
                //stateController._modelController.ClearAttackBools();
                //string boolName = "Triangle" + (_numberOfClicks + 1).ToString();
                //stateController._modelController.StartAttack(boolName);

                chargeTimer = 0;
                _onHolding = true;
            }

            // On Pressed Light Attack (Called Once)
            else if (_stateController.lightAttackinput == 1)
            {
                _exitStateTime = Time.time + 1;
                _addForceTime = Time.time + 0.4f;
                _numberOfClicks++;


                _stateController._modelController.PlayAttack();

                //stateController._modelController.ClearAttackBools();
                //string boolName = "Square" + (_numberOfClicks).ToString();
                //stateController._modelController.StartAttack(boolName);
                ClearInputs();
            }
        }
    }


    private void ClearInputs()
    {
        _stateController.lightAttackinput = -1.0f;
        _stateController.heavyAttackinput = -1.0f;
    }
}
