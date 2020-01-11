using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Power ScriptableObject", menuName = "Powers/New Power Vars", order = 0)]
public class SOPowers : ScriptableObject
{
    public Sprite icon;
    public string dislayName = "Untitled";
    [TextArea] public string dislayDescription = "Description";

    [Space]
    public int animationIndex = 0;
    public PlayerPowerHandler.powers WhichPower;

    [Space]
    public int powerRequired = 0;
    public int damage = 0;

    [Header("Upgrades")]
    [TextArea] public string[] powerDescriptions = new string[3];
    public int[] cost = new int[3];
}
