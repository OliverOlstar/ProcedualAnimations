using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathState : BaseState
{
    PlayerStateController stateController;

    public DeathState(PlayerStateController controller) : base(controller.gameObject)
    {
        stateController = controller;
    }

    public override void Enter()
    {
        //stateController._respawnComponent.Dead();
        stateController._animHandler.Dead();
    }

    public override void Exit()
    {
        stateController._animHandler.Respawn();
    }

    public override Type Tick()
    {
        //Respawn
        if (stateController.Respawn)
        {
            stateController.Respawn = false;
            return typeof(MovementState);
        }

        return null;
    }
}
