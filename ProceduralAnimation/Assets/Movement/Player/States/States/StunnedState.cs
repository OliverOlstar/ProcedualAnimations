using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunnedState : BaseState
{
    PlayerStateController stateController;

    private float _leaveStateTime;
    private float _cooldown = 0.2f;

    public StunnedState(PlayerStateController controller) : base(controller.gameObject)
    {
        stateController = controller;
    }

    public override void Enter()
    {
        // Debug.Log("StunnedState: Enter");
        _leaveStateTime = Time.time + _cooldown;
        stateController.Stunned = false;
    }

    public override void Exit()
    {
         
    }

    public override Type Tick()
    {
        if (Time.time >= _leaveStateTime)
        {
            return typeof(MovementState);
        }

        //Respawn
        if (stateController.Respawn)
        {
            stateController.Respawn = false;
            //stateController._modelController.Respawn();
            return typeof(MovementState);
        }

        return null;
    }
}
