using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Graph ScriptableObject", menuName = "Graph/New Interpolation Graph", order = 0)]
public class SOGraph : ScriptableObject
{
    public float EndValue = 0.5f;

    [Space]
    public Vector2 firstBender = new Vector2(0, -5);
    public Vector2 secondBender = new Vector2(1, 6);
}
