using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EmiterManager : NetworkBehaviour
{

    [SerializeField] float nextBeerDrop;
    [SerializeField] BeerEmiter[] emiters;

    [SerializeField] private float dropChance = 0.2f;
    [SerializeField] private float dropChanceIncrease = 0.005f;

    [SerializeField] private float waterDropChance = 0.2f;
    [SerializeField] private float waterDropChanceIncrease = 0.006f;

    private bool emission;

    private void Start()
    {
        emission = true;
        //Invoke("StartEmision", 1.0f);
    }

    private void Update()
    {

        dropChance += dropChanceIncrease * Time.deltaTime;
        waterDropChance += waterDropChanceIncrease * Time.deltaTime;
        dropChance = dropChance > 1 ? 1 : dropChance;
        waterDropChance = waterDropChance > 1 ? 1 : waterDropChance;
    }

    private void StartEmision()
    {
        for (int i = 0; i < emiters.Length; i++)
        {
            if (Random.Range(0.0f, 1.0f) <= dropChance)
                Dropliquide(Random.Range(nextBeerDrop - 2, nextBeerDrop), Random.Range(0.0f, 1.0f) < waterDropChance? BeerEmiter.type.WATER : BeerEmiter.type.BEER, i);
        }
        if (emission == true)
            Invoke("StartEmision", nextBeerDrop);
    }

    [ClientRpc]
    public void RpcSeed(int seed)
    {
        Random.InitState(seed);
    }

    [ClientRpc]
    public void RpcStartEmision()
    {
        Invoke("StartEmision", 4.0f);
    }

    [ClientRpc]
    public void RpcStopEmission()
    {
        emission = false;
    }

    private void Dropliquide(float time, BeerEmiter.type liquide, int index)
    {
        emiters[index].DropLiquid(liquide, time);
    }
}
