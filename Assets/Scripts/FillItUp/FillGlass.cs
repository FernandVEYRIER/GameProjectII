using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FillGlass : MonoBehaviour {

    [SerializeField]
    Assets.Scripts.FillItUp.PlayerController player;

    [SerializeField] float fillStep = 0.25f;
    [SerializeField] Color liquidColor;
    [SerializeField] private GameObject _liquid;

    private Renderer _liquidMat;

    public float FillAmount { get; private set; }
    public bool IsEmpty { get { return FillAmount <= 0; } }

    // Use this for initialization
    void Start () {
        _liquid.SetActive(false);
        _liquid.transform.localScale = new Vector3(_liquid.transform.localScale.x, 0, _liquid.transform.localScale.z);
        _liquidMat = _liquid.GetComponent<Renderer>();
        _liquidMat.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        _liquidMat.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        _liquidMat.material.SetInt("_ZWrite", 0);
        _liquidMat.material.DisableKeyword("_ALPHATEST_ON");
        _liquidMat.material.DisableKeyword("_ALPHABLEND_ON");
        _liquidMat.material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        _liquidMat.material.renderQueue = 3000;
        _liquidMat.material.color = liquidColor;
    }

    public void liquideHeight(float h)
    {
        _liquid.SetActive(h > 0);
        _liquid.transform.localScale = new Vector3(_liquid.transform.localScale.x, h, _liquid.transform.localScale.z);

    }

    public void Fill()
    {
        if (Assets.Scripts.FillItUp.GameManager.Instance.isServer)
        {
            FillAmount += fillStep;
            if (FillAmount > 100)
                FillAmount = 100;
            var fa = FillAmount / 100f;
            _liquid.SetActive(fa > 0);
            _liquid.transform.localScale = new Vector3(_liquid.transform.localScale.x, fa * 15, _liquid.transform.localScale.z);
            player.height = fa * 15;
        }
    }

    /// <summary>
    /// Makes the container empty by removing every liquid in it.
    /// </summary>
    public void Empty()
    {
        if (Assets.Scripts.FillItUp.GameManager.Instance.isServer)
        {
            FillAmount = 0;
            _liquid.transform.localScale = new Vector3(_liquid.transform.localScale.x, 0, _liquid.transform.localScale.z);
            _liquid.SetActive(false);
            player.height = 0;
        }
    }

}
