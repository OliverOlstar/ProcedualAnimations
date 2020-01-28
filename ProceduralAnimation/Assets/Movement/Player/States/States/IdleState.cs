using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BaseState
{
    PlayerStateController stateController;

    public IdleState(PlayerStateController controller) : base(controller.gameObject)
    {
        stateController = controller;
    }

    public override void Enter()
    {
        //Debug.Log("IdleState: Enter");
        stateController._movementComponent.disableMovement = false;
        //stateController._playerCamera.Idle = true;
    }

    public override void Exit()
    {
        //Debug.Log("IdleState: Exit");
        stateController._movementComponent.disableMovement = true;
        //stateController._playerCamera.Idle = false;
    }

    public override Type Tick()
    {
        // TODO Add Taunt system

        // Idle
        if (stateController.moveInput.magnitude > 0)
        {
            return typeof(MovementState);
        }

        // Idle Dodge
        //if (stateController.dodgeInput != -1)
        //{
        //    return typeof(DodgeState);
        //}

        //Attack
        if (stateController.heavyAttackinput != -1.0f || stateController.lightAttackinput != -1.0f /* power input */)
        {
            if (stateController.AttackStateReturnDelay <= Time.time)
            {
                return typeof(AttackState);
            }
            else
            {
                //If Inputed attack before they can return to the attack state, remove the input
                stateController.heavyAttackinput = -1.0f;
                stateController.lightAttackinput = -1.0f;
            }
        }

        //Dead
        if (stateController._playerAttributes.getHealth() <= 0)
        {
            return typeof(DeathState);
        }

        //Stunned
        if (stateController.Stunned)
        {
            return typeof(StunnedState);
        }

        return null;
    }
}
