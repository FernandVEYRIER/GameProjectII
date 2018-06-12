using Assets.Scripts.Liquids;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeerEmiter : MonoBehaviour {

    public enum type
    {
        WATER,
        BEER,
    }

    [SerializeField]
    Color water, beer;
    [SerializeField] Sprite spriteWater, spriteBeer;
    private Text text;
    private Image image;


    private type liquidType;
    private ParticleHandler _particleHandler;
    private ParticleSystem particleSystem;

    private void Start()
    {
        particleSystem = gameObject.GetComponent<ParticleSystem>();
        _particleHandler = GetComponent<ParticleHandler>();
        _particleHandler.OnParticleCollided += ParticleHandler_OnParticleCollided;

        text = gameObject.GetComponentInChildren<Text>();
        image = gameObject.GetComponentInChildren<Image>();
        text.enabled = false;
        image.enabled = false;
    }

    public void DropLiquid(type liquid, float time)
    {
        text.enabled = true;
        image.enabled = true;
        liquidType = liquid;
        var main = particleSystem.main;
        switch (liquidType)
        {
            case type.BEER:
                text.text = "Beer";
                image.sprite = spriteBeer;
                main.startColor = beer;
                break;
            case type.WATER:
                text.text = "Water";
                image.sprite = spriteWater;
                main.startColor = water;
                break;
            default:
                break;
        }
        Invoke("StartDropping", 0.5f);
        Invoke("StopDropping", time - 1);
    }

    private void StartDropping()
    {
        particleSystem.Play();
    }

    private void StopDropping()
    {
        text.enabled = false;
        image.enabled = false;
        particleSystem.Stop();
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
