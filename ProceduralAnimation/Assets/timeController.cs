using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class timeController : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
            Time.timeScale -= 0.1f;
        else if (Input.GetKeyDown(KeyCode.P))
            Time.timeScale += 0.1f;
    }
}
