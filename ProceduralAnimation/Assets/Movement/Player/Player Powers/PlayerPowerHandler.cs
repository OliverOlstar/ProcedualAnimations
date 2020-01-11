using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPowerHandler : MonoBehaviour
{
    private PlayerAttributes _playerAttributes;
    [SerializeField] private GameObject _model;

    [Space]
    [SerializeField] private SOPowers AddPowerTest;
    [SerializeField] private bool AddPowerTestActivate;

    //List of all powers to help with entering the desired power to be added
    public enum powers
    {
        MagmaErupter,
        IceQuake,
        SandCoffin
    }

    private List<SOPowers> _collectedPowers = new List<SOPowers>();

    private void Start()
    {
        _playerAttributes = GetComponent<PlayerAttributes>();
    }

    // TEMP FOR TESTING /////////////////////////
    private void FixedUpdate()
    {
        if (AddPowerTestActivate)
        {
            AddPower(AddPowerTest);
            AddPowerTestActivate = false;
        }
    }
    //////////////////////////////////////////////

    //Attack state calls this function
    public int UsingPower(int pPowerInput)
    {
        //If power specific button exists
        if (pPowerInput > 0 && pPowerInput <= _collectedPowers.Count)
        {
            //If required power
            if (_playerAttributes.getPower() >= _collectedPowers[pPowerInput - 1].powerRequired)
            {
                _playerAttributes.modifyPower(-_collectedPowers[pPowerInput - 1].powerRequired);

                //Returns animation index
                return _collectedPowers[pPowerInput - 1].animationIndex;
            }
            else
            {
                //Not enough power
                return -2;
            }
        }
        //Requested power doesnt exist
        return -1;
    }

    public void AddPower(SOPowers pPower)
    {
        // TODO Make sure you cant add two of the same power


        //Add power component based off of entered Power
        switch (pPower.WhichPower)
        {
            case powers.IceQuake:
                _model.AddComponent<Power_MagmaEruptor>();
                break;

            case powers.MagmaErupter:
                _model.AddComponent<Power_MagmaEruptor>();
                break;

            case powers.SandCoffin:
                _model.AddComponent<Power_MagmaEruptor>();
                break;
        }

        //Add power to list of collected powers
        _collectedPowers.Add(pPower);
    }


    // SAVE & LOAD
    public List<SOPowers> GetCollectedPowers()
    {
        return _collectedPowers;
    }

    public void Respawn(List<SOPowers> pPowers)
    {
        //Empty current powers
        _collectedPowers = new List<SOPowers>();

        //Remove current powers
        foreach(IPower power in _model.GetComponents<IPower>())
            power.Destroy();

        //Add powers back in
        foreach(SOPowers power in pPowers)
            AddPower(power);
    }
}
