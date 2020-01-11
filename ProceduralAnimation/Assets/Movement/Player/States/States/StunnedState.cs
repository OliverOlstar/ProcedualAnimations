using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunnedState : BaseState
{
    PlayerStateController stateController;

    private float _leaveStateTime;
    private float _cooldown = 0.5f;

    public StunnedState(PlayerStateController controller) : base(controller.gameObject)
    {
        stateController = controller;
    }

    public override void Enter()
    {
        // Debug.Log("StunnedState: Enter");
        _leaveStateTime = Time.time + _cooldown;
        stateController.Stunned = false;
        stateController._hitboxComponent.gameObject.SetActive(false);
    }

    public override void Exit()
    {
         
    }

    public override Type Tick()
    {
        // Probably not optimal but frequently the hitbox doesn't stay active
        if (stateController._hitboxComponent.gameObject.activeSelf)
            stateController._hitboxComponent.gameObject.SetActive(false);

        if (Time.time >= _leaveStateTime)
        {
            return typeof(MovementState);
        }

        //Respawn
        if (stateController.Respawn)
        {
            stateController.Respawn = false;
            stateController._animHandler.Respawn();
            return typeof(MovementState);
        }

        return null;
    }
}
