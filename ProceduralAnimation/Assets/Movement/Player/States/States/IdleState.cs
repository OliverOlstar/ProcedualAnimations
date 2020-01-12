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
        Debug.Log("IdleState: Enter");
        stateController._movementComponent.disableMovement = false;
        //stateController._Camera
    }

    public override void Exit()
    {
        Debug.Log("IdleState: Exit");
        stateController._movementComponent.disableMovement = true;
    }

    public override Type Tick()
    {
        // TODO Add Taunt system

        // Idle
        if (stateController._movementComponent.moveInput.magnitude > 0)
        {
            return typeof(MovementState);
        }

        //Dodge
        //if (stateController.longDodgeInput || stateController.shortDodgeInput)
        //{
        //    return typeof(DodgeState);
        //}

        ////Attack
        //if (stateController.heavyAttackInput || stateController.quickAttackInput || stateController.powerInput > 0)
        //{
        //    if (stateController.AttackStateReturnDelay <= Time.time)
        //    {
        //        return typeof(AttackState);
        //    }
        //    else
        //    {
        //        //If Inputed attack before they can return to the attack state, remove the input
        //    }
        //}

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

        //Respawn (Already in the respawn to state)
        if (stateController.Respawn)
        {
            stateController._animHandler.Respawn();
            stateController.Respawn = false;
        }

        return null;
    }
}
