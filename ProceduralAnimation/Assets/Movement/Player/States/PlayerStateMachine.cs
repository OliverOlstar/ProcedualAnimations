using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    public Dictionary<Type, BaseState> _States;

    public BaseState CurrentState { get; private set; }

    private void Update()
    {
        if(CurrentState == null)
        {
            CurrentState = _States.Values.First();
        }

        var nextState = CurrentState?.Tick();

        if (nextState != null && nextState != CurrentState?.GetType())
        {
            CurrentState.Exit();
            SwitchToNextState(nextState);
            CurrentState.Enter();
        }
    }

    public void SetStates(Dictionary<Type, BaseState> states)
    {
        _States = states;
    }

    private void SwitchToNextState(Type pNextState)
    {
        CurrentState = _States[pNextState];
    }
}
