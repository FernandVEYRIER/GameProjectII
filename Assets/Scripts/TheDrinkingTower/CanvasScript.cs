using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CanvasScript : NetworkBehaviour {
    public GameObject _panel = null;
    public GameObject _lost = null;
    public GameObject _win = null;
    public GameObject _loading = null;
    public Assets.Scripts.Test.ButtonScript player = null;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetPlayerFinished() {
        player.CmdLooserDrunk();
    }
}
