using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState
{
    // This is the template for all the states 
    

    private GameObject gameObject;
    private Transform transform;

    public BaseState(GameObject obj)
    {
        this.gameObject = obj;
        this.transform = obj.transform;
    }

    // The update function for this class.
    // Override in subsequent state classes.
    public abstract Type Tick();
    // The start function for this class.
    public abstract void Enter();
    // Function that runs before state exits.
    public abstract void Exit();
}
