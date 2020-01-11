using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Power_Template : MonoBehaviour, IPower
{
    public void Destroy() => Destroy(this);

    // Anim Events ///////////
    public void AEvent()
    {

    }
}