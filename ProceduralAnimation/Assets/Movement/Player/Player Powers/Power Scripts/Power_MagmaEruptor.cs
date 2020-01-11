using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Power_MagmaEruptor : MonoBehaviour, IPower
{
    public void Destroy() => Destroy(this);

    // Anim Events ///////////
    public void AESpawnMagmaball()
    {
        Debug.Log("Spawn Ball");
    }
}
