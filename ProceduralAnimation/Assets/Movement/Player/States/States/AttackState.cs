using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : BaseState
{
    PlayerStateController _stateController;

    [SerializeField] private int numberOfClicks = 0;
    [SerializeField] private float lastClickedTime = 0;
    [SerializeField] private float maxComboDelay = 0.8f;

    private float AttackStateReturnDelayLength = 0.2f;

    private bool onHolding = false;
    public float chargeTimer = 0f;
  
    public AttackState(PlayerStateController controller) : base(controller.gameObject)
    {
        _stateController = controller;
    }

    public override void Enter()
    {
        //Debug.Log("AttackState: Enter");
        //stateController._hitboxComponent.gameObject.SetActive(true); /* Handled by animation events */

        onHolding = false;
        CheckForAttack();
    }

    public override void Exit()
    {
        //Debug.Log("AttackState: Exit");
        //stateController._hitboxComponent.gameObject.SetActive(false); /* Handled by animation events */
        _stateController.AttackStateReturnDelay = Time.time + AttackStateReturnDelayLength;
        //stateController._hitboxComponent.gameObject.SetActive(false);
        numberOfClicks = 0;
        //stateController._modelController.ClearAttackBools();
    }

    public override Type Tick()
    {
        if (Time.time - lastClickedTime > maxComboDelay && onHolding == false)
        {
            return typeof(MovementState);
        }

        //CheckForAttack2();
        CheckForAttack();

        //Stunned
        if (_stateController.Stunned)
        {
            return typeof(StunnedState);
        }

        //Respawn
        if (_stateController.Respawn)
        {
            _stateController.Respawn = false;
            //stateController._modelController.Respawn();
            return typeof(MovementState);
        }

        return null;
    }
    public bool attacking = false;
    private bool heldAttack = true;
    float animSpeed = 0f;
    public bool clickActive = false;


    

    public void CheckForAttack()
    {
        if (numberOfClicks <= 2)
        {
            // On Release Heavy (Called Once)
            if ((_stateController.heavyAttackinput == 0 || chargeTimer >= 2) && onHolding == true)
            {
                ClearInputs();
                lastClickedTime = Time.time;

                numberOfClicks++;

                onHolding = false;

                //stateController._modelController.setAnimSpeed(1f);
            }

            // On Holding Heavy
            else if (onHolding)
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
                string boolName = "Triangle" + (numberOfClicks + 1).ToString();
                //stateController._modelController.StartAttack(boolName);

                chargeTimer = 0;
                onHolding = true;
            }

            // On Pressed Light Attack (Called Once)
            else if (_stateController.lightAttackinput == 1)
            {
                lastClickedTime = Time.time;
                numberOfClicks++;


                _stateController._modelController.PlayAttack();

                //stateController._modelController.ClearAttackBools();
                string boolName = "Square" + (numberOfClicks).ToString();
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
