using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeState : BaseState
{
    PlayerStateController stateController;

    public DodgeState(PlayerStateController controller) : base(controller.gameObject)
    {
        stateController = controller;
    }

    public override void Enter()
    {
        //Dodge Direction
        Vector3 vec = new Vector3(stateController.movementDir.x, 0, stateController.movementDir.z).normalized;

        //Start Dodge
        if (stateController._dodgeComponent.Dodge(stateController.shortDodgeInput, vec))
            stateController._animHandler.StartDodge(vec);

        //Remove Input
        stateController.shortDodgeInput = false;
        stateController.longDodgeInput = false;
    }

    public override void Exit()
    {
        //Stop Anim Dodge
        stateController._animHandler.StopDodge();
    }

    public override Type Tick()
    {
        //Leave Dodge
        if (stateController._dodgeComponent.doneDodge)
        {
            stateController._dodgeComponent.doneDodge = false;
            return typeof(MovementState);
        }

        //Respawn
        if (stateController.Respawn)
        {
            stateController.Respawn = false;
            stateController._animHandler.Respawn();
            stateController._dodgeComponent.EndDodge();
            return typeof(MovementState);
        }

        // TODO Maybe add way to enter stunned state from here

        return null;
    }
}
