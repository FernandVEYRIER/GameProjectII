using Assets.Scripts.Liquids;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeerEmiter : MonoBehaviour {

    public enum type
    {
        WATER,
        BEER,
    }

    [SerializeField]
    Color water, beer;

    private type liquidType;
    private ParticleHandler _particleHandler;

    private void Start()
    {
        _particleHandler = GetComponent<ParticleHandler>();
        _particleHandler.OnParticleCollided += ParticleHandler_OnParticleCollided;
        liquidType = type.BEER;
        //emiter.particleColor = beer;
    }

    public void DropLiquid(type liquid, float time)
    {
        gameObject.transform.localEulerAngles = new Vector3(90, 0, 0);
        liquidType = liquid;
        switch (liquidType)
        {
            case type.BEER:
                break;
            case type.WATER:
                break;
            default:
                break;
        }
        Invoke("StopDropping", time);
    }

    private void StopDropping()
    {
        gameObject.transform.localEulerAngles = new Vector3(0, 0, 0);
    }

    private void ParticleHandler_OnParticleCollided(object sender, GameObject obj)
    {
        var s = obj.GetComponent<FillGlass>();

        if (s != null)
        {
            switch (liquidType) 
            {
                case type.BEER:
                    s.Fill();
                    break;
                case type.WATER:
                    s.Empty();
                    break;
                default:
                    break;
            }
        }
    }
}
